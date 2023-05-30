using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Tingle.AspNetCore.Authentication.PassThrough;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class AuthenticationBuilderExtensions
{
    /// <summary>
    /// Add authentication for PassThrough
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddPassThrough(this AuthenticationBuilder builder)
        => builder.AddPassThrough(PassThroughDefaults.AuthenticationScheme, _ => { });

    /// <summary>
    /// Add authentication for PassThrough
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddPassThrough(this AuthenticationBuilder builder, Action<PassThroughOptions> configureOptions)
        => builder.AddPassThrough(PassThroughDefaults.AuthenticationScheme, configureOptions);

    /// <summary>
    /// Add authentication for PassThrough
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme">the name the scheme is to registered with</param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddPassThrough(this AuthenticationBuilder builder, string authenticationScheme, Action<PassThroughOptions> configureOptions)
        => builder.AddPassThrough(authenticationScheme, displayName: null, configureOptions: configureOptions);

    /// <summary>
    /// Add authentication for PassThrough
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme">the name the scheme is to registered with</param>
    /// <param name="displayName">the name to be used when displaying the scheme</param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddPassThrough(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<PassThroughOptions> configureOptions)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<PassThroughOptions>, PassThroughPostConfigureOptions>());
        return builder.AddScheme<PassThroughOptions, PassThroughHandler>(authenticationScheme, displayName, configureOptions);
    }
}
