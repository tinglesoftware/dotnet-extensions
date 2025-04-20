# Tingle.Extensions.MongoDB

> [!IMPORTANT]
> Use of `MongoDbContext` is no longer recommended. Instead migrate to the official EntityFrameworkCore for MongoDB [here](https://github.com/mongodb/mongo-efcore-provider). Health checks can be ran using [`Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore`](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-9.0#entity-framework-core-dbcontext-probe) in EfCore or [`AspNetCore.HealthChecks.MongoDb`](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)

`MongoDB` is a cross-platform NoSQL database program which uses JSON like documents with schema.

To connect to the database, we use the `MongoClient` class to access a `MongoDB` instance and through it, we can select the database we want to use. With the `MongoClient` instance we can drop a database, get a database, or retrieve names of the database from the server. There is no option for creating the database because once you pick/select a database and insert data into it, it automatically creates the database if it did not exist before.

`MongoClient` has a constructor which can accept a connection string or URL as parameters. If the database name is specified in the connection string, a singleton is added to the services. If the database name is specified in the `MongoClient` constructor parameters, it overrides any database name specified in the connection string.

This library provides extensions for working with `MongoDB`. It includes extension methods for `IServiceCollection` and for performing health checks on `MongoDB`.

## Adding to services collection

If a MongoDB client is created that takes a connection string as a parameter, the `GetConnectionString`() function is called which returns the connection string to the MongoDB.

Also, a MongoDB database contains a collection to store data, To get a reference of the collection, it is required to call the `GetCollection`() function of the MongoDB database. The `GetCollection`() function takes a collection name as a parameter and returns a reference to the collection.

Health checks that monitor the performance and functioning of MongoDB are also added to the services as well.

In `Program.cs` add MongoDB services as shown in the code snippet below:

```csharp
builder.Services.AddMongoDbContext<MyDbContext>(options => options.UseConnectionString(Configuration.GetConnectionString("Mongo")));
```

## Configuration

If MongoDB client is configured with a connection string, add the `ConnectionStrings` configuration to the configuration file such as appsettings.json as in the example below:

```json
{
    "ConnectionStrings:Mongo":"#{ConnectionStringsMongo}#"
}
```

## Serializers

|Source Types|BSON Destination Types|
|--|--|
|`System.Text.Json.Nodes.JsonObject`|`BsonDocument`|
|`System.Text.Json.Nodes.JsonArray`|`BsonArray`|
|`System.Text.Json.JsonElement`|`BsonDocument`, `BsonArray`, or value|
|`System.Net.IPNetwork` (.NET 8 or later)|`String`|
|`Tingle.Extensions.Primitives.Duration`|`String`|
|`Tingle.Extensions.Primitives.Etag`|`Int64`, `Binary` or `String`|
|`Tingle.Extensions.Primitives.SequenceNumber`|`Int64` or `String`|

## Diagnostics

Events for Mongo are produced on an `ActivitySource` named [`MongoDB.Driver.Core.Extensions.DiagnosticSources`](https://github.com/jbogard/MongoDB.Driver.Core.Extensions.DiagnosticSources), automatically when using `MongoDbContext`.

## HealthChecks

```cs
services.AddHealthChecks()
        .AddMongoDbContextCheck<MyContext>();
```

## Database setup

For development and preview environments, databases may need to be created automatically on startup. This can be done using the `MONGO_CREATE_DATABASE` environment variable.

```json
{
  "profiles": {
    "MyApp": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development",
        "MONGO_CREATE_DATABASE": "true"
      }
    }
  }
}
```

In the application setup:

```cs
builder.Services.AddMongoDatabaseSetup<MyContext>(); // remember to override EnsureCreatedAsync(...) as per sample
```

## Extensions

A number of extensions for building indexes or performing operations on collections exist.

See extensions for building ascending/descending indexes [here](https://github.com/tinglesoftware/dotnet-extensions/blob/365b5b13cbb242d039a4ac61a5b9fb341580b04a/src/Tingle.Extensions.MongoDB/Extensions/BuildersExtensions.cs)

See extensions for bulk operations [here](https://github.com/tinglesoftware/dotnet-extensions/blob/365b5b13cbb242d039a4ac61a5b9fb341580b04a/src/Tingle.Extensions.MongoDB/Extensions/IMongoCollectionExtensions.cs)
