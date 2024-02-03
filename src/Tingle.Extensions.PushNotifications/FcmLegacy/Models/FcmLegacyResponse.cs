using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.FcmLegacy.Models;

/// <summary>
/// Represents a response payload received from Firebase Cloud Messaging (FCM) in the legacy HTTP API.
/// </summary>
[Obsolete(MessageStrings.FirebaseLegacyObsoleteMessage)]
public class FcmLegacyResponse
{
    /// <summary>
    /// Unique ID (number) identifying the multicast message.
    /// </summary>
    [JsonPropertyName("multicast_id")]
    public long MulticastId { get; set; }

    /// <summary>
    /// Number of messages that were processed without an error.
    /// </summary>
    [JsonPropertyName("success")]
    public long Success { get; set; }

    /// <summary>
    /// Number of messages that could not be processed.
    /// </summary>
    [JsonPropertyName("failure")]
    public long Failure { get; set; }

    /// <summary>
    /// Array of objects representing the status of the messages processed.
    /// The objects are listed in the same order as the request
    /// (i.e., for each registration ID in the request, its result is listed in the same index in the response).
    /// </summary>
    [JsonPropertyName("results")]
    public IList<FcmLegacyResult>? Results { get; set; }

    /// <summary>
    /// The topic message ID when FCM has successfully received the request and will attempt to deliver to all subscribed devices.
    /// Only populated for responses from topic request.
    /// </summary>
    [JsonPropertyName("message_id")]
    public long? MessageId { get; set; }

    /// <summary>
    /// String specifying the error that occurred when processing the message for the recipient.
    /// Only populated for responses from topic request.
    /// </summary>
    [JsonPropertyName("error")]
    public FcmLegacyErrorCode? Error { get; set; }

    [JsonExtensionData]
    internal IDictionary<string, object>? Extensions { get; set; }
}
