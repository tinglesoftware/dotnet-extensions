using System.Net.Http.Headers;

namespace Tingle.Extensions.Http.Authentication;

/// <summary>
/// Implementation of <see cref="AuthenticationHandler"/> which sets the Authorization header using the libraries implementation <see cref="AuthenticationHeaderValue"/>.
/// The <see cref="AuthenticationHeaderValue.Scheme"/> is set using the provider-wide property <see cref="Scheme"/> but the <see cref="AuthenticationHeaderValue.Parameter"/>
/// is set using the abstract method <see cref="GetParameterAsync(HttpRequestMessage, CancellationToken)"/>.
/// </summary>
public abstract class AuthenticationHeaderHandler : AuthenticationHandler
{
    /// <summary>
    /// Creates a new instance of the <see cref="AuthenticationHeaderHandler"/> class.
    /// </summary>
    protected AuthenticationHeaderHandler() { }

    /// <summary>
    /// Creates a new instance of the <see cref="AuthenticationHeaderHandler"/> class with a specific inner handler.
    /// </summary>
    /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
    protected AuthenticationHeaderHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    /// <summary>
    /// The scheme to be used in the Authorization header. This is similar to <see cref="AuthenticationHeaderValue.Scheme"/>
    /// </summary>
    public virtual string Scheme { get; set; } = "Bearer";

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var parameter = await GetParameterAsync(request, cancellationToken).ConfigureAwait(false);
        request.Headers.Authorization = new AuthenticationHeaderValue(Scheme, parameter);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the authentication parameter value. This is similar to <see cref="AuthenticationHeaderValue.Parameter"/>
    /// </summary>
    /// <param name="message">The <see cref="HttpRequestMessage"/> to be authorized.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task<string?> GetParameterAsync(HttpRequestMessage message, CancellationToken cancellationToken);
}
