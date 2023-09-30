using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Apple.Models;

/// <summary>
/// Represents the data actually sent to the device.
/// If you need to send more information, inherit from this class.
/// </summary>
public class ApnsMessageData
{
    /// <summary>
    /// The payload for the push as specified by Apple
    /// </summary>
    [JsonPropertyName("aps")]
    public ApnsMessagePayload Aps { get; set; } = new ApnsMessagePayload { };
}
