using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.Reflection;

namespace Tingle.Extensions.MongoDB.Serialization.Conventions;

/// <summary>
/// A convention that allows you to set the DateTimeOffset serialization representation
/// </summary>
public class DateTimeOffsetRepresentationConvention : ConventionBase, IMemberMapConvention
{
    private readonly BsonType _representation;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeOffsetRepresentationConvention" /> class.
    /// </summary>
    /// <param name="representation">The serialization representation.</param>
    public DateTimeOffsetRepresentationConvention(BsonType representation)
    {
        EnsureRepresentationIsValidForDateTimeOffsets(representation);
        _representation = representation;
    }

    /// <summary>Gets the representation.</summary>
    public BsonType Representation => _representation;

    /// <inheritdoc/>
    public void Apply(BsonMemberMap memberMap)
    {
        var memberType = memberMap.MemberType;
        var memberTypeInfo = memberType.GetTypeInfo();

        if (memberTypeInfo == typeof(DateTimeOffset))
        {
            var serializer = memberMap.GetSerializer();
            if (serializer is IRepresentationConfigurable representationConfigurableSerializer)
            {
                var reconfiguredSerializer = representationConfigurableSerializer.WithRepresentation(_representation);
                memberMap.SetSerializer(reconfiguredSerializer);
            }
            return;
        }

        if (IsNullableDateTimeOffset(memberType))
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

    private static bool IsNullableDateTimeOffset(Type type)
    {
        return
            type.GetTypeInfo().IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
            Nullable.GetUnderlyingType(type)!.GetTypeInfo() == typeof(DateTimeOffset);
    }

    private static void EnsureRepresentationIsValidForDateTimeOffsets(BsonType representation)
    {
        if (representation == BsonType.Array ||
            representation == BsonType.Document ||
            representation == BsonType.String ||
            representation == BsonType.DateTime)
        {
            return;
        }
        throw new ArgumentException("DateTimeOffsets can only be represented as Array, Document, String, or DateTime", nameof(representation));
    }
}
