using Microsoft.AspNetCore.DataProtection.Repositories;
using MongoDB.Driver;
using System.Xml.Linq;

namespace Tingle.AspNetCore.DataProtection.MongoDB;

/// <summary>
/// An <see cref="IXmlRepository"/> which is backed by Mongo.
/// </summary>
public class MongoXmlRepository : IXmlRepository
{
    private readonly Func<IMongoCollection<DataProtectionKey>> _collectionFactory;

    /// <summary>
    /// Creates a <see cref="MongoXmlRepository"/> with keys stored at the given collection.
    /// </summary>
    /// <param name="databaseFactory">The delegate used to create <see cref="IMongoCollection{TDocument}"/> instances.</param>
    public MongoXmlRepository(Func<IMongoCollection<DataProtectionKey>> databaseFactory)
    {
        _collectionFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));
    }

    /// <inheritdoc />
    public IReadOnlyCollection<XElement> GetAllElements()
    {
        var collection = _collectionFactory();
        return collection.Find(Builders<DataProtectionKey>.Filter.Empty)
                         .ToList()
                         .Select(key => XElement.Parse(key.Xml ?? throw new InvalidOperationException($"XML data is missing for {key.Id}")))
                         .ToList()
                         .AsReadOnly();
    }

    /// <inheritdoc />
    public void StoreElement(XElement element, string friendlyName)
    {
        var newKey = new DataProtectionKey()
        {
            FriendlyName = friendlyName,
            Xml = element.ToString(SaveOptions.DisableFormatting)
        };

        var collection = _collectionFactory();
        collection.InsertOne(newKey);
    }
}
