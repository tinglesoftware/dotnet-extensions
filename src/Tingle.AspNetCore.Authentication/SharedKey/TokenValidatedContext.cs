using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Tingle.AspNetCore.Authentication.SharedKey.Validation;

namespace Tingle.AspNetCore.Authentication.SharedKey;

/// <summary>
/// Context used to call <see cref="SharedKeyEvents.OnTokenValidated"/>
/// </summary>
/// <param name="context"></param>
/// <param name="scheme"></param>
/// <param name="options"></param>
public class TokenValidatedContext(HttpContext context, AuthenticationScheme scheme, SharedKeyOptions options) : ResultContext<SharedKeyOptions>(context, scheme, options)
{
    /// <summary>
    /// The validation response
    /// </summary>
    public SharedKeyValidatedToken ValidationResponse { get; set; } = default!;
}
