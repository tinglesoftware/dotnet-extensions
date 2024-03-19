using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Tingle.AspNetCore.ApplicationInsights;

internal partial class ExtrasTelemetryInitializer(IHttpContextAccessor httpContextAccessor) : ITelemetryInitializer
{
    private const string HeaderXAppPackageId = "X-App-Package-Id";
    private const string HeaderXAppVersionName = "X-App-Version-Name";
    private const string HeaderXAppVersionCode = "X-App-Version-Code";
    private const string KeyAppClient = "Client";
    private const string KeyAppIpAddress = "IpAddress";

    public void Initialize(ITelemetry telemetry)
    {
        static void AddIfNotExsits(IDictionary<string, string> dictionary, string key, string? value)
        {
            key = GetPrefixFormat().Replace(key, string.Empty).ToLowerInvariant();
            if (!dictionary.ContainsKey(key) && !string.IsNullOrWhiteSpace(value)) dictionary[key] = value;
        }

        static string? GetIpAddress(System.Net.IPAddress? address)
        {
            if (address is null) return null;

            // if the IP is an IPv4 mapped to IPv6, remap it
            var addr = address;
            if (addr.IsIPv4MappedToIPv6)
            {
                addr = addr.MapToIPv4();
            }

            return addr.ToString();
        }

        HttpContext? httpContext;
        if (telemetry is RequestTelemetry request && (httpContext = httpContextAccessor?.HttpContext) != null)
        {
            var headers = httpContext.Request.Headers;

            // populate the package Id
            AddIfNotExsits(request.Properties, HeaderXAppPackageId, headers[HeaderXAppPackageId].FirstOrDefault());

            // populate the version name
            AddIfNotExsits(request.Properties, HeaderXAppVersionName, headers[HeaderXAppVersionName].FirstOrDefault());

            // populate the version code
            AddIfNotExsits(request.Properties, HeaderXAppVersionCode, headers[HeaderXAppVersionCode].FirstOrDefault());

            // populate the client from user-agent
            AddIfNotExsits(request.Properties, KeyAppClient, httpContext.GetUserAgent());

            // populate the IP address
            AddIfNotExsits(request.Properties, KeyAppIpAddress, GetIpAddress(httpContext.Connection?.RemoteIpAddress));
        }
    }

    [GeneratedRegex("^[Xx]-", RegexOptions.Compiled)]
    private static partial Regex GetPrefixFormat();
}
