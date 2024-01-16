using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

/// <summary>Represents a request payload sent to Firebase Cloud Messaging (FCM).</summary>
/// <param name="message">Message to send.</param>
/// <param name="validateOnly">Flag for testing the request without actually delivering the message.</param>
public class FirebaseRequest(FirebaseRequestMessage message, bool? validateOnly = null)
{
    /// <summary>Message to send.</summary>
    [JsonPropertyName("message")]
    public FirebaseRequestMessage Message { get; set; } = message ?? throw new ArgumentNullException(nameof(message));

    /// <summary>Flag for testing the request without actually delivering the message.</summary>
    [JsonPropertyName("validate_only")]
    public bool? ValidateOnly { get; set; } = validateOnly;
}
