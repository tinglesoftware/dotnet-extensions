using MongoDB.Driver.Core.Events;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace Tingle.Extensions.MongoDB.Diagnostics;

/// <summary>
/// A subscriber to events that writes to a <see cref="DiagnosticListener"/>
/// </summary>
/// <remarks>
/// This class is highly borrowed from https://github.com/jbogard/MongoDB.Driver.Core.Extensions.DiagnosticSources
/// </remarks>
public class MongoDbDiagnosticEvents : IEventSubscriber
{
    internal static readonly AssemblyName AssemblyName = typeof(MongoDbDiagnosticEvents).Assembly.GetName();
    internal static readonly string ActivitySourceName = AssemblyName.Name!;
    internal static readonly Version Version = AssemblyName.Version!;
    internal static readonly ActivitySource ActivitySource = new(ActivitySourceName, Version.ToString());

    private const string ActivityName = "MongoDB";

    private readonly bool captureCommandText = false; // Ideally should be provided via constructor
    private readonly Func<CommandStartedEvent, bool>? shouldStartActivity;
    private readonly ReflectionEventSubscriber subscriber;
    private readonly ConcurrentDictionary<int, Activity> activityMap = new();

    /// <summary>
    /// Creates an instance of <see cref="MongoDbDiagnosticEvents"/>.
    /// </summary>
    /// <param name="captureCommandText">indicates if the command text should be captured</param>
    /// <param name="shouldStartActivity">optional delegate to check if an activity should be started</param>
    public MongoDbDiagnosticEvents(bool captureCommandText = false, Func<CommandStartedEvent, bool>? shouldStartActivity = null)
    {
        this.captureCommandText = captureCommandText;
        this.shouldStartActivity = shouldStartActivity;

        // the reflection-based subscriber accepts any objects, for this case, we take non public ones
        subscriber = new ReflectionEventSubscriber(this, bindingFlags: BindingFlags.Instance | BindingFlags.NonPublic);
    }

    /// <summary>
    /// Tries to get an event handler for an event of type TEvent.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="handler">The handler.</param>
    /// <returns>true if this subscriber has provided an event handler; otherwise false.</returns>
    public bool TryGetEventHandler<TEvent>(out Action<TEvent> handler) => subscriber.TryGetEventHandler(out handler);

#pragma warning disable IDE0051 // Remove unused private members

    private void Handle(CommandStartedEvent @event)
    {
        if (shouldStartActivity != null && !shouldStartActivity(@event))
        {
            return;
        }

        var activity = ActivitySource.StartActivity(ActivityName, ActivityKind.Client);

        // if the activity is null, there is no one listening so just return
        if (activity == null) return;

        var collectionName = @event.GetCollectionName();

        // https://github.com/open-telemetry/opentelemetry-specification/blob/master/specification/trace/semantic_conventions/database.md
        activity.DisplayName = collectionName == null ? $"mongodb.{@event.CommandName}" : $"{collectionName}.{@event.CommandName}";

        // add tags known by open telemetry
        activity.AddTag("db.system", "mongodb");
        activity.AddTag("db.name", @event.DatabaseNamespace?.DatabaseName);
        activity.AddTag("db.mongodb.collection", collectionName);
        activity.AddTag("db.operation", @event.CommandName);
        var endPoint = @event.ConnectionId?.ServerId?.EndPoint;
        switch (endPoint)
        {
            case IPEndPoint ipe:
                activity.AddTag("db.user", $"mongodb://{ipe.Address}:{ipe.Port}");
                activity.AddTag("net.peer.ip", ipe.Address.ToString());
                activity.AddTag("net.peer.port", ipe.Port.ToString());
                break;
            case DnsEndPoint dnse:
                activity.AddTag("db.user", $"mongodb://{dnse.Host}:{dnse.Port}");
                activity.AddTag("net.peer.name", dnse.Host);
                activity.AddTag("net.peer.port", dnse.Port.ToString());
                break;
        }

        if (activity.IsAllDataRequested && captureCommandText)
        {
            activity.AddTag("db.statement", @event.Command.ToString());
        }

        activityMap.TryAdd(@event.RequestId, activity);
    }

    private void Handle(CommandSucceededEvent @event)
    {
        if (activityMap.TryRemove(@event.RequestId, out var activity))
        {
            WithReplacedActivityCurrent(activity, () =>
            {
                activity.AddTag("otel.status_code", "Ok");
                activity.Stop();
            });
        }
    }

    private void Handle(CommandFailedEvent @event)
    {
        if (activityMap.TryRemove(@event.RequestId, out var activity))
        {
            WithReplacedActivityCurrent(activity, () =>
            {
                if (activity.IsAllDataRequested)
                {
                    activity.AddTag("otel.status_code", "Error");
                    activity.AddTag("otel.status_description", @event.Failure.Message);
                    activity.AddTag("error.type", @event.Failure.GetType().FullName);
                    activity.AddTag("error.msg", @event.Failure.Message);
                    activity.AddTag("error.stack", @event.Failure.StackTrace);
                }

                activity.Stop();
            });
        }
    }

    private static void WithReplacedActivityCurrent(Activity activity, Action action)
    {
        var current = Activity.Current;
        try
        {
            Activity.Current = activity;
            action();
        }
        finally
        {
            Activity.Current = current;
        }
    }

#pragma warning restore IDE0051 // Remove unused private members
}
