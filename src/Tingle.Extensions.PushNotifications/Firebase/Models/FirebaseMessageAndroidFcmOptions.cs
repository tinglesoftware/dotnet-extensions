using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

///
public class FirebaseMessageAndroidFcmOptions
{
    /// <summary>
    /// Label associated with the message's analytics data.
    /// </summary>
    [JsonPropertyName("analytics_label")]
    public string? AnalyticsLabel { get; set; }
}
