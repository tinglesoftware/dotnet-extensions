namespace Tingle.Extensions.Http.Authentication;

/// <summary>
/// An implementation of <see cref="DelegatingHandler"/> that handles authentication for outgoing requests.
/// </summary>
public abstract class AuthenticationHandler : DelegatingHandler
{
    /// <summary>
    /// Creates a new instance of the <see cref="AuthenticationHandler"/> class.
    /// </summary>
    protected AuthenticationHandler() { }

    /// <summary>
    /// Creates a new instance of the <see cref="AuthenticationHandler"/> class with a specific inner handler.
    /// </summary>
    /// <param name="innerHandler"></param>
    protected AuthenticationHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }
}
