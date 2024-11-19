using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Text.Json;
using SC = Tingle.Extensions.MongoDB.MongoJsonSerializerContext;

namespace Tingle.Extensions.MongoDB.Serialization.Serializers;

/// <summary>
/// Implementation of <see cref="IBsonSerializer{TValue}"/> for <see cref="JsonElement"/>
/// </summary>
internal class JsonElementBsonSerializer : StructSerializerBase<JsonElement>
{
    /// <summary>
    /// Create a JsonWriterSettings object to use when serializing BSON docs to JSON.
    /// This will force the serializer to create valid ("strict") JSON.
    /// Without this, ObjectIDs, Dates, and Integers are output as {"_id": ObjectId(ds8f7s9d87f89sd9f8d9f7sd9f9s8d)},
    /// {"date": ISODate("2020-04-14 14:30:00:000")}, {"int": NumberInt(130)} and respectively, which is not valid JSON
    /// </summary>
    internal static readonly JsonWriterSettings settings = new() { OutputMode = JsonOutputMode.CanonicalExtendedJson, };

    /// <inheritdoc/>
    public override JsonElement Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        if (context.Reader.CurrentBsonType == BsonType.Array)
        {
            var barr = BsonSerializer.Deserialize<BsonArray>(context.Reader);
            return JsonSerializer.Deserialize(barr.ToJson(settings), SC.Default.JsonElement);
        }
        else if (context.Reader.CurrentBsonType is BsonType.Document or BsonType.EndOfDocument)
        {
            var bdoc = BsonDocumentSerializer.Instance.Deserialize(context);
            return JsonSerializer.Deserialize(bdoc.ToJson(settings), SC.Default.JsonElement);
        }
        else
        {
            context.Reader.ReadNull();
            return default;
        }
    }

    /// <inheritdoc/>
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JsonElement value)
    {
        if (value.ValueKind != JsonValueKind.Undefined)
        {
            var json = value.GetRawText();
            if (value.ValueKind == JsonValueKind.Array)
            {
                var barr = BsonSerializer.Deserialize<BsonArray>(json);
                BsonSerializer.Serialize(context.Writer, barr/*, args: args*/); // passing args causes errors
            }
            else if (value.ValueKind == JsonValueKind.Object)
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
