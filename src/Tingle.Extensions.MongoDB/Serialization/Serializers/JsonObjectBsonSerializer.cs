using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Text.Json.Nodes;

#nullable disable

namespace Tingle.Extensions.MongoDB.Serialization.Serializers;

/// <summary>
/// Implementation of <see cref="IBsonSerializer{TValue}"/> for <see cref="JsonObject"/>
/// </summary>
internal class JsonObjectBsonSerializer : SerializerBase<JsonObject>
{
    /// <inheritdoc/>
    public override JsonObject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType != BsonType.Null)
        {
            var bdoc = BsonDocumentSerializer.Instance.Deserialize(context);
            return (JsonObject)JsonNode.Parse(bdoc.ToJson(JsonElementBsonSerializer.settings))!;
        }
        else
        {
            context.Reader.ReadNull();
            return null;
        }
    }

    /// <inheritdoc/>
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JsonObject value)
    {
        if (value != null)
        {
            var bdoc = BsonDocument.Parse(value.ToString());
            BsonDocumentSerializer.Instance.Serialize(context, bdoc);
        }
        else
        {
            context.Writer.WriteNull();
        }
    }
}
