# Tingle.AspNetCore.ApplicationInsights

> [!IMPORTANT]
> Use of OpenTelemetry is recommended for instrumentation. This library is provided for convenience and compatibility with existing applications but is no longer actively maintained and not recommended for new projects.

This library provides some extensions for [Microsoft.ApplicationInsights](https://github.com/Microsoft/ApplicationInsights-dotnet).

## Add details of the request source to application insights telemetry

You can add some request source details to the `RequestTelemetry` that is sent along to application insights. The following details can be added:

- The package name which should be found in the `X-App-Package-Id` request header
- The version name which should be found in the `X-App-Version-Name` request header
- The version code which should be found in the `X-App-Version-Code` request header
- The User Agent which should be found in the `User-Agent` request header
- The IP address

To accomplish this, add the following logic to `Program.cs` or `Startup.cs` file:

```cs
services.AddApplicationInsightsTelemetryExtras();
services.AddHttpContextAccessor();
```

We've injected the `IHttpContextAccessor` in the DI container so that we can access the `HttpContext` object that contains the HTTP request details.

The request source details will be seen as custom properties of an application insights telemetry record.

## Add all request headers to application insights telemetry

You can send all request headers to application insights by adding the following logic to `Program.cs` or `Startup.cs` file:

```cs
services.AddApplicationInsightsTelemetryHeaders();
services.AddHttpContextAccessor();
```

We've injected the `IHttpContextAccessor` in the DI container so that we can access the `HttpContext` object that contains the HTTP request details.

The request headers will be seen as custom properties of an application insights telemetry record.

## Add manual dependency tracking in application insights

A dependency is a component that's called by your application. It's typically a service called by using HTTP, a database, or a file system. Application Insights measures the duration of dependency calls and whether it's failing or not, along with information like the name of the dependency. You can investigate specific dependency calls and correlate them to requests and exceptions. The list of dependencies that are automatically tracked can be seen [here](https://learn.microsoft.com/en-us/azure/azure-monitor/app/asp-net-dependencies#automatically-tracked-dependencies).

So how do we assist in tracking of dependencies that aren't automatically tracked?

We do this by using [ActivitySource](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.activitysource?view=net-5.0&ref=jimmybogard.com)/[ActivityListener](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.activitylistener?view=net-5.0&ref=jimmybogard.com) APIs, which make it quite a bit simpler to raise and listen to events for `Activity` start/stop.

Per conventions, the activity source name should be the name of the assembly creating the activities. That makes it much easier to "discover" activities, you don't have to expose a constant or search through source code to discern the name.

We can then create an `IHostedService` that uses `ActivityListener` internally, collects from the `ActivitySources` that are needed, creates instance(s) of `DependencyTelemetry` then sends to application insights via the [TrackDependency API](https://learn.microsoft.com/en-us/azure/azure-monitor/app/api-custom-events-metrics#trackdependency).

This can be accomplished by adding the following logic to `Program.cs` or `Startup.cs` file:

```cs
services.AddActivitySourceDependencyCollector(["Tingle.EventBus", "Tingle.Extensions.MongoDB"]);
```

You can replace the array of activities supplied in `AddActivitySourceDependencyCollector(...)` with your own.

## Track problem details in application insights

Track the problem details in application insights when the response is a `BadRequestObjectResult` with value of `ProblemDetails`. The properties are seen as custom properties of an application insights telemetry record. To do this, annotate your controller with the `TrackProblems` attribute:

```cs
[TrackProblems]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class DummyController : ControllerBase
{
  [HttpPost]
  public async Task<IActionResult> SendAsync([FromBody] SendRequestModel model)
  {
      ...
      // In case of a bad request
      return Problem(title: "error_title", description: "more detailed description", statusCode: 400);
      ...
    }
}
```
