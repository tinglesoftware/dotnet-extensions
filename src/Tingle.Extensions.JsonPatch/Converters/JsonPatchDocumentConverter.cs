using System.Text.Json;
using System.Text.Json.Serialization;
using Tingle.Extensions.JsonPatch.Operations;

namespace Tingle.Extensions.JsonPatch.Converters;

public class JsonPatchDocumentConverter : JsonConverter<JsonPatchDocument>
{
    /// <inheritdoc/>
    public override JsonPatchDocument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;

        var targetOperations = JsonSerializer.Deserialize<List<Operation>>(ref reader, options);

        // container target: the JsonPatchDocument. 
        var container = new JsonPatchDocument(targetOperations ?? new List<Operation>());

        return container;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, JsonPatchDocument value, JsonSerializerOptions options)
    {
        // we write an array of the operations
        JsonSerializer.Serialize(writer, value.Operations, options);
    }
}
