using Microsoft.AspNetCore.Authentication;
using Tingle.AspNetCore.Authentication.SharedKey.Validation;

namespace Tingle.AspNetCore.Authentication.SharedKey;

/// <summary>
/// Authentication options for <see cref="SharedKeyHandler"/>
/// </summary>
public class SharedKeyOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// Gets or sets the challenge to put in the "WWW-Authenticate" header.
    /// </summary>
    public string Challenge { get; set; } = SharedKeyDefaults.AuthenticationScheme;

    /// <summary>
    /// The object provided by the application to process events raised by the shared key authentication handler.
    /// The application may implement the interface fully, or it may create an instance of <see cref="SharedKeyEvents"/>
    /// and assign delegates only to the events it wants to process.
    /// </summary>
    public new SharedKeyEvents Events
    {
        get { return (SharedKeyEvents)base.Events!; }
        set { base.Events = value; }
    }

    /// <summary>
    /// The scheme for the authorization header e.g. Bearer, SharedKey
    /// </summary>
    public string HeaderValuePrefix { get; set; } = SharedKeyDefaults.HeaderPrefix;

    /// <summary>
    /// Gets the ordered list of <see cref="ISharedKeyTokenValidator"/> used to validate access tokens.
    /// </summary>
    public IList<ISharedKeyTokenValidator> TokenValidators { get; } = new List<ISharedKeyTokenValidator> { new SharedKeyTokenHandler() };

    /// <summary>
    /// Gets or sets the parameters used to validate tokens.
    /// </summary>
    /// <remarks>Contains the types and definitions required for validating a token.</remarks>
    /// <exception cref="ArgumentNullException">if 'value' is null.</exception>
    public SharedKeyTokenValidationParameters ValidationParameters { get; set; } = new SharedKeyTokenValidationParameters();

    /// <summary>
    /// Defines whether the bearer token should be stored in the
    /// <see cref="AuthenticationProperties"/> after a successful authorization.
    /// Defaults to <see langword="true"/>
    /// </summary>
    public bool SaveToken { get; set; } = true;

    /// <summary>
    /// Defines whether the token validation errors should be returned to the caller.
    /// Enabled by default, this option can be disabled to prevent the JWT handler
    /// from returning an error and an error_description in the WWW-Authenticate header.
    /// </summary>
    public bool IncludeErrorDetails { get; set; } = true;
}
