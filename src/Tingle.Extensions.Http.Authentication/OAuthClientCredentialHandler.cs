using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Http.Authentication;

/// <summary>
/// Authentication provider for OAuth Client Credentials flow (see the OAuth 2.0 spec for more details).
/// </summary>
public class OAuthClientCredentialHandler : CachingAuthenticationHeaderHandler
{
    private readonly HttpClient backChannel;

    /// <summary>
    /// Creates a new instance of the <see cref="OAuthClientCredentialHandler"/> class.
    /// </summary>
    /// <param name="backChannel">The <see cref="HttpClient"/> for making OAuth requests</param>
    public OAuthClientCredentialHandler(HttpClient? backChannel = null)
    {
        this.backChannel = backChannel ?? new HttpClient();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="OAuthClientCredentialHandler"/> class with a specific inner handler.
    /// </summary>
    /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
    /// <param name="backChannel">The <see cref="HttpClient"/> for making OAuth requests</param>
    public OAuthClientCredentialHandler(HttpMessageHandler innerHandler, HttpClient? backChannel = null) : base(innerHandler)
    {
        this.backChannel = backChannel ?? new HttpClient();
    }

    /// <summary>
    /// The authentication endpoint to be used to request a token
    /// </summary>
    public virtual string? AuthenticationEndpoint { get; set; }

    /// <summary>
    /// The client identifier (client_id) to be used in the token request
    /// </summary>
    public virtual string? ClientId { get; set; }

    /// <summary>
    /// The client secret (client_secret) to be used in the token request
    /// </summary>
    public virtual string? ClientSecret { get; set; }

    /// <summary>
    /// The resource to be requested for in the token request
    /// </summary>
    public virtual string? Resource { get; set; }

    /// <summary>
    /// The duration of before expiry of the token that it should be renewed.
    /// Defaults to 10 seconds; minumum 5 seconds.
    /// </summary>
    public virtual TimeSpan RenewalThreshold { get; set; } = TimeSpan.FromSeconds(5);

    /// <inheritdoc/>
    protected override async Task<string?> GetParameterAsync(HttpRequestMessage request,
                                                             CancellationToken cancellationToken)
    {
        // get the token from cache
        var token = await GetTokenFromCacheAsync(cancellationToken).ConfigureAwait(false);

        // request the token is necessary
        if (string.IsNullOrWhiteSpace(token))
        {
            // request the token
            var response = await RequestOAuthTokenAsync(request, backChannel, cancellationToken).ConfigureAwait(false);
            // extract the token
            token = response!.AccessToken!;

            DateTimeOffset? expires = null;

            if (response.ExpiresOn is not null)
            {
                var expires_on_long = response.ExpiresOn.Value;
                expires = DateTimeOffset.FromUnixTimeSeconds(expires_on_long);
            }
            else if (response.ExpiresIn is not null)
            {
                var expires_in_int = response.ExpiresIn.Value;
                expires = DateTimeOffset.UtcNow.AddSeconds(expires_in_int);
            }

            // save the token to the cache and set expiry in-line with the token life time
            // bring the expiry time 5 seconds earlier to allow time for renewal
            if (expires is not null)
            {
                var ex = CalculateCacheEntryExpiry(expires.Value);
                await SetTokenInCacheAsync(token, ex, cancellationToken).ConfigureAwait(false);
            }
        }

        return token;
    }

    /// <summary>
    /// Requests an OAuth token
    /// </summary>
    /// <param name="message">the request message to be authorized</param>
    /// <param name="backChannel">the HTTP client to be used to make any necessary requests</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task<OAuthTokenResponse?> RequestOAuthTokenAsync(HttpRequestMessage message,
                                                                       HttpClient backChannel,
                                                                       CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string?>
        {
            ["client_id"] = ClientId,
            ["client_secret"] = ClientSecret,
            ["resource"] = Resource,
            ["grant_type"] = "client_credentials"
        };

        return RequestOAuthTokenAsync(parameters, backChannel, cancellationToken);
    }

    /// <summary>
    /// Requests an OAuth token
    /// </summary>
    /// <param name="parameters">the paramters for the OAauth request</param>
    /// <param name="backChannel">the HTTP client to be used to make any necessary requests</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<OAuthTokenResponse?> RequestOAuthTokenAsync(IReadOnlyDictionary<string, string?> parameters,
                                                                             HttpClient backChannel,
                                                                             CancellationToken cancellationToken)
    {
        Logger?.LogDebug("Requesting OAuth token");

        // prepare the request
        var request = new HttpRequestMessage(HttpMethod.Post, AuthenticationEndpoint)
        {
            Content = new FormUrlEncodedContent(parameters)
        };

        // make the request
        using var response = await backChannel.SendAsync(request, cancellationToken).ConfigureAwait(false);
#if NET5_0_OR_GREATER
        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
        Logger?.LogTrace("OAuth token response:\n\n{Response}\n\n{Body}", response.ToString(), body);
        response.EnsureSuccessStatusCode(); // ensure it succeeded
        return JsonSerializer.Deserialize(body, CustomJsonSerializerContext.Default.OAuthTokenResponse);
    }

    internal DateTimeOffset CalculateCacheEntryExpiry(DateTimeOffset expiresOn)
    {
        var threshold = Math.Abs(RenewalThreshold.Ticks);
        threshold = Math.Max(TimeSpan.FromSeconds(5).Ticks, threshold); // minimum of 5 seconds
        return expiresOn - TimeSpan.FromTicks(threshold);
    }

    /// <summary>
    /// Represents a response from an OAuth request
    /// </summary>
    protected internal class OAuthTokenResponse
    {
        /// <summary>
        /// The access token
        /// </summary>
        [JsonPropertyName("access_token")]
        public virtual string? AccessToken { get; set; }

        /// <summary>
        /// The time the token expires (written as time since Epoch)
        /// </summary>
        [JsonPropertyName("expires_on")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public virtual long? ExpiresOn { get; set; }

        /// <summary>
        /// The duration of the token from now in seconds
        /// </summary>
        [JsonPropertyName("expires_in")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public virtual int? ExpiresIn { get; set; }
    }
}
