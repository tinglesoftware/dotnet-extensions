using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.FcmLegacy.Models;

/// <summary>
/// Represents an <see cref="FcmLegacyNotification"/> for Web (i.e. Chrome).
/// </summary>
public class FcmLegacyNotificationWeb : FcmLegacyNotification
{
    /// <summary>
    /// The URL to use for the notification's icon.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// The action associated with a user click on the notification.
    /// For all URL values, HTTPS is required.
    /// </summary>
    [JsonPropertyName("click_action")]
    public string? ClickAction { get; set; }
}
