using MongoDB.Driver;

namespace Tingle.Extensions.Caching.MongoDB.Tests;

public sealed class MongoDbFixture : IDisposable
{
    public MongoDbFixture()
    {
        var dbName = Guid.NewGuid().ToString("n");
        var mub = new MongoUrlBuilder()
        {
            Server = MongoServerAddress.Parse("localhost:27017"),
            DatabaseName = dbName
        };
        ConnectionString = mub.ToString();
        Client = new MongoClient(ConnectionString);
        Database = Client.GetDatabase(dbName);
    }

    public string ConnectionString { get; private set; }

    public IMongoClient? Client { get; private set; }

    public IMongoDatabase? Database { get; private set; }

    public string? DatabaseName => Database?.DatabaseNamespace.DatabaseName;

    public IMongoCollection<T>? GetCollection<T>(string? name = null) => Database?.GetCollection<T>(name ?? typeof(T).Name);

    #region IDisposable Support
    private bool disposed = false; // To detect redundant calls

    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                Client?.DropDatabase(Database?.DatabaseNamespace.DatabaseName);
            }

            Client = null;
            Database = null;

            disposed = true;
        }
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
    }
    #endregion
}
