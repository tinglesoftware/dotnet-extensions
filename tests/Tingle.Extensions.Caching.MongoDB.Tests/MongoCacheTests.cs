using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Tingle.Extensions.Caching.MongoDB.Tests;

public class MongoCacheTests
{
    [Fact]
    public void RequiredParameters()
    {
        // Null-check
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => new MongoCache((IOptions<MongoCacheOptions>)null!));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        IOptions<MongoCacheOptions> options = Options.Create(new MongoCacheOptions() { });
        // Database
        Assert.Throws<InvalidOperationException>(() => new MongoCache(options));
        options.Value.DatabaseName = "something";
        // Container
        Assert.Throws<InvalidOperationException>(() => new MongoCache(options));
        options.Value.CollectionName = "something";
        // ConnectionString or MongoClient
        Assert.Throws<InvalidOperationException>(() => new MongoCache(options));

        // Verify that it creates with all parameters
        _ = new MongoCache(Options.Create(new MongoCacheOptions()
        {
            DatabaseName = "something",
            CollectionName = "something",
            ConnectionString = "mongodb://localhost:27017/myapp"
        }));

        _ = new MongoCache(Options.Create(new MongoCacheOptions()
        {
            DatabaseName = "something",
            CollectionName = "something",
            MongoClient = new MongoClient("mongodb://localhost:27017/myapp")
        }));
    }

    // We need a lot more unit tests here
}
