using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Apple.Models;

/// <summary>
/// Represents the data actually sent to the device.
/// If you need to send more information, inherit from this type.
/// </summary>
/// <param name="Aps">The payload for the push as specified by Apple </param>
public record ApnsMessageData([property: JsonPropertyName("aps")] ApnsMessagePayload Aps);
