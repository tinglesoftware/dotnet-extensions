using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Tingle.Extensions.Http.Authentication;

namespace Tingle.Extensions.PushNotifications.Firebase;

/// <summary>
/// Implementation of <see cref=" AuthenticationHandler"/> for <see cref="FirebaseNotifier"/>.
/// </summary>
internal class FirebaseAuthenticationHandler : OAuthClientCredentialHandler
{
    private const string Scope = "https://www.googleapis.com/auth/firebase.messaging";

    private readonly FirebaseNotifierOptions options;

    /// <summary>Creates an instance of <see cref="FirebaseAuthenticationHandler"/>.</summary>
    /// <param name="cache">The <see cref="IMemoryCache"/> for storing generated tokens for their lifetime.</param>
    /// <param name="optionsAccessor">The options accessor for <see cref="FirebaseNotifierOptions"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/> to use.</param>
    public FirebaseAuthenticationHandler(IMemoryCache cache, IOptionsSnapshot<FirebaseNotifierOptions> optionsAccessor, ILogger<FirebaseAuthenticationHandler> logger)
        : this(cache, optionsAccessor, logger, null) { }

    /// <summary>Creates an instance of <see cref="FirebaseAuthenticationHandler"/>.</summary>
    /// <param name="cache">The <see cref="IMemoryCache"/> for storing generated tokens for their lifetime.</param>
    /// <param name="optionsAccessor">The options accessor for <see cref="FirebaseNotifierOptions"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/> to use.</param>
    /// <param name="backChannel">An optional <see cref="HttpClient"/> to use when making authentication requests.</param>
    internal FirebaseAuthenticationHandler(IMemoryCache cache, IOptionsSnapshot<FirebaseNotifierOptions> optionsAccessor, ILogger<FirebaseAuthenticationHandler> logger, HttpClient? backChannel)
        : base(backChannel)
    {
        Scheme = "Bearer";
        Cache = new(cache);
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
        AuthenticationEndpoint = options.TokenUri;
    }

    /// <inheritdoc/>
    public override string CacheKey => $"firebase:tokens:{options.ProjectId}";

    /// <inheritdoc/>
    protected override Task<OAuthTokenResponse?> RequestOAuthTokenAsync(HttpRequestMessage message, HttpClient backChannel, CancellationToken cancellationToken)
    {
        var assertion = GenerateAssertion(clientEmail: options.ClientEmail!,
                                          tokenUri: options.TokenUri!,
                                          privateKey: options.PrivateKey!,
                                          issued: DateTimeOffset.UtcNow);

        // make OAuth2 request for a token
        var parameters = new Dictionary<string, string?>
        {
            ["assertion"] = assertion,
            ["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer",
        };

        return RequestOAuthTokenAsync(parameters, backChannel, cancellationToken);
    }

    private static string GenerateAssertion(string clientEmail, string tokenUri, string privateKey, DateTimeOffset issued)
    {
        // prepare header
        var header = new FirebaseAuthHeader("RS256", "JWT");
        var header_json = System.Text.Json.JsonSerializer.Serialize(header, PushNotificationsJsonSerializerContext.Default.FirebaseAuthHeader);
        var headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(header_json));

        // prepare payload
        var expires = issued.AddHours(1);
        var payload = new FirebaseAuthPayload(Issuer: clientEmail,
                                              Scope: Scope,
                                              Audience: tokenUri,
                                              IssuedAtSeconds: issued.ToUnixTimeSeconds(),
                                              ExpiresAtSeconds: expires.ToUnixTimeSeconds());
        var payload_json = System.Text.Json.JsonSerializer.Serialize(payload, PushNotificationsJsonSerializerContext.Default.FirebaseAuthPayload);
        var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload_json));

        // import key, https://stackoverflow.com/a/72661119
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKey.AsSpan());

        // sign data
        var unsignedJwtData = $"{headerBase64}.{payloadBase64}";
        var signature = rsa.SignData(Encoding.UTF8.GetBytes(unsignedJwtData), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return $"{unsignedJwtData}.{Convert.ToBase64String(signature)}";
    }

    internal record FirebaseAuthHeader([property: JsonPropertyName("alg")] string? Algorithm,
                                       [property: JsonPropertyName("typ")] string? Type);
    internal record FirebaseAuthPayload([property: JsonPropertyName("iss")] string? Issuer,
                                        [property: JsonPropertyName("scope")] string? Scope,
                                        [property: JsonPropertyName("aud")] string? Audience,
                                        [property: JsonPropertyName("iat")] long IssuedAtSeconds,
                                        [property: JsonPropertyName("exp")] long ExpiresAtSeconds);
}
