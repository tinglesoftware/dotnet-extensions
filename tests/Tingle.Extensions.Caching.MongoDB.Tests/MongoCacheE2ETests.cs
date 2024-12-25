using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Tingle.Extensions.Caching.MongoDB.Tests;

public class MongoCacheE2ETests
{
    [Fact]
    public async Task InitializeCollectionIfNotExists()
    {
        using var dbFixture = new MongoDbFixture();
        const string sessionId = "sessionId";
        const int ttl = 1400;

        IOptions<MongoCacheOptions> options = Options.Create(new MongoCacheOptions()
        {
            CollectionName = "session",
            DatabaseName = dbFixture.DatabaseName,
            MongoClient = dbFixture.Client,
        });

        var cache = new MongoCache(options);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(ttl)
        };
        await cache.SetAsync(sessionId, [], cacheOptions, TestContext.Current.CancellationToken);

        // Verify that collection has been created
        Assert.Contains("session", await dbFixture.Database!.ListCollectionNames(cancellationToken: TestContext.Current.CancellationToken)
                                                            .ToListAsync(TestContext.Current.CancellationToken));
        var collection = dbFixture.Database.GetCollection<MongoCacheEntry>("session");
        var indexes = await collection.Indexes.List(cancellationToken: TestContext.Current.CancellationToken)
                                              .ToListAsync(TestContext.Current.CancellationToken);
        Assert.Single(indexes, b => b["name"] != "_id_");
    }

    [Fact]
    public async Task UsesExistingCollection()
    {
        using var dbFixture = new MongoDbFixture();
        const string sessionId = "sessionId";
        const int ttl = 1400;

        // ensure there are no collections
        Assert.Empty(await dbFixture.Database!.ListCollectionNames(cancellationToken: TestContext.Current.CancellationToken)
                                              .ToListAsync(TestContext.Current.CancellationToken));

        // create own collection (different index name means the one we create here is used)
        var collection = dbFixture.GetCollection<MongoCacheEntry>("Cache")!;
        var model = new CreateIndexModel<MongoCacheEntry>(
            Builders<MongoCacheEntry>.IndexKeys.Ascending(x => x.ExpiresAt),
            new CreateIndexOptions<MongoCacheEntry> { Name = "NotDefaultName", ExpireAfter = TimeSpan.Zero, });
        await collection.Indexes.CreateOneAsync(model, cancellationToken: TestContext.Current.CancellationToken);

        var cache = new MongoCache(Options.Create(new MongoCacheOptions()
        {
            DatabaseName = dbFixture.DatabaseName,
            MongoClient = dbFixture.Client,
            CreateIfNotExists = false, // collection has already been created
        }));
        Assert.Null(await cache.GetAsync(sessionId, TestContext.Current.CancellationToken));
        var cacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(ttl)
        };
        await cache.SetAsync(sessionId, [], cacheOptions, TestContext.Current.CancellationToken);

        // Verify that collection was not created
        Assert.Equal("Cache", Assert.Single(
            await dbFixture.Database!.ListCollectionNames(cancellationToken: TestContext.Current.CancellationToken)
                                     .ToListAsync(TestContext.Current.CancellationToken)));
        var indexes = await collection.Indexes.List(cancellationToken: TestContext.Current.CancellationToken)
                                              .ToListAsync(TestContext.Current.CancellationToken);
        var ix = Assert.Single(indexes, b => b["name"] != "_id_");
        Assert.Equal(model.Options.Name, ix["name"]);
    }

    [Fact]
    public async Task StoreEntryData()
    {
        using var dbFixture = new MongoDbFixture();
        const string sessionId = "sessionId";
        const int ttl = 1400;

        IOptions<MongoCacheOptions> options = Options.Create(new MongoCacheOptions()
        {
            CollectionName = "session",
            DatabaseName = dbFixture.DatabaseName,
            MongoClient = dbFixture.Client,
        });

        var cache = new MongoCache(options);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(ttl)
        };
        byte[] data = [1, 2, 3, 4];
        await cache.SetAsync(sessionId, data, cacheOptions, TestContext.Current.CancellationToken);

        // Verify that collection has been created
        var collection = dbFixture.GetCollection<MongoCacheEntry>("session");
        var filter = Builders<MongoCacheEntry>.Filter.Eq(e => e.Key, sessionId);
        var storedSession = await collection.Find(filter).SingleAsync(TestContext.Current.CancellationToken);
        Assert.Equal(sessionId, storedSession.Key);
        Assert.Equal(data, storedSession.Content);
    }

    [Fact]
    public async Task GetSessionData()
    {
        using var dbFixture = new MongoDbFixture();
        const string sessionId = "sessionId";
        const int ttl = 1400;
        byte[] data = [1];

        IOptions<MongoCacheOptions> options = Options.Create(new MongoCacheOptions()
        {
            CollectionName = "session",
            DatabaseName = dbFixture.DatabaseName,
            MongoClient = dbFixture.Client,
        });

        var cache = new MongoCache(options);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(ttl)
        };
        await cache.SetAsync(sessionId, data, cacheOptions, TestContext.Current.CancellationToken);

        Assert.Equal(data, await cache.GetAsync(sessionId, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetSessionData_WhenNotExists()
    {
        using var dbFixture = new MongoDbFixture();
        const string sessionId = "sessionId";

        IOptions<MongoCacheOptions> options = Options.Create(new MongoCacheOptions()
        {
            CollectionName = "session",
            DatabaseName = dbFixture.DatabaseName,
            MongoClient = dbFixture.Client,
        });

        var cache = new MongoCache(options);
        Assert.Null(await cache.GetAsync(sessionId, TestContext.Current.CancellationToken));
    }
}
