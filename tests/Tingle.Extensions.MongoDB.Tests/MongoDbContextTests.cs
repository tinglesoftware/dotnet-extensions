using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Tingle.Extensions.MongoDB.Tests;

public class MongoDbContextTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public void CanBeResolvedViaDI()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddXUnit(outputHelper));

        services.AddMongoDbContext<SampleDbContext>(options =>
        {
            var dbName = Guid.NewGuid().ToString().Replace("_", "")[..10];
            var connectionString = $"mongodb://localhost:27017/{dbName}";
            options.UseMongoConnectionString(connectionString);
        });

        var sp = services.BuildServiceProvider(validateScopes: true);

        // resolving directly
        using var scope = sp.CreateScope();
        var provider = scope.ServiceProvider;

        var context = provider.GetRequiredService<SampleDbContext>();
        Assert.NotNull(context.Persons);
    }

    public class Person
    {
        [BsonId]
        public string? Id { get; set; }

        public string? Name { get; set; }
    }

    public class SampleDbContext(MongoDbContextOptions<MongoDbContextTests.SampleDbContext> options) : MongoDbContext(options)
    {
        public IMongoCollection<Person> Persons => Collection<Person>("Persons");

        protected internal override void OnConfiguring(MongoDbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
