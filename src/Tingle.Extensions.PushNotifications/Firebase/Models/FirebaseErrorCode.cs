using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.FcmLegacy.Models;

/// <summary>
/// Represents a reason why an FCM request failed.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FirebaseErrorCode
{
    /// <summary>
    /// Internal server error.
    /// </summary>
    INTERNAL,

    /// <summary>
    /// One or more arguments specified in the request were invalid.
    /// </summary>
    INVALID_ARGUMENT,

    /// <summary>
    /// Sending limit exceeded for the message target.
    /// </summary>
    QUOTA_EXCEEDED,

    /// <summary>
    /// The authenticated sender ID is different from the sender ID for the registration token.
    /// </summary>
    SENDER_ID_MISMATCH,

    /// <summary>
    /// APNs certificate or web push auth key was invalid or missing.
    /// </summary>
    THIRD_PARTY_AUTH_ERROR,

    /// <summary>
    /// Cloud Messaging service is temporarily unavailable.
    /// </summary>
    UNAVAILABLE,

    /// <summary>
    /// App instance was unregistered from FCM.
    /// This usually means that the token used is no longer valid and a new one must be used.
    /// </summary>
    UNREGISTERED,
}
