using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// Extension methods on <see cref="HttpContext"/>
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Get the UserAgent making the request.
    /// This is usually present in the <c>User-Agent</c> header.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> to extract from</param>
    /// <returns></returns>
    public static string? GetUserAgent(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        if (httpContext.Request.Headers.TryGetValue(HeaderNames.UserAgent, out var values))
        {
            return values.FirstOrDefault();
        }

        return null;
    }
}
