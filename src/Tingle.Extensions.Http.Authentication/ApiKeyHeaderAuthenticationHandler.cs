namespace Tingle.Extensions.Http.Authentication;

/// <summary>
/// Implementation of <see cref="AuthenticationHandler"/> that sets an authentication value in a header
/// </summary>
public class ApiKeyHeaderAuthenticationHandler : BasicHeaderAuthenticationHandler
{
    /// <summary>
    /// Creates a new instance of the <see cref="ApiKeyHeaderAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="parameter">the value to be supplied for authentication</param>
    public ApiKeyHeaderAuthenticationHandler(string parameter) : base(parameter) { }

    /// <summary>
    /// Creates a new instance of the <see cref="ApiKeyHeaderAuthenticationHandler"/> class with a specific inner handler.
    /// </summary>
    /// <param name="parameter">the value to be supplied for authentication</param>
    /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
    public ApiKeyHeaderAuthenticationHandler(string parameter, HttpMessageHandler innerHandler)
        : base(parameter, innerHandler) { }

    /// <summary>
    /// The scheme to be used in the Authorization header.
    /// This is similar to <see cref="System.Net.Http.Headers.AuthenticationHeaderValue.Scheme"/>
    /// </summary>
    public override string Scheme { get; set; } = "ApiKey";
}
