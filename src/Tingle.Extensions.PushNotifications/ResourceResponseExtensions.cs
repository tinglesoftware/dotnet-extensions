using Tingle.Extensions.Http;

namespace Tingle.Extensions.PushNotifications;

/// <summary>Extensions for <see cref="ResourceResponse{TResource, TProblem}"/>.</summary>
public static class ResourceResponseExtensions
{
    /// <summary>Get the APNs request Id.</summary>
    /// <param name="headers">The <see cref="ResourceResponseHeaders"/> instance.</param>
    /// <returns>Value for the <c>apns-id</c> header.</returns>
    public static string? GetApnsId(this ResourceResponseHeaders headers)
    {
        if (headers is null) throw new ArgumentNullException(nameof(headers));
        return headers.TryGetValue("apns-id", out var value) ? value.Single() : null;
    }
}
