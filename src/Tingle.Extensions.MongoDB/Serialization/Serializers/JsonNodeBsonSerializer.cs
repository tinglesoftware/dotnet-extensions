using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Text.Json.Nodes;

#nullable disable

namespace Tingle.Extensions.MongoDB.Serialization.Serializers;

/// <summary>
/// Implementation of <see cref="IBsonSerializer{TValue}"/> for <see cref="JsonNode"/>
/// </summary>
internal class JsonNodeBsonSerializer : SerializerBase<JsonNode>
{
    /// <inheritdoc/>
    public override JsonNode Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.Array)
        {
            var barr = BsonSerializer.Deserialize<BsonArray>(context.Reader);
            return JsonNode.Parse(barr.ToJson(JsonElementBsonSerializer.settings))!;
        }
        else if (context.Reader.CurrentBsonType is BsonType.Document or BsonType.EndOfDocument)
        {
            var bdoc = BsonDocumentSerializer.Instance.Deserialize(context);
            return JsonNode.Parse(bdoc.ToJson(JsonElementBsonSerializer.settings))!;
        }
        else
        {
            context.Reader.ReadNull();
            return null;
        }
    }

    /// <inheritdoc/>
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JsonNode value)
    {
        if (value != null)
        {
            var json = value.ToString();
            if (value is JsonArray)
            {
                var barr = BsonSerializer.Deserialize<BsonArray>(json);
                BsonSerializer.Serialize(context.Writer, barr/*, args: args*/); // passing args causes errors
            }
            else if (value is JsonObject)
            {
                var bdoc = BsonDocument.Parse(json);
                BsonSerializer.Serialize(context.Writer, bdoc/*, args: args*/); // passing args causes errors
            }
            else
            {
                var bdoc = BsonDocument.Parse(json);
                BsonDocumentSerializer.Instance.Serialize(context, bdoc);
            }
        }
        else
        {
            context.Writer.WriteNull();
        }
    }
}
