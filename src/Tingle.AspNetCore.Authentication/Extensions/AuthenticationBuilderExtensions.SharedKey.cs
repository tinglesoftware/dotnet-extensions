using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Tingle.AspNetCore.Authentication.SharedKey;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions on <see cref="AuthenticationBuilder"/> for SharedKey and PassThrough
/// </summary>
public static partial class AuthenticationBuilderExtensions
{
    /// <summary>
    /// Add authentication for shared key
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddSharedKey(this AuthenticationBuilder builder)
        => builder.AddSharedKey(SharedKeyDefaults.AuthenticationScheme, _ => { });

    /// <summary>
    /// Add authentication for shared key
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddSharedKey(this AuthenticationBuilder builder, Action<SharedKeyOptions> configureOptions)
        => builder.AddSharedKey(SharedKeyDefaults.AuthenticationScheme, configureOptions);

    /// <summary>
    /// Add authentication for shared key
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme">the name the scheme is to registered with</param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddSharedKey(this AuthenticationBuilder builder, string authenticationScheme, Action<SharedKeyOptions> configureOptions)
        => builder.AddSharedKey(authenticationScheme, displayName: null, configureOptions: configureOptions);

    /// <summary>
    /// Add authentication for shared key
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="authenticationScheme">the name the scheme is to registered with</param>
    /// <param name="displayName">the name to be used when displaying the scheme</param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddSharedKey(this AuthenticationBuilder builder, string authenticationScheme, string? displayName, Action<SharedKeyOptions> configureOptions)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<SharedKeyOptions>, SharedKeyPostConfigureOptions>());
        return builder.AddScheme<SharedKeyOptions, SharedKeyHandler>(authenticationScheme, displayName, configureOptions);
    }
}
