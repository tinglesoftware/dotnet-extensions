using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

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
    public IMongoCollection<Person> Persons => Collection<Person>("Persons");

    protected override void OnConfiguring(MongoDbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}
