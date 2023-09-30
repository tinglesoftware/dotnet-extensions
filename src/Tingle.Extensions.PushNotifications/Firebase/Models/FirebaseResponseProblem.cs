using System.Text.Json.Serialization;
using Tingle.Extensions.PushNotifications.FcmLegacy.Models;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

/// <summary>
/// Represents a problem response from Firebase
/// </summary>
public class FirebaseResponseProblem
{
    ///
    [JsonPropertyName("error")]
    public FirebaseResponseError? Error { get; set; }

    [JsonExtensionData]
    internal IDictionary<string, object>? Extensions { get; set; }
}

///
public class FirebaseResponseError
{
    ///
    [JsonPropertyName("code")]
    public int Code { get; set; }

    ///
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    ///
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    ///
    [JsonPropertyName("details")]
    public List<FirebaseResponseErrorDetails>? Details { get; set; } = new();

    [JsonExtensionData]
    internal IDictionary<string, object>? Extensions { get; set; }
}

///
public class FirebaseResponseErrorDetails
{
    ///
    [JsonPropertyName("@type")]
    public string? Type { get; set; }

    ///
    [JsonPropertyName("errorCode")]
    public FirebaseErrorCode ErrorCode { get; set; }

    [JsonExtensionData]
    internal IDictionary<string, object>? Extensions { get; set; }
}
