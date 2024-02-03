using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.FcmLegacy.Models;

/// <summary>
/// Represents a request payload sent to Firebase Cloud Messaging (FCM) using the legacy HTTP API.
/// </summary>
[Obsolete(MessageStrings.FirebaseLegacyObsoleteMessage)]
public class FcmLegacyRequest
{
    /// <summary>
    /// The recipient of a message.
    /// It can be a device's registration token, a device group's notification key,
    /// or a single topic(prefixed with <c>/topics/</c>).
    /// To send to multiple topics, use the <see cref="Condition"/> parameter.
    /// </summary>
    [JsonPropertyName("to")]
    public string? To { get; set; }

    /// <summary>
    /// The registration tokens to be targeted. Can be ignored if <see cref="To"/> is used
    /// The recipient of a multicast message, a message sent to more than one registration token.
    /// The value should be an array of registration tokens to which to send the multicast message.
    /// The array must contain at least 1 and at most 1000 registration tokens. 
    /// To send a message to a single device, use the <see cref="RegistrationIds"/>  parameter.
    /// </summary>
    [JsonPropertyName("registration_ids")]
    public IEnumerable<string>? RegistrationIds { get; set; }

    /// <summary>
    /// Specifies a logical expression of conditions that determine the message target.
    /// <br/>
    /// Supported condition: Topic, formatted as <c>'yourTopic' in topics</c>. This value is case-insensitive.
    /// <br/>
    /// Supported operators: <c>&amp;&amp;</c>, <c>||</c>. Maximum two operators per topic message supported.
    /// </summary>
    [JsonPropertyName("condition")]
    public string? Condition { get; set; }

    /// <summary>
    /// Identifies a group of messages (e.g., with <c>Updates Available</c>) that can be collapsed,
    /// so that only the last message gets sent when delivery can be resumed.
    /// This is intended to avoid sending too many of the same messages when the device comes back online or becomes active.
    /// <br/>
    /// Note that there is no guarantee of the order in which messages get sent.
    /// <br/>
    /// Note: A maximum of 4 different collapse keys are allowed at any given time.
    /// This means an FCM connection server can simultaneously store 4 different messages per client app.
    /// If you exceed this number, there is no guarantee which 4 collapse keys the FCM connection server will keep.
    /// </summary>
    [JsonPropertyName("collapse_key")]
    public string? CollapseKey { get; set; }

    /// <summary>
    /// The priority of the message
    /// By default, notification messages are sent with <see cref="FcmLegacyPriority.High"/> priority,
    /// and data messages are sent with <see cref="FcmLegacyPriority.Normal"/> priority.
    /// <see cref="FcmLegacyPriority.Normal"/> priority optimizes the client app's battery consumption
    /// and should be used unless immediate delivery is required.
    /// For messages with <see cref="FcmLegacyPriority.Normal"/> priority, 
    /// the app may receive the message with unspecified delay.
    /// <br/>
    /// When a message is sent with <see cref="FcmLegacyPriority.High"/> priority, it is sent immediately,
    /// and the app can display a notification.
    /// </summary>
    [JsonPropertyName("priority")]
    public FcmLegacyPriority? Priority { get; set; }

    /// <summary>
    /// On iOS, use this field to represent <c>content-available</c> in the APNs payload.
    /// When a notification or message is sent and this is set to <see langword="true"/>, an inactive client app is awoken,
    /// and the message is sent through APNs as a silent notification and not through the FCM connection server.
    /// <br/>
    /// Note that silent notifications in APNs are not guaranteed to be delivered, and can depend on factors such
    /// as the user turning on Low Power Mode, force quitting the app, etc.
    /// On Android, data messages wake the app by default.
    /// On Chrome, currently not supported.
    /// </summary>
    [JsonPropertyName("content_available")]
    public bool? ContentAvailable { get; set; }

    /// <summary>
    /// Currently for iOS 10+ devices only.
    /// On iOS, use this field to represent <c>mutable-content</c> in the APNs payload.
    /// When a notification is sent and this is set to <see langword="true"/>, the content of the notification
    /// can be modified before it is displayed, using a
    /// <see href="https://developer.apple.com/reference/usernotifications/unnotificationserviceextension">Notification Service app extension</see>.
    /// <br/>
    /// This will be ignored for Android and web.
    /// </summary>
    [JsonPropertyName("mutable_content")]
    public bool? MutableContent { get; set; }

    /// <summary>
    /// Specifies how long (in seconds) the message should be kept in FCM storage if the device is offline.
    /// The maximum time to live supported is 4 weeks, and the default value is 4 weeks. For more information, 
    /// see <see href="https://firebase.google.com/docs/cloud-messaging/concept-options#ttl">Setting the lifespan of a message</see>.
    /// </summary>
    [JsonPropertyName("time_to_live")]
    public long? TtlSeconds { get; set; }

    /// <summary>
    /// Specifies the package name of the application where the registration tokens must match in order to receive the message.
    /// (Android Only)
    /// </summary>
    [JsonPropertyName("restricted_package_name")]
    public string? RestrictedPackageName { get; set; }

    /// <summary>
    /// When set to true, allows developers to test a request without actually sending a message.
    /// Defaults to <see langword="false"/>
    /// </summary>
    [JsonPropertyName("dry_run")]
    public bool? DryRun { get; set; }

    /// <summary>
    /// Specifies the custom key-value pairs of the message's payload.
    /// <br/>
    /// For example, with data:{"score":"3x1"}:
    /// <br/>
    /// On iOS, if the message is sent via APNs, it represents the custom data fields.
    /// If it is sent via FCM connection server, it would be represented as key value
    /// dictionary in <c>AppDelegate application:didReceiveRemoteNotification:</c>.
    /// <br/>
    /// On Android, this would result in an intent extra named score with the string value <c>3x1</c>.
    /// The key should not be a reserved word (<c>from</c>, <c>message_type</c>, or any word starting with <c>google</c> or <c>gcm</c>).
    /// Do not use any of the json property names defined in this class (such as <c>collapse_key</c>).
    /// </summary>
    [JsonPropertyName("data")]
    public IDictionary<string, string>? Data { get; set; }
}

/// <summary>
/// Represents a request payload sent to Firebase Cloud Messaging (FCM) using the legacy HTTP API.
/// </summary>
/// <typeparam name="TNotification">The type for use with the <see cref="Notification"/> property.</typeparam>
[Obsolete(MessageStrings.FirebaseLegacyObsoleteMessage)]
public class FcmLegacyRequest<TNotification> : FcmLegacyRequest where TNotification : FcmLegacyNotification, new() // using the generic type solves a serialization issue with System.Text.Json
{
    /// <summary>
    /// This parameter specifies the predefined, user-visible key-value pairs of the notification payload.
    /// See Notification payload support for detail. For more information about notification message and data message options, see
    /// <see href="https://firebase.google.com/docs/cloud-messaging/concept-options#notifications_and_data_messages">Message types</see>.
    /// If a notification payload is provided, or the <see cref="FcmLegacyRequest.ContentAvailable"/> option is set to <see langword="true"/>
    /// for a message to an iOS device, the message is sent through APNs, otherwise it is sent through the FCM connection server.
    /// </summary>
    [JsonPropertyName("notification")]
    public TNotification? Notification { get; set; }
}

/// <summary>
/// Represents a request payload sent to Firebase Cloud Messaging (FCM) using the legacy HTTP API to Android.
/// </summary>
[Obsolete(MessageStrings.FirebaseLegacyObsoleteMessage)]
public class FcmLegacyRequestAndroid : FcmLegacyRequest<FcmLegacyNotificationAndroid> { }

/// <summary>
/// Represents a request payload sent to Firebase Cloud Messaging (FCM) using the legacy HTTP API to iOS.
/// </summary>
[Obsolete(MessageStrings.FirebaseLegacyObsoleteMessage)]
public class FcmLegacyRequestIos : FcmLegacyRequest<FcmLegacyNotificationIos> { }

/// <summary>
/// Represents a request payload sent to Firebase Cloud Messaging (FCM) using the legacy HTTP API to Web.
/// </summary>
[Obsolete(MessageStrings.FirebaseLegacyObsoleteMessage)]
public class FcmLegacyRequestWeb : FcmLegacyRequest<FcmLegacyNotificationWeb> { }
