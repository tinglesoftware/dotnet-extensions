using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Net;
using System.Reflection;
using Tingle.Extensions.MongoDB.Serialization.Serializers;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.MongoDB.Serialization;

/// <summary>
/// Provides serializers for Tingle types.
/// </summary>
public class TingleSerializationProvider : BsonSerializationProviderBase
{
    private static readonly Dictionary<Type, Type> __serializersTypes = new()
    {
        { typeof(IPNetwork), typeof(IPNetworkBsonSerializer) },
        { typeof(Duration), typeof(DurationBsonSerializer) },
        { typeof(Etag), typeof(EtagBsonSerializer) },
        { typeof(SequenceNumber), typeof(SequenceNumberBsonSerializer) }
    };

    /// <inheritdoc/>
    public override IBsonSerializer? GetSerializer(Type type, IBsonSerializerRegistry serializerRegistry)
    {
        ArgumentNullException.ThrowIfNull(type);

        var typeInfo = type.GetTypeInfo();
        if (typeInfo.IsGenericType && typeInfo.ContainsGenericParameters)
        {
            var message = string.Format("Generic type {0} has unassigned type parameters.", BsonUtils.GetFriendlyTypeName(type));
            throw new ArgumentException(message, nameof(type));
        }

        if (__serializersTypes.TryGetValue(type, out var serializerType))
        {
            return CreateSerializer(serializerType, serializerRegistry);
        }

        if (typeInfo.IsGenericType && !typeInfo.ContainsGenericParameters)
        {
            if (__serializersTypes.TryGetValue(type.GetGenericTypeDefinition(), out var serializerTypeDefinition))
            {
                return CreateGenericSerializer(serializerTypeDefinition, typeInfo.GetGenericArguments(), serializerRegistry);
            }
        }

        return null;
    }
}
