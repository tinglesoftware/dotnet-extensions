using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Tingle.AspNetCore.Authentication.PassThrough;

/// <summary>
/// Context used with <see cref="PassThroughEvents"/>
/// </summary>
public class PassThroughContext : ResultContext<PassThroughOptions>
{
    /// <summary>
    /// Creates an instance of <see cref="PassThroughContext"/>
    /// </summary>
    /// <param name="principal">Claims Principal</param>
    /// <param name="properties">Authentication properties</param>
    /// <param name="context">HttpContext</param>
    /// <param name="scheme">Authentication Scheme</param>
    /// <param name="options">options</param>
    public PassThroughContext(ClaimsPrincipal principal,
                              AuthenticationProperties properties,
                              HttpContext context,
                              AuthenticationScheme scheme,
                              PassThroughOptions options)
        : base(context, scheme, options)
    {
        Principal = principal ?? throw new ArgumentNullException(nameof(principal));
        Properties = properties ?? throw new ArgumentNullException(nameof(properties));
    }
}
