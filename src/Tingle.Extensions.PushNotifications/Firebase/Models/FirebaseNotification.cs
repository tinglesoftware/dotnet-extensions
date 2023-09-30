using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

///
public class FirebaseNotification
{
    /// <summary>
    /// The notification's title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// The notification's body text.
    /// </summary>
    [JsonPropertyName("body")]
    public string? Body { get; set; }

    /// <summary>
    /// Contains the URL of an image that is going to be downloaded on the device and displayed in a notification.
    /// JPEG, PNG, BMP have full support across platforms.
    /// Animated GIF and video only work on iOS.
    /// WebP and HEIF have varying levels of support across platforms and platform versions.
    /// Android has 1MB image size limit.
    /// Quota usage and implications/costs for hosting image on Firebase Storage: https://firebase.google.com/pricing
    /// </summary>
    [JsonPropertyName("image")]
    public string? Image { get; set; }
}
