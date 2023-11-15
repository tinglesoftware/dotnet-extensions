using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Tingle.AspNetCore.Authentication.PassThrough;

/// <summary>
/// PassThrough authentication handler 
/// </summary>
public class PassThroughHandler : AuthenticationHandler<PassThroughOptions>
{
#if NET8_0_OR_GREATER

    /// <summary>
    /// Create an instance of <see cref="PassThroughHandler"/>
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    /// <param name="encoder"></param>
    public PassThroughHandler(IOptionsMonitor<PassThroughOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) { }

#else

    /// <summary>
    /// Create an instance of <see cref="PassThroughHandler"/>
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    /// <param name="encoder"></param>
    /// <param name="clock"></param>
    [Obsolete("ISystemClock is obsolete, use TimeProvider on AuthenticationSchemeOptions instead.")]
    public PassThroughHandler(IOptionsMonitor<PassThroughOptions> options,
                              ILoggerFactory logger,
                              UrlEncoder encoder,
                              ISystemClock clock) : base(options, logger, encoder, clock) { }

#endif



    /// <summary>
    /// The handler calls methods on the events which give the application control at certain points where processing is occurring. 
    /// If it is not provided a default instance is supplied which does nothing when the methods are called.
    /// </summary>
    protected new PassThroughEvents Events
    {
        get { return (PassThroughEvents)base.Events!; }
        set { base.Events = value; }
    }

    /// <summary>
    /// Do actual authentication work
    /// </summary>
    /// <returns></returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(Options.ClaimsIssuer);
        var properties = new AuthenticationProperties();

        var principal = new ClaimsPrincipal(identity);
        var context = new PassThroughContext(principal: principal,
                                             properties: properties,
                                             context: Context,
                                             scheme: Scheme,
                                             options: Options);

        var ticket = new AuthenticationTicket(principal: principal,
                                              properties: context.Properties,
                                              authenticationScheme: Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
