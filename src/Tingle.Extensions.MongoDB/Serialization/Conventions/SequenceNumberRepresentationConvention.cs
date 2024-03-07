using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.Reflection;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.MongoDB.Serialization.Conventions;

/// <summary>
/// A convention that allows you to set the SequenceNumber serialization representation
/// </summary>
public class SequenceNumberRepresentationConvention : ConventionBase, IMemberMapConvention
{
    private readonly BsonType _representation;

    /// <summary>
    /// Initializes a new instance of the <see cref="SequenceNumberRepresentationConvention" /> class.
    /// </summary>
    /// <param name="representation">The serialization representation.</param>
    public SequenceNumberRepresentationConvention(BsonType representation)
    {
        EnsureRepresentationIsValidForSequenceNumbers(representation);
        _representation = representation;
    }

    /// <summary>Gets the representation.</summary>
    public BsonType Representation => _representation;

    /// <inheritdoc/>
    public void Apply(BsonMemberMap memberMap)
    {
        var memberType = memberMap.MemberType;
        var memberTypeInfo = memberType.GetTypeInfo();

        if (memberTypeInfo == typeof(SequenceNumber))
        {
            var serializer = memberMap.GetSerializer();
            if (serializer is IRepresentationConfigurable representationConfigurableSerializer)
            {
                var reconfiguredSerializer = representationConfigurableSerializer.WithRepresentation(_representation);
                memberMap.SetSerializer(reconfiguredSerializer);
            }
            return;
        }

        if (IsNullableSequenceNumber(memberType))
        {
            var serializer = memberMap.GetSerializer();
            if (serializer is IChildSerializerConfigurable childSerializerConfigurableSerializer)
            {
                var childSerializer = childSerializerConfigurableSerializer.ChildSerializer;
                if (childSerializer is IRepresentationConfigurable representationConfigurableChildSerializer)
                {
                    var reconfiguredChildSerializer = representationConfigurableChildSerializer.WithRepresentation(_representation);
                    var reconfiguredSerializer = childSerializerConfigurableSerializer.WithChildSerializer(reconfiguredChildSerializer);
                    memberMap.SetSerializer(reconfiguredSerializer);
                }
            }
            return;
        }
    }

    private static bool IsNullableSequenceNumber(Type type)
    {
        return
            type.GetTypeInfo().IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
            Nullable.GetUnderlyingType(type)!.GetTypeInfo() == typeof(SequenceNumber);
    }

    private static void EnsureRepresentationIsValidForSequenceNumbers(BsonType representation)
    {
        if (representation == BsonType.Int64 ||
            representation == BsonType.String)
        {
            return;
        }
        throw new ArgumentException("SequenceNumbers can only be represented as Int64 or String", nameof(representation));
    }
}
