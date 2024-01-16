using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Tingle.AspNetCore.Authentication.SharedKey;

/// <summary>
/// Context used for <see cref="SharedKeyEvents.OnForbidden"/>
/// </summary>
/// <param name="context"></param>
/// <param name="scheme"></param>
/// <param name="options"></param>
public class ForbiddenContext(HttpContext context, AuthenticationScheme scheme, SharedKeyOptions options)
    : ResultContext<SharedKeyOptions>(context, scheme, options) { }
