namespace Tingle.Extensions.Http.Authentication;

/// <summary>
/// Implementation of <see cref="AuthenticationHandler"/> that sets an authentication value in the query
/// </summary>
public class ApiKeyQueryAuthenticationHandler : AuthenticationHandler
{
    /// <summary>
    /// Creates a new instance of the <see cref="ApiKeyQueryAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="authenticationValue">the value to be supplied for authentication</param>
    /// <param name="queryParameterName">the query parameter name to be used</param>
    public ApiKeyQueryAuthenticationHandler(string authenticationValue, string queryParameterName = "auth")
    {
        if (string.IsNullOrWhiteSpace(AuthenticationValue = authenticationValue))
        {
            throw new ArgumentNullException(nameof(authenticationValue));
        }

        if (string.IsNullOrWhiteSpace(QueryParameterName = queryParameterName))
        {
            throw new ArgumentNullException(nameof(queryParameterName));
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ApiKeyQueryAuthenticationHandler"/> class with a specific inner handler.
    /// </summary>
    /// <param name="authenticationValue">the value to be supplied for authentication</param>
    /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
    /// <param name="queryParameterName">the query parameter name to be used</param>
    public ApiKeyQueryAuthenticationHandler(string authenticationValue, HttpMessageHandler innerHandler, string queryParameterName = "auth")
        : base(innerHandler)
    {
        if (string.IsNullOrWhiteSpace(AuthenticationValue = authenticationValue))
        {
            throw new ArgumentNullException(nameof(authenticationValue));
        }

        if (string.IsNullOrWhiteSpace(QueryParameterName = queryParameterName))
        {
            throw new ArgumentNullException(nameof(queryParameterName));
        }
    }

    /// <summary>
    /// The value to be supplied for authentication
    /// </summary>
    public virtual string AuthenticationValue { get; set; }

    /// <summary>
    /// The query parameter name to be used
    /// </summary>
    public virtual string QueryParameterName { get; set; }

    /// <inheritdoc/>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var builder = new UriBuilder(request.RequestUri!);
        var parts = System.Web.HttpUtility.ParseQueryString(builder.Query);
        parts[QueryParameterName] = AuthenticationValue;
        builder.Query = parts.ToString();
        request.RequestUri = builder.Uri;

        return base.SendAsync(request, cancellationToken);
    }
}
