# Tingle.Extensions.Caching.MongoDB

This folder contains an implementation of `IDistributedCache` using MongoDB that can also be leveraged in ASP.NET Core as a Session State Provider.
This implementation is highly inspired by [CosmosCache](https://github.com/Azure/Microsoft.Extensions.Caching.Cosmos).

## MongoClient initialization

The implementation provides two distinct options:

### Use an existing instance of a MongoClient

This option will make the provider re-use an existing `MongoClient` instance, which won't be disposed when the provider is disposed.

```c-sharp
services.AddMongoCache((MongoCacheOptions cacheOptions) =>
{
    cacheOptions.CollectionName = Configuration["MongoCacheCollection"];
    cacheOptions.DatabaseName = Configuration["MongoCacheDatabase"];
    cacheOptions.MongoClient = existingMongoClient;
    cacheOptions.CreateIfNotExists = true;
});
```

### Use a defined connection string

This option will make the provider maintain an internal instance of `MongoClient` that will get disposed when the provider is disposed. The `MongoClient` will be created using the provided `ConnectionString`.

```c-sharp
services.AddMongoCache((MongoCacheOptions cacheOptions) =>
{
    cacheOptions.CollectionName = Configuration["MongoCacheCollection"];
    cacheOptions.DatabaseName = Configuration["MongoCacheDatabase"];
    cacheOptions.ConnectionString = Configuration["MongoConnectionString"];
    cacheOptions.CreateIfNotExists = true;
});
```

### State storage

The provider stores the state in a collection within a database, both parameters are required within the `MongoCacheOptions` initialization. An optional parameter, `CreateIfNotExists` will make sure to create the collection if it does not exist with an optimized configuration for key-value storage.
