using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tingle.Extensions.Http.Authentication;

namespace Tingle.Extensions.PushNotifications.FcmLegacy;

/// <summary>
/// Implementation of <see cref=" AuthenticationHandler"/> for <see cref="FcmLegacyNotifier"/>.
/// </summary>
internal class FcmLegacyAuthenticationHandler : AuthenticationHandler
{
    private readonly FcmLegacyNotifierOptions options;

    public FcmLegacyAuthenticationHandler(IOptionsSnapshot<FcmLegacyNotifierOptions> optionsAccessor)
    {
        options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
    }

    /// <inheritdoc/>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.TryAddWithoutValidation("Authorization", $"key={options.Key}");
        return base.SendAsync(request, cancellationToken);
    }
}
