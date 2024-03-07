using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Reflection;
using Tingle.Extensions.MongoDB.Serialization.Serializers;

namespace Tingle.Extensions.MongoDB.Serialization;

/// <summary>
/// Provides serializers for System.Text.Json types.
/// </summary>
public class JsonSerializationProvider : BsonSerializationProviderBase
{
    private static readonly Dictionary<Type, Type> __serializersTypes = new()
    {
        { typeof(System.Text.Json.JsonElement), typeof(JsonElementBsonSerializer) },
        { typeof(System.Text.Json.Nodes.JsonObject), typeof(JsonObjectBsonSerializer) },
        { typeof(System.Text.Json.Nodes.JsonNode), typeof(JsonNodeBsonSerializer) },
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
