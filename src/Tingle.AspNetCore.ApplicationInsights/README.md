# Tingle.AspNetCore.ApplicationInsights

> [!CAUTION]
> This documentation for `Tingle.AspNetCore.ApplicationInsights` may not cover the most recent version of the code. Use it sparingly
> 
> See <https://github.com/tinglesoftware/dotnet-extensions/issues/224>

This is a customized library developed by Tingle. The `ApplicationInsights` package in AspNetCore offers functionality to understand how the application is performing and how it is being used. `Tingle.AspNetCore.ApplicationInsights` has additional functionality for Application Insights on AspNetCore mainly used in building Tingle APIs and Services.

## Adding to Service Collections

The following logic is added to `Program.cs` file:

```csharp
// Enables Application Insights telemetry collection
builder.Services.AddApplicationInsightsTelemetryExtras(builder.Configuration)
```

This library has a number of extensibility points, one of them being the telemetry initializer. To implement the telemetry initializer, a class is created that implements the `ITelemetryInitializer` interface.  In this case, `ExtrasTelemetryInitializer` is a class implementing `ITelemetryInitializer` interface. The `ExtrasTelemetryInitializer` extends ApplicationInsights telemetry collection by supplying additional information about the application which includes the `AppPackageId`, `AppVersionName`, `AppVersionCode`, `AppClient`, `AppIpAddress` and `AppKind`.

In the `ITelemetryInitializer` interface's `Initialize` method, a `HttpContext` object is created. Whenever a new HTTP request or response is made, a `HttpContext` object is created which wraps all HTTP related information in one place. The `HttpContext` object is accessed through the `IHttpContextAccessor` and its default implementation `HttpAccessor`. It is necessary to use the `IHttpContextAccessor` so as to access the `HttpContext` object within a service.

The initializers are used to mark every collected telemetry item with the current web request identity so that traces and exceptions can be correlated to corresponding requests.

The library is used to track error details in application insights when the response is `BadRequestObjectResult` with the value of type `ProblemDetails`.

From the request header, the application's package ID, version name, version code, client, IP address and kind properties are extracted so that the traces and exceptions can be correlated/matched to the corresponding requests.

## Configuration

The instrumentation key is specified in configuration. The following code sample shows how to specify an instrumentation key in `appsettings.json.`

```json
{
  "ApplicationInsights:InstrumentationKey":"#{ApplicationInsightsInstrumentationKey}#"
}
```

## Sample Usage

```csharp
[TrackProblems]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class DummyController : ControllerBase
{
  [HttpPost]
  [ProducesResponseType(typeof(RequestEntry), 200)]
  [ProducesResponseType(typeof(ErrorModel), 400)]
  public async Task<IActionResult> SendAsync([FromBody] SendRequestModel model)
  {
      ...
      // In case of a bad request
      return Problem(title: "error_title", description: "more detailed description", statusCode: 400);
      ...
    }
}
```
