using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Tingle.AspNetCore.Authentication.SharedKey;

/// <summary>
/// Context used for <see cref="SharedKeyEvents.OnForbidden"/>
/// </summary>
public class ForbiddenContext : ResultContext<SharedKeyOptions>
{
    /// <summary>
    /// Creates and instance of <see cref="ForbiddenContext"/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="scheme"></param>
    /// <param name="options"></param>
    public ForbiddenContext(HttpContext context, AuthenticationScheme scheme, SharedKeyOptions options)
        : base(context, scheme, options) { }
}
