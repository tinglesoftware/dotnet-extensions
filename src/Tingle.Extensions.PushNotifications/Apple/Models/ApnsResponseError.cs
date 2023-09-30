using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Apple.Models;

/// <summary>
/// Represents an error response from APNs
/// </summary>
public class ApnsResponseError
{
    /// <summary>
    /// The error indicating the reason for the failure.
    /// </summary>
    [JsonPropertyName("reason")]
    public ApnsErrorReason Reason { get; set; }

    /// <summary>
    /// If the value in the <c>:status</c> header is <c>410</c>, the value
    /// is the last time at which APNs confirmed that the device token was
    /// no longer valid for the topic.
    /// 
    /// Stop pushing notifications until the device registers a token with
    /// a later timestamp with your provider.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public object? Timestamp { get; set; }

    [JsonExtensionData]
    internal IDictionary<string, object>? Extensions { get; set; }
}
