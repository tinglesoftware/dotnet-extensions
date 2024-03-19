using Tingle.AspNetCore.Swagger.ReDoc;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>
/// </summary>
public static class ReDocServiceCollectionExtensions
{
    /// <summary>
    /// Configure ReDoc services
    /// </summary>
    /// <param name="services">the services to be added to</param>
    /// <param name="setupAction">The action used to configure the options</param>
    /// <returns></returns>
    public static IServiceCollection AddReDoc(this IServiceCollection services, Action<ReDocOptions>? setupAction = null)
    {
        if (setupAction is not null) services.Configure(setupAction);
        services.ConfigureOptions<ReDocConfigureOptions>();
        return services;
    }
}
