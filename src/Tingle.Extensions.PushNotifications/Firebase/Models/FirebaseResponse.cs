using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

/// <summary>
/// Represents a response payload received from Firebase Cloud Messaging (FCM).
/// </summary>
public class FirebaseResponse
{
    /// <summary>
    /// The identifier of the message sent, in the format of <c>projects/*/messages/{message_id}</c>.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonExtensionData]
    internal IDictionary<string, object>? Extensions { get; set; }
}
