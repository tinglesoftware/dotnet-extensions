using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.FcmLegacy.Models;

/// <summary>
/// Represents a reason why an FCM request failed in the legacy HTTP API.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<FcmLegacyErrorCode>))]
public enum FcmLegacyErrorCode
{
    /// <summary>
    /// Check that the request contains a registration token.
    /// Specified via <see cref="FcmLegacyRequest.To"/> or <see cref="FcmLegacyRequest.RegistrationIds"/> field.
    /// </summary>
    MissingRegistration,

    /// <summary>
    /// Check the format of the registration token you pass to the server.
    /// Make sure it matches the registration token the client app receives from registering with Firebase Notifications.
    /// Do not truncate or add additional characters.
    /// </summary>
    InvalidRegistration,

    /// <summary>
    /// An existing registration token may cease to be valid in a number of scenarios, including:
    /// <list type="bullet">
    /// <item>If the client app unregisters with FCM.</item>
    /// <item>
    /// If the client app is automatically unregistered, which can happen if the user uninstalls the application.
    /// For example, on iOS, if the APNs Feedback Service reported the APNs token as invalid.
    /// </item>
    /// <item>
    /// If the registration token expires (for example, Google might decide to refresh registration tokens, or the APNs token has expired for iOS devices).
    /// </item>
    /// <item>
    /// If the client app is updated but the new version is not configured to receive messages.
    /// </item>
    /// </list>
    /// <br/>
    /// For all these cases, remove this registration token from the app server and stop using it to send messages.
    /// </summary>
    NotRegistered,

    /// <summary>
    /// Make sure the message was addressed to a registration token whose package name matches the value passed in the request.
    /// </summary>
    InvalidPackageName,

    /// <summary>
    /// A registration token is tied to a certain group of senders.
    /// When a client app registers for FCM, it must specify which senders are allowed to send messages.
    /// You should use one of those sender IDs when sending messages to the client app.
    /// If you switch to a different sender, the existing registration tokens won't work.
    /// </summary>
    MismatchSenderId,

    /// <summary>
    /// Check that the total size of the payload data included in a message does not exceed
    /// FCM limits: 4096 bytes for most messages, or 2048 bytes in the case of messages to topics.
    /// This includes both the keys and the values.
    /// </summary>
    MessageTooBig,

    /// <summary>
    /// Check that the payload <see cref="FcmLegacyRequest.Data"/> does not contain a key
    /// (such as <c>from</c>, or <c>gcm</c>, or any value prefixed by <c>google</c>) that is used internally by FCM.
    /// Note that some words (such as <c>collapse_key</c>) are also used by FCM but are allowed in the payload,
    /// in which case the payload value will be overridden by the FCM value.
    /// </summary>
    InvalidDataKey,

    /// <summary>
    /// Check that the value used in <see cref="FcmLegacyRequest.TtlSeconds"/> is an integer representing
    /// a duration in seconds between 0 and 2,419,200 (4 weeks).
    /// </summary>
    InvalidTtl,

    /// <summary>
    /// The server couldn't process the request in time. Retry the same request, but you must:
    /// <list type="bullet">
    /// <item>Honor the <c>Retry-After</c> header if it is included in the response from the FCM Connection Server.</item>
    /// <item>
    /// </item>
    /// Implement exponential back-off in your retry mechanism.
    /// (e.g. if you waited one second before the first retry, wait at least two second before the next one, then 4 seconds and so on).
    /// If you're sending multiple messages, delay each one independently by an additional random amount to
    /// avoid issuing a new request for all messages at the same time.
    /// Senders that cause problems risk being blacklisted.
    /// </list>
    /// </summary>
    Unavailable,

    /// <summary>
    /// The server encountered an error while trying to process the request.
    /// You could retry the same request following the requirements listed in <see cref="Unavailable"/>.
    /// If the error persists, please contact <see href="https://firebase.google.com/support/">Firebase support</see>.
    /// </summary>
    InternalServerError,

    /// <summary>
    /// The rate of messages to a particular device is too high.
    /// If an iOS app sends messages at a rate exceeding APNs limits, it may receive this error message.
    /// <br/>
    /// Reduce the number of messages sent to this device and use
    /// <see href="https://en.wikipedia.org/wiki/Exponential_backoff">exponential</see> back-off to retry sending.
    /// </summary>
    DeviceMessageRateExceeded,

    /// <summary>
    /// The rate of messages to subscribers to a particular topic is too high.
    /// Reduce the number of messages sent for this topic and use
    /// <see href="https://en.wikipedia.org/wiki/Exponential_backoff">exponential</see> back-off to retry sending.
    /// </summary>
    TopicsMessageRateExceeded,

    /// <summary>
    /// A message targeted to an iOS device could not be sent because the required APNs authentication key
    /// was not uploaded or has expired.
    /// Check the validity of your development and production credentials.
    /// </summary>
    InvalidApnsCredential,
}
