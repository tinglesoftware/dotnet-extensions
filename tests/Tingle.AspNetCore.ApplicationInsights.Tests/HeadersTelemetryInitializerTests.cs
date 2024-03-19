using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Tingle.AspNetCore.ApplicationInsights.Tests;

public class HeadersTelemetryInitializerTests
{
    [Fact]
    public void Skips_Non_RequestTelemetry()
    {
        var httpContext = new DefaultHttpContext();
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var initializer = new HeadersTelemetryInitializer(httpContextAccessor);
        var telemetry = new TraceTelemetry();
        Assert.Empty(telemetry.Properties);
        initializer.Initialize(telemetry);
        Assert.Empty(telemetry.Properties); // remains unchanged
    }

    [Fact]
    public void AddsAllProperties()
    {
        var httpContext = new DefaultHttpContext();
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };

        // populate values in the context and the request
        var httpRequest = httpContext.Request;
        httpRequest.Headers[HeaderNames.ContentType] = "application/json";
        httpRequest.Headers[HeaderNames.ETag] = "AAAAAAAAAAA=";
        httpRequest.Headers["X-Workspace-Id"] = "112";

        // prepare the initializer
        var initializer = new HeadersTelemetryInitializer(httpContextAccessor);
        var telemetry = new RequestTelemetry();
        Assert.Empty(telemetry.Properties);

        // execute
        initializer.Initialize(telemetry);

        // assert
        var expected = new Dictionary<string, string[]>
        {
            ["Content-Type"] = ["application/json"],
            ["ETag"] = ["AAAAAAAAAAA="],
            ["X-Workspace-Id"] = ["112"],
        };
        var properties = new SortedDictionary<string, string>(telemetry.Properties);
        var kvp = Assert.Single(properties);
        Assert.Equal("Headers", kvp.Key);
        Assert.NotNull(kvp.Value);
        var actual = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string[]>>(kvp.Value);
        Assert.Equal(expected, actual);
    }
}
