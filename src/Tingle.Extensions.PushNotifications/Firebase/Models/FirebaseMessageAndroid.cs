using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

///
public class FirebaseMessageAndroid
{
    /// <summary>
    /// An identifier of a group of messages that can be collapsed,
    /// so that only the last message gets sent when delivery can be resumed.
    /// A maximum of 4 different collapse keys is allowed at any given time.
    /// </summary>
    [JsonPropertyName("collapse_key")]
    public string? CollapseKey { get; set; }

    /// <summary>
    /// Message priority. Can take "normal" and "high" values.
    /// For more information, see <see href="https://goo.gl/GjONJv">Setting the priority of a message</see>.
    /// </summary>
    [JsonPropertyName("priority")]
    public FirebaseMessageAndroidPriority? Priority { get; set; }

    /// <summary>
    /// How long (in seconds) the message should be kept in FCM storage if the device is offline.
    /// The maximum time to live supported is 4 weeks, and the default value is 4 weeks if not set.
    /// Set it to 0 if want to send the message immediately.
    /// In JSON format, the Duration type is encoded as a string rather than an object,
    /// where the string ends in the suffix "s" (indicating seconds) and is preceded by the number of seconds,
    /// with nanoseconds expressed as fractional seconds.
    /// <br/>
    /// For example, 3 seconds with 0 nanoseconds should be encoded in JSON format as "3s",
    /// while 3 seconds and 1 nanosecond should be expressed in JSON format as "3.000000001s".
    /// The ttl will be rounded down to the nearest second.
    /// <br/>
    /// A duration in seconds with up to nine fractional digits, ending with 's'. Example: "3.5s".
    /// </summary>
    [JsonPropertyName("ttl")]
    public string? Ttl { get; set; }

    /// <summary>
    /// Package name of the application where the registration token must match in order to receive the message.
    /// </summary>
    [JsonPropertyName("restricted_package_name")]
    public string? RestrictedPackageName { get; set; }

    /// <summary>
    /// Arbitrary key/value payload. If present, it will override <see cref="FirebaseRequestMessage.Data"/>
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<string, string>? Data { get; set; }

    /// <summary>
    /// Notification to send to android devices.
    /// </summary>
    [JsonPropertyName("notification")]
    public FirebaseMessageAndroidNotification? Notification { get; set; }

    /// <summary>
    /// Options for features provided by the FCM SDK for Android.
    /// </summary>
    [JsonPropertyName("fcm_options")]
    public FirebaseMessageAndroidFcmOptions? FcmOptions { get; set; }

    /// <summary>
    /// If set to true, messages will be allowed to be delivered to the app while the device is in direct boot mode.
    /// See <see href="https://developer.android.com/training/articles/direct-boot">Support Direct Boot mode</see>.
    /// </summary>
    [JsonPropertyName("direct_boot_ok")]
    public string? DirectBootOk { get; set; }
}
