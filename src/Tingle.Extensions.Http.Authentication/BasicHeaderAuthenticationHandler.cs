namespace Tingle.Extensions.Http.Authentication;

/// <summary>
/// Implementation of <see cref="AuthenticationHandler"/> that sets an authentication value in a header
/// </summary>
public class BasicHeaderAuthenticationHandler : AuthenticationHeaderHandler
{
    /// <summary>
    /// Creates a new instance of the <see cref="BasicHeaderAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="parameter">the value to be supplied for authentication</param>
    public BasicHeaderAuthenticationHandler(string parameter)
    {
        if (string.IsNullOrWhiteSpace(Parameter = parameter))
        {
            throw new ArgumentNullException(nameof(parameter));
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="BasicHeaderAuthenticationHandler"/> class with a specific inner handler.
    /// </summary>
    /// <param name="parameter">the value to be supplied for authentication</param>
    /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
    public BasicHeaderAuthenticationHandler(string parameter, HttpMessageHandler innerHandler) : base(innerHandler)
    {
        if (string.IsNullOrWhiteSpace(Parameter = parameter))
        {
            throw new ArgumentNullException(nameof(parameter));
        }
    }

    /// <summary>
    /// The value to be supplied for authentication
    /// </summary>
    public virtual string Parameter { get; set; }

    /// <inheritdoc/>
    protected override Task<string?> GetParameterAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult<string?>(Parameter);
    }
}
