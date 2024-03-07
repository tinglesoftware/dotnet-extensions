using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.MongoDB.Serialization.Serializers;

/// <summary>Implementation of <see cref="IBsonSerializer{TValue}"/> for <see cref="Etag"/></summary>
public class EtagBsonSerializer : StructSerializerBase<Etag>, IRepresentationConfigurable<EtagBsonSerializer>
{
    // private fields
    private readonly Int64Serializer _int64Serializer = new();
    private readonly StringSerializer _stringSerializer = new();
    private readonly ByteArraySerializer _byteArraySerializer = new();
    private readonly BsonType _representation;

    // constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="EtagBsonSerializer"/> class.
    /// </summary>
    public EtagBsonSerializer() : this(BsonType.Int64) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EtagBsonSerializer"/> class.
    /// </summary>
    /// <param name="representation">The representation.</param>
    public EtagBsonSerializer(BsonType representation)
    {
        switch (representation)
        {
            case BsonType.Int64:
            case BsonType.String:
            case BsonType.Binary:
                break;

            default:
                var message = string.Format("{0} is not a valid representation for a EtagBsonSerializer.", representation);
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
    public override Etag Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonType = context.Reader.CurrentBsonType;
        return bsonType switch
        {
            BsonType.String => new Etag(_stringSerializer.Deserialize(context)),
            BsonType.Int64 => new Etag((ulong)_int64Serializer.Deserialize(context)),
            BsonType.Binary => new Etag(_byteArraySerializer.Deserialize(context)),
            _ => throw CreateCannotDeserializeFromBsonTypeException(bsonType),
        };
    }

    /// <inheritdoc/>
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Etag value)
    {
        var bsonWriter = context.Writer;

        switch (_representation)
        {
            case BsonType.String:
                bsonWriter.WriteString(value.ToString());
                break;

            case BsonType.Int64:
                bsonWriter.WriteInt64((long)(ulong)value);
                break;

            case BsonType.Binary:
                bsonWriter.WriteBinaryData(value.ToByteArray());
                break;

            default:
                var message = string.Format("'{0}' is not a valid Etag representation.", _representation);
                throw new BsonSerializationException(message);
        }
    }

    /// <inheritdoc/>
    public EtagBsonSerializer WithRepresentation(BsonType representation)
    {
        if (representation == _representation)
        {
            return this;
        }
        else
        {
            return new EtagBsonSerializer(representation);
        }
    }

    // explicit interface implementations
    IBsonSerializer IRepresentationConfigurable.WithRepresentation(BsonType representation)
    {
        return WithRepresentation(representation);
    }

}
