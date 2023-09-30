using Tingle.Extensions.Http;
using Tingle.Extensions.PushNotifications.Firebase;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Configuration options for <see cref="FirebaseNotifier"/>
/// </summary>
public class FirebaseNotifierOptions : AbstractHttpApiClientOptions
{
    /// <summary>
    /// Gets or sets the Firebase project identifier.
    /// </summary>
    public virtual string? ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the client email for your Firebase Service Account.
    /// </summary>
    public virtual string? ClientEmail { get; set; }

    /// <summary>
    /// Gets or sets the endpoint for requesting OAuth2 tokens.
    /// </summary>
    public virtual string? TokenUri { get; set; }

    /// <summary>
    /// Gets or sets the private key for the Firebase Service Account.
    /// </summary>
    public virtual string? PrivateKey { get; set; }
}
