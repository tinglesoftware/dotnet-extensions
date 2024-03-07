#if NET8_0_OR_GREATER
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Net;

namespace Tingle.Extensions.MongoDB.Serialization.Serializers;

/// <summary>Implementation of <see cref="IBsonSerializer{TValue}"/> for <see cref="IPNetwork"/></summary>
public class IPNetworkBsonSerializer : StructSerializerBase<IPNetwork>, IRepresentationConfigurable<IPNetworkBsonSerializer>
{
    // private fields
    private readonly StringSerializer _stringSerializer = new();
    private readonly BsonType _representation;

    // constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="IPNetworkBsonSerializer"/> class.
    /// </summary>
    public IPNetworkBsonSerializer() : this(BsonType.String) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IPNetworkBsonSerializer"/> class.
    /// </summary>
    /// <param name="representation">The representation.</param>
    public IPNetworkBsonSerializer(BsonType representation)
    {
        switch (representation)
        {
            case BsonType.String:
                break;

            default:
                var message = string.Format("{0} is not a valid representation for a IPNetworkBsonSerializer.", representation);
                throw new ArgumentException(message);
        }

        _representation = representation;
    }

    // public properties
    /// <summary>
    /// Gets the representation.
    /// </summary>
    /// <value>
    /// The representation.
    /// </value>
    public BsonType Representation => _representation;

    /// <inheritdoc/>
    public override IPNetwork Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonType = context.Reader.CurrentBsonType;
        return bsonType switch
        {
            BsonType.String => IPNetwork.Parse(_stringSerializer.Deserialize(context)),
            _ => throw CreateCannotDeserializeFromBsonTypeException(bsonType),
        };
    }

    /// <inheritdoc/>
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, IPNetwork value)
    {
        var bsonWriter = context.Writer;

        switch (_representation)
        {
            case BsonType.String:
                bsonWriter.WriteString(value.ToString());
                break;

            default:
                var message = string.Format("'{0}' is not a valid IPNetwork representation.", _representation);
                throw new BsonSerializationException(message);
        }
    }

    /// <inheritdoc/>
    public IPNetworkBsonSerializer WithRepresentation(BsonType representation)
    {
        if (representation == _representation)
        {
            return this;
        }
        else
        {
            return new IPNetworkBsonSerializer(representation);
        }
    }

    // explicit interface implementations
    IBsonSerializer IRepresentationConfigurable.WithRepresentation(BsonType representation)
    {
        return WithRepresentation(representation);
    }
}
#endif
