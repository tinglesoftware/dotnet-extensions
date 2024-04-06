using Microsoft.AspNetCore.Authentication;
using Tingle.AspNetCore.Authentication.PassThrough;

namespace Tingle.AspNetCore.Authentication.Tests;

public class PassThroughTests : SharedAuthenticationTests<PassThroughOptions>
{
    protected override string DefaultScheme => PassThroughDefaults.AuthenticationScheme;
    protected override Type HandlerType => typeof(PassThroughHandler);
    protected override bool SupportsSignIn { get => false; }
    protected override bool SupportsSignOut { get => false; }

    protected override void RegisterAuth(AuthenticationBuilder services, Action<PassThroughOptions> configure)
    {
        services.AddPassThrough(o =>
        {
            ConfigureDefaults(o);
            configure.Invoke(o);
        });
    }

    private void ConfigureDefaults(PassThroughOptions o)
    {
    }

}
