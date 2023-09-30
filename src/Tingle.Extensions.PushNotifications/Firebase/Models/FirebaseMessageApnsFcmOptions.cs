using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

///
public class FirebaseMessageApnsFcmOptions
{
    /// <summary>
    /// Label associated with the message's analytics data.
    /// </summary>
    [JsonPropertyName("analytics_label")]
    public string? AnalyticsLabel { get; set; }

    /// <summary>
    /// Contains the URL of an image that is going to be displayed in a notification.
    /// If present, it will override <see cref="FirebaseNotification.Image"/>
    /// </summary>
    [JsonPropertyName("image")]
    public string? Image { get; set; }
}
