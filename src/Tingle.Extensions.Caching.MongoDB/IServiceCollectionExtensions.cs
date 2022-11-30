using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Tingle.Extensions.Caching.MongoDB;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up Mongo distributed cache related services in an <see cref="IServiceCollection" />.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds MongoDB distributed caching services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="setupAction">
    /// An <see cref="Action{MongoCacheOptions}"/> to configure the provided <see cref="MongoCacheOptions"/>.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMongoCache(this IServiceCollection services, Action<MongoCacheOptions> setupAction)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

        services.AddOptions();
        services.Configure(setupAction);
        services.Add(ServiceDescriptor.Singleton<IDistributedCache, MongoCache>((IServiceProvider provider) =>
        {
            var optionsMonitor = provider.GetService<IOptionsMonitor<MongoCacheOptions>>();
            if (optionsMonitor != null)
            {
                return new MongoCache(optionsMonitor);
            }

            var options = provider.GetRequiredService<IOptions<MongoCacheOptions>>();
            return new MongoCache(options);
        }));

        return services;
    }

    /// <summary>
    /// Adds MongoDB distributed caching services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">
    /// The connection string for Mongo
    /// This value is set in <see cref="MongoCacheOptions.ConnectionString"/>.
    /// </param>
    /// <param name="databaseName">
    /// The database name to store the cache.
    /// This value is set in <see cref="MongoCacheOptions.DatabaseName"/>.
    /// When <see langword="null"/>, the value is extracted from the <paramref name="connectionString"/>.
    /// </param>
    /// <param name="collectionName">
    /// The collection name to store the cache.
    /// This value is set in <see cref="MongoCacheOptions.CollectionName"/>.
    /// Defaults to <c>Cache</c>
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMongoCache(this IServiceCollection services,
                                                   string? connectionString,
                                                   string? databaseName = null,
                                                   string collectionName = "Cache")
    {
        // nulls are checked in the constructor
        return AddMongoCache(services, options =>
        {
            options.ConnectionString = connectionString;
            options.DatabaseName = databaseName;
            options.CollectionName = collectionName;
        });
    }
}
