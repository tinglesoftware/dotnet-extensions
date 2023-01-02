using Microsoft.AspNetCore.Authorization;
using Tingle.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/> related to authorization
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds an instance of <see cref="IAuthorizationHandler"/> as a singleton
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <param name="services"></param>
    /// <param name="lifetime">The lifetime of the service.</param>
    /// <returns></returns>
    public static IServiceCollection AddAuthorizationHandler<THandler>(this IServiceCollection services,
                                                                       ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where THandler : class, IAuthorizationHandler
    {
        var sd = ServiceDescriptor.Describe(serviceType: typeof(IAuthorizationHandler),
                                            implementationType: typeof(THandler),
                                            lifetime: lifetime);
        services.Add(sd);

        return services;
    }

    /// <summary>
    /// Adds <see cref="IAuthorizationHandler"/> that send handles instances of <see cref="ApprovedIPNetworkRequirement"/>.
    /// Ensure that the necessary Authorization and framework services are added to the same collection
    /// using services.AddAuthorization(...) and services.AddHttpContextAccessor(...)
    /// </summary>
    /// <param name="services">the services to add to</param>
    /// <returns></returns>
    public static IServiceCollection AddApprovedNetworksHandler(this IServiceCollection services)
    {
        // the handler must be scoped because it injects and the IHttpContextAccessor
        return services.AddAuthorizationHandler<ApprovedIPNetworkHandler>(ServiceLifetime.Scoped);
    }
}
