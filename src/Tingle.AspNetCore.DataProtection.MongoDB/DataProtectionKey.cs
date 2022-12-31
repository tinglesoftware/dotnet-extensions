using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tingle.AspNetCore.DataProtection.MongoDB;

/// <summary>
/// Model used by <see cref="MongoXmlRepository"/>.
/// </summary>
public class DataProtectionKey
{
    /// <summary>
    /// The entity identifier of the <see cref="DataProtectionKey"/>.
    /// </summary>
    [BsonId]
    public ObjectId Id { get; set; }

    /// <summary>
    /// The friendly name of the <see cref="DataProtectionKey"/>.
    /// </summary>
    public string? FriendlyName { get; set; }

    /// <summary>
    /// The XML representation of the <see cref="DataProtectionKey"/>.
    /// </summary>
    public string? Xml { get; set; }
}
