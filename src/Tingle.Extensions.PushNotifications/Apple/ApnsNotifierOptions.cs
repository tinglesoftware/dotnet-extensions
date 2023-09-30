using Tingle.Extensions.Http;
using Tingle.Extensions.PushNotifications.Apple;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Configuration options for <see cref="ApnsNotifier"/>.
/// </summary>
public class ApnsNotifierOptions : AbstractHttpApiClientOptions
{
    /// <summary>
    /// Gets or sets a delegate to get the raw bytes of the private
    /// key which is passed in the value of <see cref="KeyId"/>.
    /// </summary>
    /// <remarks>The private key should be in PKCS #8 (.p8) format.</remarks>
    public virtual Func<string, Task<byte[]>>? PrivateKeyBytes { get; set; }

    /// <summary>
    /// Gets or sets the ID for your Apple Push Notifications private key.
    /// </summary>
    public virtual string? KeyId { get; set; }

    /// <summary>
    /// Gets or sets the Team ID for your Apple Developer account.
    /// </summary>
    public virtual string? TeamId { get; set; }

    /// <summary>
    /// Gets or sets the bundle ID for your app (iOS, watchOS, tvOS iPadOS, etc).
    /// </summary>
    public virtual string? BundleId { get; set; }
}
