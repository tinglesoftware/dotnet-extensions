using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.FcmLegacy.Models;

/// <summary>
/// Abstractions for an FCM notification using legacy HTTP API.
/// </summary>
public abstract class FcmLegacyNotification
{
    /// <summary>
    /// The notification's title.
    /// This field is not visible on iOS phones and tablets.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// The notification's body text.
    /// </summary>
    [JsonPropertyName("body")]
    public string? Body { get; set; }
}
