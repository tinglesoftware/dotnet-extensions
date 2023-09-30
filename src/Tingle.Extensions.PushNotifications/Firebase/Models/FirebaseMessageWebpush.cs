using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

///
public class FirebaseMessageWebpush
{
    /// <summary>
    /// HTTP headers defined in webpush protocol.
    /// Refer to <see href="https://tools.ietf.org/html/rfc8030#section-5">Webpush protocol</see> for supported headers,
    /// e.g. "TTL": "15".
    /// </summary>
    [JsonPropertyName("headers")]
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Arbitrary key/value payload. If present, it will override <see cref="FirebaseRequestMessage.Data"/>
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<string, string>? Data { get; set; }

    /// <summary>
    /// Web Notification options.
    /// Supports Notification instance properties as defined in
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/API/Notification">Web Notification API</see>.
    /// If present, "title" and "body" fields override <see cref="FirebaseNotification.Title"/> and <see cref="FirebaseNotification.Body"/>.
    /// </summary>
    [JsonPropertyName("notification")]
    public JsonObject? Notification { get; set; }

    /// <summary>
    /// Options for features provided by the FCM SDK for Web.
    /// </summary>
    [JsonPropertyName("fcm_options")]
    public FirebaseMessageWebpushFcmOptions? FcmOptions { get; set; }
}
