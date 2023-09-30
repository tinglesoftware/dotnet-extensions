using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Apple.Models;

/// <summary>
/// Represents a reason why an APNs request failed
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ApnsErrorReason
{
    /// <summary>
    /// The collapse identifier exceeds the maximum allowed size
    /// </summary>
    BadCollapseId,

    /// <summary>
    /// The specified device token was bad. Verify that the request contains
    /// a valid token and that the token matches the environment.
    /// </summary>
    BadDeviceToken,

    /// <summary>
    /// The <c>apns-expiration</c> value is bad.
    /// </summary>
    BadExpirationDate,

    /// <summary>
    /// The <c>apns-id</c> value is bad.
    /// </summary>
    BadMessageId,

    /// <summary>
    /// The <c>apns-priority</c> value is bad.
    /// </summary>
    BadPriority,

    /// <summary>
    /// The <c>apns-topic</c> value is bad.
    /// </summary>
    BadTopic,

    /// <summary>
    /// The device token does not match the specified topic.
    /// </summary>
    DeviceTokenNotForTopic,

    /// <summary>
    /// One or more headers were repeated.
    /// </summary>
    DuplicateHeaders,

    /// <summary>
    /// Idle time out.
    /// </summary>
    IdleTimeout,

    /// <summary>
    /// The device token is not specified in the request <c>:path</c>. Verify
    /// that the <c>:path</c> header contains the device token.
    /// </summary>
    MissingDeviceToken,

    /// <summary>
    /// The <c>apns-topic</c> header of the request was not specified and was
    /// required. The <c>apns-topic</c> header is mandatory when the client
    /// is connected using a certificate that supports multiple topics.
    /// </summary>
    MissingTopic,

    /// <summary>
    /// The message payload was empty.
    /// </summary>
    PayloadEmpty,

    /// <summary>
    /// Pushing to this topic is not allowed.
    /// </summary>
    TopicDisallowed,

    /// <summary>
    /// The certificate was bad.
    /// </summary>
    BadCertificate,

    /// <summary>
    /// The client certificate was for the wrong environment.
    /// </summary>
    BadCertificateEnvironment,

    /// <summary>
    /// The provider token is stale and a new token should be generated.
    /// </summary>
    ExpiredProviderToken,

    /// <summary>
    /// The specified action is not allowed.
    /// </summary>
    Forbidden,

    /// <summary>
    /// The provider token is not valid or the token signature could not be verified.
    /// </summary>
    InvalidProviderToken,

    /// <summary>
    /// No provider certificate was used to connect to APNs and Authorization
    /// header was missing or no provider token was specified.
    /// </summary>
    MissingProviderToken,

    /// <summary>
    /// The request contained a bad <c>:path</c> value.
    /// </summary>
    BadPath,

    /// <summary>
    /// The specified <c>:method</c> was not <c>POST</c>.
    /// </summary>
    MethodNotAllowed,

    /// <summary>
    /// The device token is inactive for the specified topic.
    /// </summary>
    Unregistered,

    /// <summary>
    /// The message payload was too large. See
    /// Creating the Remote Notification Payload
    /// (https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW1)
    /// for details on maximum payload size.
    /// </summary>
    PayloadTooLarge,

    /// <summary>
    /// The provider token is being updated too often.
    /// </summary>
    TooManyProviderTokenUpdates,

    /// <summary>
    /// Too many requests were made consecutively to the same device token.
    /// </summary>
    TooManyRequests,

    /// <summary>
    /// An internal server error occurred.
    /// </summary>
    InternalServerError,

    /// <summary>
    /// The service is unavailable.
    /// </summary>
    ServiceUnavailable,

    /// <summary>
    /// The server is shutting down.
    /// </summary>
    Shutdown,
}
