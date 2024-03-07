using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.MongoDB.Serialization.Serializers;

/// <summary>Implementation of <see cref="IBsonSerializer{TValue}"/> for <see cref="Duration"/></summary>
public class DurationBsonSerializer : StructSerializerBase<Duration>, IRepresentationConfigurable<DurationBsonSerializer>
{
    // private fields
    private readonly StringSerializer _stringSerializer = new();
    private readonly BsonType _representation;

    // constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="DurationBsonSerializer"/> class.
    /// </summary>
    public DurationBsonSerializer() : this(BsonType.String) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DurationBsonSerializer"/> class.
    /// </summary>
    /// <param name="representation">The representation.</param>
    public DurationBsonSerializer(BsonType representation)
    {
        switch (representation)
        {
            case BsonType.String:
                break;

            default:
                var message = string.Format("{0} is not a valid representation for a DurationBsonSerializer.", representation);
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
    public override Duration Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonType = context.Reader.CurrentBsonType;
        return bsonType switch
        {
            BsonType.String => Duration.Parse(_stringSerializer.Deserialize(context)),
            _ => throw CreateCannotDeserializeFromBsonTypeException(bsonType),
        };
    }

    /// <inheritdoc/>
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Duration value)
    {
        var bsonWriter = context.Writer;

        switch (_representation)
        {
            case BsonType.String:
                bsonWriter.WriteString(value.ToString());
                break;

            default:
                var message = string.Format("'{0}' is not a valid Duration representation.", _representation);
                throw new BsonSerializationException(message);
        }
    }

    /// <inheritdoc/>
    public DurationBsonSerializer WithRepresentation(BsonType representation)
    {
        if (representation == _representation)
        {
            return this;
        }
        else
        {
            return new DurationBsonSerializer(representation);
        }
    }

    // explicit interface implementations
    IBsonSerializer IRepresentationConfigurable.WithRepresentation(BsonType representation)
    {
        return WithRepresentation(representation);
    }

}
