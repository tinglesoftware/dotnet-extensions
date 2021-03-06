using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Tingle.Extensions.Caching.MongoDB;

/// <summary>
/// Distributed cache implementation over MongoDB.
/// </summary>
public class MongoCache : IDistributedCache
{
    private readonly SemaphoreSlim connectionLock = new(initialCount: 1, maxCount: 1);
    private readonly MongoCacheOptions options;
    private IMongoClient? client;
    private IMongoCollection<MongoCacheEntry>? collection;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoCache"/> class.
    /// </summary>
    /// <param name="optionsAccessor">Options accessor.</param>
    public MongoCache(IOptions<MongoCacheOptions> optionsAccessor)
    {
        if (optionsAccessor == null)
        {
            throw new ArgumentNullException(nameof(optionsAccessor));
        }

        options = optionsAccessor.Value;

        if (string.IsNullOrWhiteSpace(options.ConnectionString) && options.MongoClient == null)
        {
            throw new InvalidOperationException($"You need to specify either a {nameof(options.ConnectionString)} or an existing {nameof(options.MongoClient)} in the {nameof(MongoCacheOptions)}.");
        }

        // attempt to pull from the database name from the connection string
        if (string.IsNullOrWhiteSpace(options.DatabaseName) && !string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            var url = new MongoUrl(options.ConnectionString);
            options.DatabaseName = url.DatabaseName;
        }

        if (string.IsNullOrWhiteSpace(options.DatabaseName))
        {
            throw new InvalidOperationException($"You need to specify either a database name in the {nameof(options.ConnectionString)} or the {nameof(MongoCacheOptions)}.");
        }

        if (string.IsNullOrWhiteSpace(options.CollectionName))
        {
            throw new InvalidOperationException($"You need to specify either a collection name in the {nameof(options.ConnectionString)} or the {nameof(MongoCacheOptions)}.");
        }
    }

    /// <inheritdoc/>
    public byte[]? Get(string key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        Connect();

        var filter = Builders<MongoCacheEntry>.Filter.Eq(c => c.Key, key);
        var entry = collection.Find(filter).SingleOrDefault();
        if (entry == null) return null;

        // If using sliding expiration then replace item with one that has updated values
        if (entry.IsSlidingExpiration.GetValueOrDefault())
        {
            entry.ExpiresAt = DateTime.UtcNow.AddSeconds(entry.TimeToLive ?? 0);
            collection!.ReplaceOne(filter, entry);
        }

        return entry.Content;
    }

    /// <inheritdoc/>
    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (key == null) throw new ArgumentNullException(nameof(key));

        await ConnectAsync(token).ConfigureAwait(false);

        var filter = Builders<MongoCacheEntry>.Filter.Eq(c => c.Key, key);
        var entry = await collection.Find(filter).SingleOrDefaultAsync(cancellationToken: token).ConfigureAwait(false);
        if (entry == null) return null;

        // If using sliding expiration then replace item with one that has updated values
        if (entry.IsSlidingExpiration.GetValueOrDefault())
        {
            entry.ExpiresAt = DateTime.UtcNow.AddSeconds(entry.TimeToLive ?? 0);
            await collection!.ReplaceOneAsync(filter, entry, cancellationToken: token).ConfigureAwait(false);
        }

        return entry.Content;
    }

    /// <inheritdoc/>
    public void Refresh(string key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        Connect();

        var filter = Builders<MongoCacheEntry>.Filter.Eq(c => c.Key, key);
        var entry = collection.Find(filter).SingleOrDefault();
        var r_opt = new ReplaceOptions { IsUpsert = true };
        collection!.ReplaceOne(filter, entry, r_opt);
    }

    /// <inheritdoc/>
    public async Task RefreshAsync(string key, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (key == null) throw new ArgumentNullException(nameof(key));

        await ConnectAsync(token).ConfigureAwait(false);

        var filter = Builders<MongoCacheEntry>.Filter.Eq(c => c.Key, key);
        var entry = await collection.Find(filter).SingleOrDefaultAsync(cancellationToken: token).ConfigureAwait(false);
        var r_opt = new ReplaceOptions { IsUpsert = true };
        await collection!.ReplaceOneAsync(filter, entry, r_opt, cancellationToken: token).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Remove(string key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        Connect();

        var filter = Builders<MongoCacheEntry>.Filter.Eq(c => c.Key, key);
        collection!.DeleteOne(filter);
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (key == null) throw new ArgumentNullException(nameof(key));

        await ConnectAsync(token).ConfigureAwait(false);

        var filter = Builders<MongoCacheEntry>.Filter.Eq(c => c.Key, key);
        await collection!.DeleteOneAsync(filter, cancellationToken: token).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (options == null) throw new ArgumentNullException(nameof(options));

        Connect();

        var filter = Builders<MongoCacheEntry>.Filter.Eq(c => c.Key, key);
        var item = BuildMongoCacheEntry(key, value, options);
        var r_opt = new ReplaceOptions { IsUpsert = true };
        collection!.ReplaceOne(filter, item, r_opt);
    }

    /// <inheritdoc/>
    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (key == null) throw new ArgumentNullException(nameof(key));
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (options == null) throw new ArgumentNullException(nameof(options));

        await ConnectAsync(token).ConfigureAwait(false);

        var filter = Builders<MongoCacheEntry>.Filter.Eq(c => c.Key, key);
        var item = BuildMongoCacheEntry(key, value, options);
        var r_opt = new ReplaceOptions { IsUpsert = true };
        await collection!.ReplaceOneAsync(filter, item, r_opt, cancellationToken: token).ConfigureAwait(false);
    }

    private static MongoCacheEntry BuildMongoCacheEntry(string key, byte[] content, DistributedCacheEntryOptions options)
    {
        DateTimeOffset creationTime = DateTimeOffset.UtcNow;
        DateTimeOffset? absoluteExpiration = GetAbsoluteExpiration(creationTime, options);
        long? timeToLive = GetExpirationInSeconds(creationTime, absoluteExpiration, options);

        var expiresAt = timeToLive.HasValue ? creationTime.AddSeconds(timeToLive.Value).UtcDateTime : (DateTime?)null;

        return new MongoCacheEntry()
        {
            Key = key,
            Content = content,
            TimeToLive = timeToLive,
            ExpiresAt = expiresAt,
            IsSlidingExpiration = timeToLive.HasValue && options.SlidingExpiration.HasValue,
        };
    }

    private static long? GetExpirationInSeconds(DateTimeOffset creationTime, DateTimeOffset? absoluteExpiration, DistributedCacheEntryOptions options)
    {
        if (absoluteExpiration.HasValue && options.SlidingExpiration.HasValue)
        {
            return (long)Math.Min((absoluteExpiration.Value - creationTime).TotalSeconds,
                                  options.SlidingExpiration.Value.TotalSeconds);
        }
        else if (absoluteExpiration.HasValue)
        {
            return (long)(absoluteExpiration.Value - creationTime).TotalSeconds;
        }
        else if (options.SlidingExpiration.HasValue)
        {
            return (long)options.SlidingExpiration.Value.TotalSeconds;
        }

        return null;
    }

    private static DateTimeOffset? GetAbsoluteExpiration(DateTimeOffset creationTime, DistributedCacheEntryOptions options)
    {
        if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration <= creationTime)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(DistributedCacheEntryOptions.AbsoluteExpiration),
                                                  actualValue: options.AbsoluteExpiration.Value,
                                                  message: "The absolute expiration value must be in the future.");
        }

        var absoluteExpiration = options.AbsoluteExpiration;
        if (options.AbsoluteExpirationRelativeToNow.HasValue)
        {
            absoluteExpiration = creationTime + options.AbsoluteExpirationRelativeToNow;
        }

        return absoluteExpiration;
    }

    private async Task ConnectAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (collection != null) return;

        await connectionLock.WaitAsync(token).ConfigureAwait(false);
        try
        {
            if (collection == null)
            {
                collection = await MongoCollectionInitializeAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            connectionLock.Release();
        }
    }

    private void Connect()
    {
        if (collection != null) return;

        connectionLock.Wait();
        try
        {
            if (collection == null)
            {
                collection = MongoCollectionInitialize();
            }
        }
        finally
        {
            connectionLock.Release();
        }
    }

    private async Task<IMongoCollection<MongoCacheEntry>> MongoCollectionInitializeAsync()
    {
        client = GetClientInstance();

        var database = client.GetDatabase(options.DatabaseName);
        if (options.CreateIfNotExists)
        {
            // if the collection does not exist, create it
            var collectionNames = await database.ListCollectionNames().ToListAsync().ConfigureAwait(false);
            if (!collectionNames.Contains(options.CollectionName, StringComparer.OrdinalIgnoreCase))
            {
                await database.CreateCollectionAsync(options.CollectionName).ConfigureAwait(false);
                var collection = database.GetCollection<MongoCacheEntry>(options.CollectionName);

                var keys = Builders<MongoCacheEntry>.IndexKeys.Ascending(x => x.ExpiresAt);
                var ci_opt = new CreateIndexOptions
                {
                    Name = "DefaultExpireAfterSecondsIndex",
                    ExpireAfter = TimeSpan.Zero, // expire/remove immediately
                };
                var model = new CreateIndexModel<MongoCacheEntry>(keys, ci_opt);
                await collection.Indexes.CreateOneAsync(model).ConfigureAwait(false);
            }
        }

        return database.GetCollection<MongoCacheEntry>(options.CollectionName);
    }

    private IMongoCollection<MongoCacheEntry> MongoCollectionInitialize()
    {
        client = GetClientInstance();

        var database = client.GetDatabase(options.DatabaseName);
        if (options.CreateIfNotExists)
        {
            // if the collection does not exist, create it
            var collectionNames = database.ListCollectionNames().ToList();
            if (!collectionNames.Contains(options.CollectionName, StringComparer.OrdinalIgnoreCase))
            {
                database.CreateCollection(options.CollectionName);
                var collection = database.GetCollection<MongoCacheEntry>(options.CollectionName);

                var keys = Builders<MongoCacheEntry>.IndexKeys.Ascending(x => x.ExpiresAt);
                var ci_opt = new CreateIndexOptions
                {
                    Name = "DefaultExpireAfterSecondsIndex",
                    ExpireAfter = TimeSpan.Zero, // expire/remove immediately
                };
                var model = new CreateIndexModel<MongoCacheEntry>(keys, ci_opt);
                collection.Indexes.CreateOne(model);
            }
        }

        return database.GetCollection<MongoCacheEntry>(options.CollectionName);
    }

    private IMongoClient GetClientInstance()
    {
        if (options.MongoClient != null)
        {
            return options.MongoClient;
        }

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            throw new ArgumentException($"'{nameof(options.ConnectionString)}' cannot be null or whitespace",
                                        nameof(options.ConnectionString));
        }

        return new MongoClient(options.ConnectionString);
    }
}
