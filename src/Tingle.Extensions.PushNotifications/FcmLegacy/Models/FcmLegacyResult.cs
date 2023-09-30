using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.FcmLegacy.Models;

/// <summary>
/// Represents the result of each sent message using the legacy HTTP API.
/// </summary>
public class FcmLegacyResult
{
    /// <summary>
    /// String specifying a unique ID for each successfully processed message.
    /// </summary>
    [JsonPropertyName("message_id")]
    public string? MessageId { get; set; }

    /// <summary>
    /// String specifying the error that occurred when processing the message for the recipient.
    /// </summary>
    [JsonPropertyName("error")]
    public FcmLegacyErrorCode? Error { get; set; }

    [JsonExtensionData]
    internal IDictionary<string, object>? Extensions { get; set; }
}
