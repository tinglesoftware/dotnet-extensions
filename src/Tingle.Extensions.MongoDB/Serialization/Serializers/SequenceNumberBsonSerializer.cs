using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.MongoDB.Serialization.Serializers;

/// <summary>Implementation of <see cref="IBsonSerializer{TValue}"/> for <see cref="SequenceNumber"/></summary>
public class SequenceNumberBsonSerializer : StructSerializerBase<SequenceNumber>, IRepresentationConfigurable<SequenceNumberBsonSerializer>
{
    // private fields
    private readonly Int64Serializer _int64Serializer = new();
    private readonly StringSerializer _stringSerializer = new();
    private readonly BsonType _representation;

    // constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="SequenceNumberBsonSerializer"/> class.
    /// </summary>
    public SequenceNumberBsonSerializer() : this(BsonType.Int64) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SequenceNumberBsonSerializer"/> class.
    /// </summary>
    /// <param name="representation">The representation.</param>
    public SequenceNumberBsonSerializer(BsonType representation)
    {
        switch (representation)
        {
            case BsonType.Int64:
            case BsonType.String:
                break;

            default:
                var message = string.Format("{0} is not a valid representation for a SequenceNumberBsonSerializer.", representation);
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
    public override SequenceNumber Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonType = context.Reader.CurrentBsonType;
        return bsonType switch
        {
            BsonType.String => new SequenceNumber(long.Parse(_stringSerializer.Deserialize(context))),
            BsonType.Int64 => new SequenceNumber(_int64Serializer.Deserialize(context)),
            _ => throw CreateCannotDeserializeFromBsonTypeException(bsonType),
        };
    }

    /// <inheritdoc/>
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, SequenceNumber value)
    {
        var bsonWriter = context.Writer;

        switch (_representation)
        {
            case BsonType.String:
                bsonWriter.WriteString(value.ToString());
                break;

            case BsonType.Int64:
                bsonWriter.WriteInt64(value);
                break;

            default:
                var message = string.Format("'{0}' is not a valid SequenceNumber representation.", _representation);
                throw new BsonSerializationException(message);
        }
    }

    /// <inheritdoc/>
    public SequenceNumberBsonSerializer WithRepresentation(BsonType representation)
    {
        if (representation == _representation)
        {
            return this;
        }
        else
        {
            return new SequenceNumberBsonSerializer(representation);
        }
    }

    // explicit interface implementations
    IBsonSerializer IRepresentationConfigurable.WithRepresentation(BsonType representation)
    {
        return WithRepresentation(representation);
    }

}
