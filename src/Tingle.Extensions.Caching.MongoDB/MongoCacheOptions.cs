using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Tingle.Extensions.Caching.MongoDB;

/// <summary>
/// Options to configure <see cref="MongoCache"/>.
/// </summary>
public class MongoCacheOptions : IOptions<MongoCacheOptions>
{
    /// <summary>
    /// Gets or sets the connection string to build a mongo client.
    /// Either use this or provide an existing <see cref="MongoClient"/>.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets an existing MongoClient to use for the storage operations.
    /// Either use this or provide a <see cref="ConnectionString"/> to provision a client.
    /// </summary>
    public IMongoClient? MongoClient { get; set; }

    /// <summary>
    /// Gets or sets the database name to store the cache.
    /// </summary>
    public string? DatabaseName { get; set; }

    /// <summary>
    /// Gets or sets the collection name to store the cache.
    /// </summary>
    /// <value>Default value is <c>Cache</c></value>
    public string? CollectionName { get; set; } = "Cache";

    /// <summary>
    /// Gets or sets a value indicating whether initialization it will check for the Collection
    /// existence and create it if it doesn't exist.
    /// </summary>
    /// <value>Default value is <see langword="true"/>.</value>
    public bool CreateIfNotExists { get; set; } = true;

    /// <summary>
    /// Gets the current options values.
    /// </summary>
    MongoCacheOptions IOptions<MongoCacheOptions>.Value => this;
}
