using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Tingle.Extensions.Http.Authentication;

namespace Tingle.Extensions.PushNotifications.Apple;

/// <summary>
/// Implementation of <see cref=" AuthenticationHandler"/> for <see cref="ApnsNotifier"/>.
/// </summary>
internal class ApnsAuthenticationHandler : CachingAuthenticationHeaderHandler
{
    private readonly ApnsNotifierOptions options;

    public ApnsAuthenticationHandler(IMemoryCache cache, IOptionsSnapshot<ApnsNotifierOptions> optionsAccessor, ILogger<ApnsAuthenticationHandler> logger)
    {
        Scheme = "bearer";
        Cache = new(cache);
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
    }

    /// <inheritdoc/>
    public override string CacheKey => $"apns:tokens:{options.TeamId}:{options.KeyId}";

    /// <inheritdoc/>
    protected override async Task<string?> GetParameterAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // get the token from cache
        var token = await GetTokenFromCacheAsync(cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(token))
        {
            var keyId = options.KeyId!;

            // prepare header
            var header = new ApnsAuthHeader("ES256", keyId);
            var header_json = System.Text.Json.JsonSerializer.Serialize(header, PushNotificationsJsonSerializerContext.Default.ApnsAuthHeader);
            var headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(header_json));

            // prepare payload
            var payload = new ApnsAuthPayload(options.TeamId, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            var payload_json = System.Text.Json.JsonSerializer.Serialize(payload, PushNotificationsJsonSerializerContext.Default.ApnsAuthPayload);
            var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload_json));

            // import key, https://stackoverflow.com/a/44008229
            var privateKey = await options.PrivateKeyBytes!(keyId).ConfigureAwait(false);
            using var ecdsa = ECDsa.Create();
            ecdsa.ImportPkcs8PrivateKey(privateKey, out _);

            // sign data
            var unsignedJwtData = $"{headerBase64}.{payloadBase64}";
            var signature = ecdsa.SignData(Encoding.UTF8.GetBytes(unsignedJwtData), HashAlgorithmName.SHA256);
            token = $"{unsignedJwtData}.{Convert.ToBase64String(signature)}";

            // according to apple docs, the token should be refreshed no more than once every 20 minutes and
            // no less than once every 60 minutes.
            // https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/establishing_a_token-based_connection_to_apns
            var expires = DateTimeOffset.UtcNow.AddMinutes(60);

            // save the token to the cache and set expiry in-line with the token life time
            // bring the expiry time 5 seconds earlier to allow time for renewal
            expires -= TimeSpan.FromSeconds(5);
            await SetTokenInCacheAsync(token, expires, cancellationToken).ConfigureAwait(false);
        }

        return token;
    }

    internal record ApnsAuthHeader([property: JsonPropertyName("alg")] string? Algorithm,
                                   [property: JsonPropertyName("kid")] string? KeyId);
    internal record ApnsAuthPayload([property: JsonPropertyName("iss")] string? TeamId,
                                    [property: JsonPropertyName("iat")] long IssuedAtSeconds);
}
