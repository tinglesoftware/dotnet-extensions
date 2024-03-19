using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using SC = Tingle.AspNetCore.ApplicationInsights.InsightsJsonSerializerContext;

namespace Tingle.AspNetCore.ApplicationInsights;

internal class HeadersTelemetryInitializer(IHttpContextAccessor httpContextAccessor) : ITelemetryInitializer
{
    private const string KeyHeaders = "Headers";

    public void Initialize(ITelemetry telemetry)
    {
        HttpContext? httpContext;
        if (telemetry is RequestTelemetry rt && (httpContext = httpContextAccessor?.HttpContext) != null)
        {
            var headers = httpContext.Request.Headers;
            var dict = headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(s => s!).ToArray());
            rt.Properties[KeyHeaders] = System.Text.Json.JsonSerializer.Serialize(dict, SC.Default.IDictionaryStringStringArray);
        }
    }
}
