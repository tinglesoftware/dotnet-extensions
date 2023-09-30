using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Apple.Models;

/// <summary>
/// Represents the response from APNs
/// </summary>
public class ApnsMessageResponse
{
    // For a successful request, the body of the response is empty.

    [JsonExtensionData]
    internal IDictionary<string, object>? Extensions { get; set; }
}
