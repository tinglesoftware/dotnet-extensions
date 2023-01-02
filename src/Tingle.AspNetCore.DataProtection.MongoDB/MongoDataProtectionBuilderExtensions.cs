using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Tingle.AspNetCore.DataProtection.MongoDB;

namespace Microsoft.AspNetCore.DataProtection;

/// <summary>
/// Contains Mongo-specific extension methods for modifying a
/// <see cref="IDataProtectionBuilder"/>.
/// </summary>
public static class MongoDataProtectionBuilderExtensions
{
    /// <summary>
    /// Configures the data protection system to persist keys to the specified key in a Mongo database
    /// </summary>
    /// <param name="builder">The builder instance to modify.</param>
    /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
    public static IDataProtectionBuilder PersistKeysToMongo(this IDataProtectionBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return PersistKeysToMongo(builder, (services) =>
        {
            var collection = services.GetService<IMongoCollection<DataProtectionKey>>();
            if (collection == null)
            {
                var database = services.GetService<IMongoDatabase>();
                if (database == null)
                {
                    var client = services.GetRequiredService<IMongoClient>();
                    database = client.GetDatabase("DataProtection");
                }
                collection = database.GetCollection<DataProtectionKey>("DataProtection-Keys");
            }
            return collection;
        });
    }

    /// <summary>
    /// Configures the data protection system to persist keys to the specified key in a Mongo database
    /// </summary>
    /// <param name="builder">The builder instance to modify.</param>
    /// <param name="client">The <see cref="IMongoClient"/> to use.</param>
    /// <param name="databaseName">The name of the database to use</param>
    /// <param name="collectionName">The name of the collection to store the keys</param>
    /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
    public static IDataProtectionBuilder PersistKeysToMongo(this IDataProtectionBuilder builder, IMongoClient client, string databaseName, string collectionName)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        return PersistKeysToMongo(builder, () => client.GetDatabase(databaseName).GetCollection<DataProtectionKey>(collectionName));
    }

    /// <summary>
    /// Configures the data protection system to persist keys to the specified key in a Mongo database
    /// </summary>
    /// <param name="builder">The builder instance to modify.</param>
    /// <param name="database">The <see cref="IMongoDatabase"/> to use.</param>
    /// <param name="collectionName">The name of the collection to store the keys</param>
    /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
    public static IDataProtectionBuilder PersistKeysToMongo(this IDataProtectionBuilder builder, IMongoDatabase database, string collectionName)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (database == null)
        {
            throw new ArgumentNullException(nameof(database));
        }

        return PersistKeysToMongo(builder, () => database.GetCollection<DataProtectionKey>(collectionName));
    }

    /// <summary>
    /// Configures the data protection system to persist keys to specified key in a Mongo database
    /// </summary>
    /// <param name="builder">The builder instance to modify.</param>
    /// <param name="collectionFactory">The delegate used to create <see cref="IMongoCollection{TDocument}"/> instances.</param>
    /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
    public static IDataProtectionBuilder PersistKeysToMongo(this IDataProtectionBuilder builder, Func<IServiceProvider, IMongoCollection<DataProtectionKey>> collectionFactory)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (collectionFactory == null)
        {
            throw new ArgumentNullException(nameof(collectionFactory));
        }

        builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(services =>
        {
            return new ConfigureOptions<KeyManagementOptions>(options =>
            {
                options.XmlRepository = new MongoXmlRepository(() => collectionFactory(services));
            });
        });
        return builder;
    }

    /// <summary>
    /// Configures the data protection system to persist keys to specified key in a Mongo database
    /// </summary>
    /// <param name="builder">The builder instance to modify.</param>
    /// <param name="collectionFactory">The delegate used to create <see cref="IMongoCollection{TDocument}"/> instances.</param>
    /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
    public static IDataProtectionBuilder PersistKeysToMongo(this IDataProtectionBuilder builder, Func<IMongoCollection<DataProtectionKey>> collectionFactory)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (collectionFactory == null)
        {
            throw new ArgumentNullException(nameof(collectionFactory));
        }

        builder.Services.Configure<KeyManagementOptions>(options =>
        {
            options.XmlRepository = new MongoXmlRepository(collectionFactory);
        });
        return builder;
    }
}
