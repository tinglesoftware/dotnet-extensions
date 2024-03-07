using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.Reflection;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.MongoDB.Serialization.Conventions;

/// <summary>
/// A convention that allows you to set the Etag serialization representation
/// </summary>
public class EtagRepresentationConvention : ConventionBase, IMemberMapConvention
{
    private readonly BsonType _representation;

    /// <summary>
    /// Initializes a new instance of the <see cref="EtagRepresentationConvention" /> class.
    /// </summary>
    /// <param name="representation">The serialization representation.</param>
    public EtagRepresentationConvention(BsonType representation)
    {
        EnsureRepresentationIsValidForEtags(representation);
        _representation = representation;
    }

    /// <summary>Gets the representation.</summary>
    public BsonType Representation => _representation;

    /// <inheritdoc/>
    public void Apply(BsonMemberMap memberMap)
    {
        var memberType = memberMap.MemberType;
        var memberTypeInfo = memberType.GetTypeInfo();

        if (memberTypeInfo == typeof(Etag))
        {
            var serializer = memberMap.GetSerializer();
            if (serializer is IRepresentationConfigurable representationConfigurableSerializer)
            {
                var reconfiguredSerializer = representationConfigurableSerializer.WithRepresentation(_representation);
                memberMap.SetSerializer(reconfiguredSerializer);
            }
            return;
        }

        if (IsNullableEtag(memberType))
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

    private static bool IsNullableEtag(Type type)
    {
        return
            type.GetTypeInfo().IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
            Nullable.GetUnderlyingType(type)!.GetTypeInfo() == typeof(Etag);
    }

    private static void EnsureRepresentationIsValidForEtags(BsonType representation)
    {
        if (representation == BsonType.Int64 ||
            representation == BsonType.String ||
            representation == BsonType.Binary)
        {
            return;
        }
        throw new ArgumentException("Etags can only be represented as Int64, String, or Binary", nameof(representation));
    }
}
