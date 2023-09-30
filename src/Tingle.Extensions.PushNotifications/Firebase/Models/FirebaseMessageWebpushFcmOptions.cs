using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

///
public class FirebaseMessageWebpushFcmOptions
{
    /// <summary>
    /// The link to open when the user clicks on the notification. For all URL values, HTTPS is required.
    /// </summary>
    [JsonPropertyName("link")]
    public string? Link { get; set; }

    /// <summary>
    /// Label associated with the message's analytics data.
    /// </summary>
    [JsonPropertyName("analytics_label")]
    public string? AnalyticsLabel { get; set; }
}
