using System.Net.Http.Headers;
using Tingle.Extensions.Http;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/> relating to <see cref="AbstractHttpApiClient{TOptions}"/>
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>Add a HTTP API client implemented via <see cref="AbstractHttpApiClient{TOptions}"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add to.</param>
    /// <param name="configure">Action to configure options.</param>
    /// <typeparam name="TClient">The client type. The type specified will be registered in the service collection as a transient service.</typeparam>
    /// <typeparam name="TOptions">The options type.</typeparam>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddHttpApiClient<TClient, TOptions>(this IServiceCollection services,
                                                                         Action<TOptions>? configure = null)
        where TClient : AbstractHttpApiClient<TOptions>
        where TOptions : AbstractHttpApiClientOptions, new()
    {
        // if we have a configuration action, add it
        if (configure != null) services.Configure(configure);

        return services.AddHttpClient<TClient>()
                       .ConfigureHttpClient((provider, client) =>
                       {
                           // populate the User-Agent header
                           var assemblyName = typeof(TClient).GetType().Assembly.GetName();
                           var userAgent = new ProductInfoHeaderValue(productName: assemblyName.Name!,
                                                                      productVersion: assemblyName.Version!.ToString(3));
                           client.DefaultRequestHeaders.UserAgent.Add(userAgent);
                       });
    }

    /// <summary>Add a HTTP API client implemented via <see cref="AbstractHttpApiClient{TOptions}"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add to.</param>
    /// <param name="configure">Action to configure options.</param>
    /// <typeparam name="TClient">The client resolution type.</typeparam>
    /// <typeparam name="TImplementation">The client implementation type. The type specified will be registered in the service collection as a transient service.</typeparam>
    /// <typeparam name="TOptions">The options type.</typeparam>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddHttpApiClient<TClient, TImplementation, TOptions>(this IServiceCollection services,
                                                                                          Action<TOptions>? configure = null)
        where TClient : class
        where TImplementation : AbstractHttpApiClient<TOptions>, TClient
        where TOptions : AbstractHttpApiClientOptions, new()
    {
        services.AddTransient<TClient>(p => p.GetRequiredService<TImplementation>());
        return services.AddHttpApiClient<TImplementation, TOptions>(configure);
    }
}
