using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tingle.Extensions.Caching.MongoDB.Tests
{
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
            await cache.SetAsync(sessionId, Array.Empty<byte>(), cacheOptions);

            // Verify that container has been created
            Assert.Contains("session", await dbFixture.Database!.ListCollectionNames().ToListAsync());
            var collection = dbFixture.Database.GetCollection<MongoCacheEntry>("session");
            var indexes = await collection.Indexes.List().ToListAsync();
            Assert.Single(indexes.Where(b => b["name"] != "_id_"));
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
            byte[] data = new byte[4] { 1, 2, 3, 4 };
            await cache.SetAsync(sessionId, data, cacheOptions);

            // Verify that container has been created

            var collection = dbFixture.GetCollection<MongoCacheEntry>("session");
            var filter = Builders<MongoCacheEntry>.Filter.Eq(e => e.Key, sessionId);
            var storedSession = await collection.Find(filter).SingleAsync();
            Assert.Equal(sessionId, storedSession.Key);
            Assert.Equal(data, storedSession.Content);
        }

        [Fact]
        public async Task GetSessionData()
        {
            using var dbFixture = new MongoDbFixture();
            const string sessionId = "sessionId";
            const int ttl = 1400;
            byte[] data = new byte[1] { 1 };

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
            await cache.SetAsync(sessionId, data, cacheOptions);

            Assert.Equal(data, await cache.GetAsync(sessionId));
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
            Assert.Null(await cache.GetAsync(sessionId));
        }
    }
}
