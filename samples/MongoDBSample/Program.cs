using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

Console.WriteLine("Hello World!");

var dbName = Guid.NewGuid().ToString().Replace("_", "")[..10];
var connectionString = $"mongodb://localhost:27017/{dbName}";

var services = new ServiceCollection();
services.AddLogging();
services.AddMongoDbContext<SampleDbContext>(options =>
{
    options.UseMongoConnectionString(connectionString);
});

var sp = services.BuildServiceProvider(validateScopes: true);

// resolving directly
using var scope = sp.CreateScope();
var provider = scope.ServiceProvider;

var context = provider.GetRequiredService<SampleDbContext>();
var persons = await context.Persons.AsQueryable().ToListAsync();
Console.WriteLine($"Found {persons.Count} persons");

class Person
{
    [BsonId]
    public string? Id { get; set; }

    public string? Name { get; set; }
}

class SampleDbContext(MongoDbContextOptions<SampleDbContext> options) : MongoDbContext(options)
{
    private const string ColNamePersons = "Persons";

    public IMongoCollection<Person> Persons => Collection<Person>(ColNamePersons);

    protected override void OnConfiguring(MongoDbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected async override Task EnsureCreatedAsync(IMongoDatabase database, CancellationToken cancellationToken = default)
    {
        var names = await database.ListCollectionNames(null, cancellationToken).ToListAsync(cancellationToken);

        if (!names.Contains(ColNamePersons, StringComparer.Ordinal))
        {
            var options = new CreateCollectionOptions<Person> { };
            await database.CreateCollectionAsync(name: ColNamePersons,
                                                 options: options,
                                                 cancellationToken: cancellationToken);

            // create indexes
            await Persons.Indexes.CreateManyAsync(
                models: [
                    // definition for Status
                    new CreateIndexModel<Person>(
                        Builders<Person>.IndexKeys.Ascending(p => p.Name),
                        new CreateIndexOptions<Person> { }),
                ],
                cancellationToken: cancellationToken);
        }

    }
}
