using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

/// <summary>Represents a request payload sent to Firebase Cloud Messaging (FCM).</summary>
public class FirebaseRequest
{
    /// <summary>Creates an instance of <see cref="FirebaseRequest"/>.</summary>
    /// <param name="message">Message to send.</param>
    /// <param name="validateOnly">Flag for testing the request without actually delivering the message.</param>
    public FirebaseRequest(FirebaseRequestMessage message, bool? validateOnly = null)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        ValidateOnly = validateOnly;
    }

    /// <summary>Message to send.</summary>
    [JsonPropertyName("message")]
    public FirebaseRequestMessage Message { get; set; }

    /// <summary>Flag for testing the request without actually delivering the message.</summary>
    [JsonPropertyName("validate_only")]
    public bool? ValidateOnly { get; set; }
}
