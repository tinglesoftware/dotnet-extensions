using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Events;
using MongoDB.Driver.Core.Servers;
using System.Diagnostics;
using System.Net;
using Tingle.Extensions.MongoDB.Diagnostics;

namespace Tingle.Extensions.MongoDB.Tests;

public class MongoDbDiagnosticEventsTests
{
    static MongoDbDiagnosticEventsTests()
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;
    }

    [Fact]
    public void NoActivityCreatedWhenNoListenerIsAttached()
    {
        var startFired = false;
        var stopFired = false;

        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Nonsense",
            ActivityStarted = _ => startFired = true,
            ActivityStopped = _ => stopFired = true
        };

        ActivitySource.AddActivityListener(listener);

        var behavior = new MongoDbDiagnosticEvents();

        Assert.True(behavior.TryGetEventHandler<CommandStartedEvent>(out var startEvent));
        Assert.True(behavior.TryGetEventHandler<CommandSucceededEvent>(out var stopEvent));

        startEvent(new CommandStartedEvent());
        stopEvent(new CommandSucceededEvent());

        Assert.False(startFired);
        Assert.False(stopFired);
    }

    [Fact]
    public void ActivityStartedAndStoppedWhenSampling()
    {
        var startFired = false;
        var stopFired = false;

        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Tingle.Extensions.MongoDB",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.PropagationData,
            ActivityStarted = _ => startFired = true,
            ActivityStopped = _ => stopFired = true
        };
        ActivitySource.AddActivityListener(listener);

        var behavior = new MongoDbDiagnosticEvents();

        Assert.True(behavior.TryGetEventHandler<CommandStartedEvent>(out var startEvent));
        Assert.True(behavior.TryGetEventHandler<CommandSucceededEvent>(out var stopEvent));

        startEvent(new CommandStartedEvent());
        stopEvent(new CommandSucceededEvent());

        Assert.True(startFired);
        Assert.True(stopFired);
        Assert.Null(Activity.Current);
    }

    [Fact]
    public void StartsAndLogsSuccessfulActivity()
    {
        var stopFired = false;
        var startFired = false;

        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Tingle.Extensions.MongoDB",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.PropagationData,
            ActivityStarted = activity =>
            {
                startFired = true;
                Assert.NotNull(activity);
                Assert.Equal("MongoDB", Activity.Current?.OperationName);
            },
            ActivityStopped = activity =>
            {
                stopFired = true;
                Assert.NotNull(activity);
                Assert.Equal("MongoDB", Activity.Current?.OperationName);
            }
        };
        ActivitySource.AddActivityListener(listener);

        var behavior = new MongoDbDiagnosticEvents();

        Assert.True(behavior.TryGetEventHandler<CommandStartedEvent>(out var startEvent));
        Assert.True(behavior.TryGetEventHandler<CommandSucceededEvent>(out var stopEvent));

        startEvent(new CommandStartedEvent());
        stopEvent(new CommandSucceededEvent());

        Assert.True(startFired);
        Assert.True(stopFired);
        Assert.Null(Activity.Current);
    }

    [Fact]
    public void StartsAndLogsFailedActivity()
    {
        var exceptionFired = false;
        var startFired = false;

        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Tingle.Extensions.MongoDB",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStarted = activity =>
            {
                startFired = true;
                Assert.NotNull(activity);
                Assert.Equal("MongoDB", Activity.Current?.OperationName);
            },
            ActivityStopped = activity =>
            {
                exceptionFired = true;
                Assert.NotNull(activity);
                Assert.Equal("MongoDB", Activity.Current?.OperationName);
                var statusTag = activity.Tags.SingleOrDefault(t => t.Key == "otel.status_code");
                Assert.NotEqual(default, statusTag);
                Assert.Equal("Error", statusTag.Value);
            }
        };
        ActivitySource.AddActivityListener(listener);

        var behavior = new MongoDbDiagnosticEvents();

        Assert.True(behavior.TryGetEventHandler<CommandStartedEvent>(out var startEvent));
        Assert.True(behavior.TryGetEventHandler<CommandFailedEvent>(out var stopEvent));

        var connectionId = new ConnectionId(new ServerId(new ClusterId(), new DnsEndPoint("localhost", 8000)));
        var databaseNamespace = new DatabaseNamespace("test");
        var command = new BsonDocument(new Dictionary<string, object>
        {
            {"update", "my_collection"}
        });
        startEvent(new CommandStartedEvent("update", command, databaseNamespace, null, 1, connectionId));
        stopEvent(new CommandFailedEvent("update", databaseNamespace, new Exception("Failed"), null, 1, connectionId, TimeSpan.Zero));

        Assert.True(startFired);
        Assert.True(exceptionFired);
        Assert.Null(Activity.Current);
    }

    [Fact]
    public void RecordsAllData()
    {
        var stopFired = false;
        var startFired = false;

        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Tingle.Extensions.MongoDB",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStarted = activity =>
            {
                startFired = true;
                Assert.NotNull(activity);
            },
            ActivityStopped = activity =>
            {
                stopFired = true;
                Assert.NotNull(activity);
                Assert.Equal("MongoDB", Activity.Current?.OperationName);
                var instanceTag = activity.Tags.SingleOrDefault(t => t.Key == "db.name");
                Assert.NotEqual(default, instanceTag);
                Assert.Equal("test", instanceTag.Value);

                Assert.Equal("mongodb", activity.Tags.SingleOrDefault(t => t.Key == "db.system").Value);
                Assert.Equal("update", activity.Tags.SingleOrDefault(t => t.Key == "db.operation").Value);
                Assert.Equal(default, activity.Tags.SingleOrDefault(t => t.Key == "db.statement").Value);
            }
        };
        ActivitySource.AddActivityListener(listener);

        var behavior = new MongoDbDiagnosticEvents(captureCommandText: false);

        Assert.True(behavior.TryGetEventHandler<CommandStartedEvent>(out var startEvent));
        Assert.True(behavior.TryGetEventHandler<CommandSucceededEvent>(out var stopEvent));

        var connectionId = new ConnectionId(new ServerId(new ClusterId(), new DnsEndPoint("localhost", 8000)));
        var databaseNamespace = new DatabaseNamespace("test");
        var command = new BsonDocument(new Dictionary<string, object>
        {
            {"update", "my_collection"}
        });
        startEvent(new CommandStartedEvent("update", command, databaseNamespace, null, 1, connectionId));
        stopEvent(new CommandSucceededEvent("update", command, databaseNamespace, null, 1, connectionId, TimeSpan.Zero));

        Assert.True(startFired);
        Assert.True(stopFired);
    }

    [Fact]
    public void RecordsCommandTextWhenOptionIsSet()
    {
        var stopFired = false;
        var startFired = false;

        var command = new BsonDocument(new Dictionary<string, object>
            {
                {"update", "my_collection"}
            });

        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Tingle.Extensions.MongoDB",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStarted = activity =>
            {
                startFired = true;
                Assert.NotNull(activity);
            },
            ActivityStopped = activity =>
            {
                Assert.NotNull(activity);
                Assert.Equal("MongoDB", Activity.Current?.OperationName);
                var statementTag = activity.Tags.SingleOrDefault(t => t.Key == "db.statement");
                Assert.NotEqual(default, statementTag);
                Assert.Equal(command.ToString(), statementTag.Value);

                stopFired = true;
            }
        };
        ActivitySource.AddActivityListener(listener);

        var behavior = new MongoDbDiagnosticEvents(captureCommandText: true);

        Assert.True(behavior.TryGetEventHandler<CommandStartedEvent>(out var startEvent));
        Assert.True(behavior.TryGetEventHandler<CommandSucceededEvent>(out var stopEvent));

        var connectionId = new ConnectionId(new ServerId(new ClusterId(), new DnsEndPoint("localhost", 8000)));
        var databaseNamespace = new DatabaseNamespace("test");
        startEvent(new CommandStartedEvent("update", command, databaseNamespace, null, 1, connectionId));
        stopEvent(new CommandSucceededEvent("update", command, databaseNamespace, null, 1, connectionId, TimeSpan.Zero));

        Assert.True(startFired);
        Assert.True(stopFired);
    }

    [Fact]
    public void WorksWithParallelActivities()
    {
        var activities = new List<Activity?>();

        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Tingle.Extensions.MongoDB",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = _ => activities.Add(Activity.Current),
            ActivityStopped = _ => activities.Add(Activity.Current)
        };
        ActivitySource.AddActivityListener(listener);

        var behavior = new MongoDbDiagnosticEvents();

        Assert.True(behavior.TryGetEventHandler<CommandStartedEvent>(out var startEvent));
        Assert.True(behavior.TryGetEventHandler<CommandSucceededEvent>(out var stopEvent));

        var outerActivity = new Activity("Outer");
        outerActivity.Start();

        var connectionId = new ConnectionId(new ServerId(new ClusterId(), new DnsEndPoint("localhost", 8000)));
        var databaseNamespace = new DatabaseNamespace("test");
        var updateCommand = new BsonDocument(new Dictionary<string, object>
        {
            {"update", "my_collection"}
        });
        var insertCommand = new BsonDocument(new Dictionary<string, object>
        {
            {"insert", "my_collection"}
        });
        startEvent(new CommandStartedEvent("update", updateCommand, databaseNamespace, null, 1, connectionId));
        startEvent(new CommandStartedEvent("insert", insertCommand, databaseNamespace, null, 2, connectionId));
        stopEvent(new CommandSucceededEvent("update", updateCommand, databaseNamespace, null, 1, connectionId, TimeSpan.Zero));
        stopEvent(new CommandSucceededEvent("insert", insertCommand, databaseNamespace, null, 2, connectionId, TimeSpan.Zero));

        outerActivity.Stop();

        Assert.Equal(4, activities.Count);
        Assert.Equal(4, activities.Count(a => a != null && a.OperationName == "MongoDB"));
        Assert.Null(Activity.Current);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void ShouldStartActivityIsRespected(bool? filterResult, bool shouldFireActivity)
    {
        var activities = new List<Activity?>();

        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Tingle.Extensions.MongoDB",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.PropagationData,
            ActivityStarted = _ => activities.Add(Activity.Current)
        };
        ActivitySource.AddActivityListener(listener);

        Func<CommandStartedEvent, bool>? filter = filterResult == null ? null : x => filterResult.Value;

        var behavior = new MongoDbDiagnosticEvents(shouldStartActivity: filter);

        Assert.True(behavior.TryGetEventHandler<CommandStartedEvent>(out var startEvent));
        Assert.True(behavior.TryGetEventHandler<CommandSucceededEvent>(out var stopEvent));

        startEvent(new CommandStartedEvent());
        stopEvent(new CommandSucceededEvent());

        Assert.Equal(shouldFireActivity ? 1 : 0, activities.Count);
    }
}
