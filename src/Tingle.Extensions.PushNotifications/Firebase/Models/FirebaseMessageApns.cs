using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

///
public class FirebaseMessageApns
{
    /// <summary>
    /// HTTP request headers defined in Apple Push Notification Service.
    /// Refer to <see href="https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/sending_notification_requests_to_apns">APNs request headers</see>
    /// for supported headers such as <c>apns-expiration</c> and <c>apns-priority</c>.
    /// <br/>
    /// The backend sets a default value for <c>apns-expiration</c> of 30 days and a default value for <c>apns-priority</c> of 10 if not explicitly set.
    /// </summary>
    [JsonPropertyName("headers")]
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// APNs payload, including both <c>aps</c> dictionary and custom payload.
    /// <see href="https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/generating_a_remote_notification">Payload Key Reference</see>.
    /// If present, "title" and "body" fields override <see cref="FirebaseNotification.Title"/> and <see cref="FirebaseNotification.Body"/>.
    /// </summary>
    [JsonPropertyName("payload")]
    public JsonObject? Payload { get; set; }

    /// <summary>
    /// Options for features provided by the FCM SDK for iOS.
    /// </summary>
    [JsonPropertyName("fcm_options")]
    public FirebaseMessageApnsFcmOptions? FcmOptions { get; set; }
}
