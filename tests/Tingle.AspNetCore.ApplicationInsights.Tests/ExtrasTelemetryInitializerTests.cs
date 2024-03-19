using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace Tingle.AspNetCore.ApplicationInsights.Tests;

public class ExtrasTelemetryInitializerTests
{
    [Fact]
    public void Skips_Non_RequestTelemetry()
    {
        var httpContext = new DefaultHttpContext();
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
        var initializer = new ExtrasTelemetryInitializer(httpContextAccessor);
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
        var ip = MakeRandomIPAddress();
        httpContext.Connection.RemoteIpAddress = ip;
        var httpRequest = httpContext.Request;
        httpRequest.Headers[HeaderNames.UserAgent] = "okhttp/3.0.3";
        httpRequest.Headers["X-App-Package-Id"] = "com.tingle.app";
        httpRequest.Headers["X-App-Version-Name"] = "1.0.1";
        httpRequest.Headers["X-App-Version-Code"] = "112";

        // prepare the initializer
        var initializer = new ExtrasTelemetryInitializer(httpContextAccessor);
        var telemetry = new RequestTelemetry();
        Assert.Empty(telemetry.Properties);

        // execute
        initializer.Initialize(telemetry);

        // assert
        var properties = new SortedDictionary<string, string>(telemetry.Properties);
        var actualKeys = properties.Keys.ToList();
        var actualValues = properties.Values.ToList();
        Assert.Equal(["app-package-id", "app-version-code", "app-version-name", "client", "ipaddress"], actualKeys);
        Assert.Equal(["com.tingle.app", "112", "1.0.1", "okhttp/3.0.3", ip.ToString()], actualValues);
    }

    [Fact]
    public void SkipsNonPresentSources()
    {
        var httpContext = new DefaultHttpContext();
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };

        // populate values in the context and the request
        var ip = MakeRandomIPAddress();
        httpContext.Connection.RemoteIpAddress = ip;
        var httpRequest = httpContext.Request;
        httpRequest.Headers[HeaderNames.UserAgent] = "Tingle.Services.Clients/1.0.0";

        // prepare the initializer
        var initializer = new ExtrasTelemetryInitializer(httpContextAccessor);
        var telemetry = new RequestTelemetry();
        Assert.Empty(telemetry.Properties);

        // execute
        initializer.Initialize(telemetry);

        // assert
        var properties = new SortedDictionary<string, string>(telemetry.Properties);
        var actualKeys = properties.Keys.ToList();
        var actualValues = properties.Values.ToList();
        Assert.Equal(["client", "ipaddress"], actualKeys);
        Assert.Equal(["Tingle.Services.Clients/1.0.0", ip.ToString()], actualValues);
    }

    [Fact]
    public void SkipsNullIp()
    {
        var httpContext = new DefaultHttpContext();
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };

        // populate values in the context and the request
        var httpRequest = httpContext.Request;
        httpRequest.Headers[HeaderNames.UserAgent] = "Tingle.Services.Clients/1.0.0";

        // prepare the initializer
        var initializer = new ExtrasTelemetryInitializer(httpContextAccessor);
        var telemetry = new RequestTelemetry();
        Assert.Empty(telemetry.Properties);

        // execute
        initializer.Initialize(telemetry);

        // assert
        var properties = new SortedDictionary<string, string>(telemetry.Properties);
        var actualKeys = properties.Keys.ToList();
        var actualValues = properties.Values.ToList();
        Assert.Equal(["client"], actualKeys);
        Assert.Equal(["Tingle.Services.Clients/1.0.0"], actualValues);
    }

    private static IPAddress MakeRandomIPAddress()
    {
        var rnd = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
        var bytes = Enumerable.Range(0, 4).Select(_ => Convert.ToByte(rnd.Next(1, 254))).ToArray();
        return new IPAddress(bytes);
    }
}
