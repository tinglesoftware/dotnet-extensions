using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

///
public class FirebaseRequestMessage
{
    /// <summary>
    /// Arbitrary key/value payload, which must be UTF-8 encoded.
    /// The key should not be a reserved word (<c>from</c>, <c>message_type</c>, or any word starting with <c>google</c> or <c>gcm</c>).
    /// When sending payloads containing only data fields to iOS devices, only normal priority ("apns-priority": "5") is allowed in ApnsConfig.
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<string, string>? Data { get; set; }

    /// <summary>
    /// Basic notification template to use across all platforms.
    /// </summary>
    [JsonPropertyName("notification")]
    public FirebaseNotification? Notification { get; set; }

    /// <summary>
    /// Android specific options for messages sent through <see href="https://goo.gl/4GLdUl">FCM connection server</see>.
    /// </summary>
    [JsonPropertyName("android")]
    public FirebaseMessageAndroid? Android { get; set; }

    /// <summary>
    /// <see href="https://tools.ietf.org/html/rfc8030">Webpush</see> options.
    /// </summary>
    [JsonPropertyName("webpush")]
    public FirebaseMessageWebpush? Webpush { get; set; }

    /// <summary>
    /// <see href="https://goo.gl/MXRTPa">Apple Push Notification Service</see> options.
    /// </summary>
    [JsonPropertyName("apns")]
    public FirebaseMessageApns? Apns { get; set; }

    /// <summary>
    /// Template for FCM SDK feature options to use across all platforms.
    /// </summary>
    [JsonPropertyName("fcm_options")]
    public FirebaseMessageFcmOptions? FcmOptions { get; set; }

    /// <summary>
    /// Registration token to send a message to.
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>
    /// Topic name to send a message to, e.g. <c>weather</c>. Note: <c>/topics/</c> prefix should not be provided.
    /// </summary>
    [JsonPropertyName("topic")]
    public string? Topic { get; set; }

    /// <summary>
    /// Condition to send a message to, e.g. <c>'foo' in topics &amp;&amp; 'bar' in topics</c>.
    /// </summary>
    [JsonPropertyName("condition")]
    public string? Condition { get; set; }
}
