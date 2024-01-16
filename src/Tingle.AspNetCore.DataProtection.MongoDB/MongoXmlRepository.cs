using Microsoft.AspNetCore.DataProtection.Repositories;
using MongoDB.Driver;
using System.Xml.Linq;

namespace Tingle.AspNetCore.DataProtection.MongoDB;

/// <summary>
/// An <see cref="IXmlRepository"/> which is backed by Mongo.
/// </summary>
/// <param name="databaseFactory">The delegate used to create <see cref="IMongoCollection{TDocument}"/> instances.</param>
public class MongoXmlRepository(Func<IMongoCollection<DataProtectionKey>> databaseFactory) : IXmlRepository
{
    /// <inheritdoc />
    public IReadOnlyCollection<XElement> GetAllElements()
    {
        var collection = databaseFactory();
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

        var collection = databaseFactory();
        collection.InsertOne(newKey);
    }
}
