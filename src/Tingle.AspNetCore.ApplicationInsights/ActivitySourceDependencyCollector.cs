using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace Tingle.AspNetCore.ApplicationInsights;

// See https://github.com/microsoft/ApplicationInsights-dotnet/issues/1427
internal class ActivitySourceDependencyCollector : IHostedService
{
    private readonly TelemetryClient client;
    private readonly IDictionary<string, ActivitySamplingResult> activities;
    private readonly ActivityListener? listener;

    public ActivitySourceDependencyCollector(TelemetryClient client, IDictionary<string, ActivitySamplingResult> activities)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.activities = activities ?? throw new ArgumentNullException(nameof(activities));

        if (activities.Count > 0)
        {
            listener = new ActivityListener
            {
                ShouldListenTo = ShouldListenTo,
                Sample = Sample,
                ActivityStopped = ActivityStopped,
            };
        }
    }

    private bool ShouldListenTo(ActivitySource source) => !string.IsNullOrWhiteSpace(source.Name) && activities.ContainsKey(source.Name);

    private ActivitySamplingResult Sample(ref ActivityCreationOptions<ActivityContext> options) => activities[options.Source.Name];

    internal void ActivityStopped(Activity activity)
    {
        // extensibility point - can chain more telemetry extraction methods here
        var telemetry = ExtractDependencyTelemetry(activity);
        if (telemetry == null)
        {
            return;
        }

        // properly fill dependency telemetry operation context
        if (activity.IdFormat == ActivityIdFormat.W3C)
        {
            telemetry.Context.Operation.Id = activity.TraceId.ToHexString();
            if (activity.ParentSpanId != default)
            {
                telemetry.Context.Operation.ParentId = activity.ParentSpanId.ToHexString();
            }

            telemetry.Id = activity.SpanId.ToHexString();
        }
        else
        {
            telemetry.Id = activity.Id;
            telemetry.Context.Operation.Id = activity.RootId;
            telemetry.Context.Operation.ParentId = activity.ParentId;
        }

        telemetry.Timestamp = activity.StartTimeUtc;

        //telemetry.Properties["DiagnosticSource"] = diagnosticListener.Name;
        telemetry.Properties["Activity"] = activity.OperationName;

        client.TrackDependency(telemetry);
    }

    internal static DependencyTelemetry ExtractDependencyTelemetry(Activity activity)
    {
        var telemetry = new DependencyTelemetry
        {
            Id = activity.Id,
            Duration = activity.Duration,
            Name = activity.OperationName,
        };

        Uri? requestUri = null;
        string? component = null;
        string? queryStatement = null;
        string? httpUrl = null;
        string? peerAddress = null;
        string? peerService = null;

        foreach (KeyValuePair<string, string?> tag in activity.Tags)
        {
            // interpret Tags as defined by OpenTracing conventions
            // https://github.com/opentracing/specification/blob/master/semantic_conventions.md
            switch (tag.Key)
            {
                case "component":
                    {
                        component = tag.Value;
                        break;
                    }

                case "db.statement":
                    {
                        queryStatement = tag.Value;
                        break;
                    }

                case "error":
                    {
                        if (bool.TryParse(tag.Value, out var failed))
                        {
                            telemetry.Success = !failed;
                            continue; // skip Properties
                        }

                        break;
                    }

                case "http.status_code":
                    {
                        telemetry.ResultCode = tag.Value;
                        continue; // skip Properties
                    }

                case "http.method":
                    {
                        continue; // skip Properties
                    }

                case "http.url":
                    {
                        httpUrl = tag.Value;
                        if (Uri.TryCreate(tag.Value, UriKind.RelativeOrAbsolute, out requestUri))
                        {
                            continue; // skip Properties
                        }

                        break;
                    }

                case "peer.address":
                    {
                        peerAddress = tag.Value;
                        break;
                    }

                case "peer.hostname":
                    {
                        telemetry.Target = tag.Value;
                        continue; // skip Properties
                    }

                case "peer.service":
                    {
                        peerService = tag.Value;
                        break;
                    }
            }

            // if more than one tag with the same name is specified, the first one wins
            // verify if still needed once https://github.com/Microsoft/ApplicationInsights-dotnet/issues/562 is resolved
            if (!telemetry.Properties.ContainsKey(tag.Key))
            {
                telemetry.Properties.Add(tag);
            }
        }

        if (string.IsNullOrEmpty(telemetry.Type))
        {
            telemetry.Type = peerService ?? component /*?? diagnosticListener.Name*/ ?? activity.OperationName;
        }

        if (string.IsNullOrEmpty(telemetry.Target))
        {
            // 'peer.address' can be not user-friendly, thus use only if nothing else specified
            telemetry.Target = requestUri?.Host ?? peerAddress;
        }

        if (string.IsNullOrEmpty(telemetry.Name))
        {
            telemetry.Name = activity.OperationName;
        }

        if (string.IsNullOrEmpty(telemetry.Data))
        {
            telemetry.Data = queryStatement ?? requestUri?.OriginalString ?? httpUrl;
        }

        return telemetry;
    }

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (listener is not null)
        {
            ActivitySource.AddActivityListener(listener);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        listener?.Dispose();
        return Task.CompletedTask;
    }
}
