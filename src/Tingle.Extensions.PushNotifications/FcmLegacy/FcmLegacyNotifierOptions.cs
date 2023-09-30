using Tingle.Extensions.Http;
using Tingle.Extensions.PushNotifications.FcmLegacy;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Configuration options for <see cref="FcmLegacyNotifier"/>
/// </summary>
public class FcmLegacyNotifierOptions : AbstractHttpApiClientOptions
{
    /// <summary>
    /// The authentication key for Firebase
    /// </summary>
    public virtual string? Key { get; set; }
}
