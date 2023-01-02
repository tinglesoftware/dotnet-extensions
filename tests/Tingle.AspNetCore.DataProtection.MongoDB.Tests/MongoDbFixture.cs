using MongoDB.Driver;

namespace Tingle.AspNetCore.DataProtection.MongoDB.Tests;

public sealed class MongoDbFixture : IDisposable
{
    private readonly string dbName;
    private readonly IMongoClient client;

    public MongoDbFixture()
    {
        dbName = Guid.NewGuid().ToString();
        var mub = new MongoUrlBuilder()
        {
            Server = MongoServerAddress.Parse("localhost:27017"),
            DatabaseName = dbName
        };
        ConnectionString = mub.ToString();
        client = new MongoClient(ConnectionString);
    }

    public string ConnectionString { get; private set; }

    public IMongoClient Client => client;

    public IMongoDatabase Database => client.GetDatabase(dbName);

    public IMongoCollection<T> GetCollection<T>(string? name = null) => Database.GetCollection<T>(name ?? typeof(T).Name);

    public string DatabaseName => dbName;

    #region IDisposable Support
    private bool disposed = false; // To detect redundant calls

    private void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                client.DropDatabase(dbName);
            }

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
