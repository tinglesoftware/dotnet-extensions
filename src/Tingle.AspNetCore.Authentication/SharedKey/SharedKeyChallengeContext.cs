using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Tingle.AspNetCore.Authentication.SharedKey;

/// <summary>
/// Context used with <see cref="SharedKeyEvents.OnChallenge"/>
/// </summary>
/// <param name="context"></param>
/// <param name="scheme"></param>
/// <param name="options"></param>
/// <param name="properties"></param>
public class SharedKeyChallengeContext(HttpContext context,
                                 AuthenticationScheme scheme,
                                 SharedKeyOptions options,
                                 AuthenticationProperties properties) : PropertiesContext<SharedKeyOptions>(context, scheme, options, properties)
{
    /// <summary>
    /// Any failures encountered during the authentication process.
    /// </summary>
    public Exception? AuthenticateFailure { get; set; }

    /// <summary>
    /// Gets or sets the "error" value returned to the caller as part
    /// of the WWW-Authenticate header. This property may be null when
    /// <see cref="SharedKeyOptions.IncludeErrorDetails"/> is set to <c>false</c>.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the "error_description" value returned to the caller as part
    /// of the WWW-Authenticate header. This property may be null when
    /// <see cref="SharedKeyOptions.IncludeErrorDetails"/> is set to <c>false</c>.
    /// </summary>
    public string? ErrorDescription { get; set; }

    /// <summary>
    /// Gets or sets the "error_uri" value returned to the caller as part of the
    /// WWW-Authenticate header. This property is always null unless explicitly set.
    /// </summary>
    public string? ErrorUri { get; set; }

    /// <summary>
    /// If true, will skip any default logic for this challenge.
    /// </summary>
    public bool Handled { get; private set; }

    /// <summary>
    /// Skips any default logic for this challenge.
    /// </summary>
    public void HandleResponse() => Handled = true;
}
