using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Tingle.AspNetCore.Authentication.SharedKey;

/// <summary>
/// Context used for <see cref="SharedKeyEvents.OnAuthenticationFailed"/>
/// </summary>
/// <param name="context"></param>
/// <param name="scheme"></param>
/// <param name="options"></param>
public class AuthenticationFailedContext(HttpContext context, AuthenticationScheme scheme, SharedKeyOptions options) : ResultContext<SharedKeyOptions>(context, scheme, options)
{
    /// <summary>
    /// The exception describing the failure
    /// </summary>
    public Exception Exception { get; set; } = default!;
}
