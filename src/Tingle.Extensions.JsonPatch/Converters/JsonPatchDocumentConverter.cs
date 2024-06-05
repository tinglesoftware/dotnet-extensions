using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tingle.Extensions.JsonPatch.Operations;

namespace Tingle.Extensions.JsonPatch.Converters;

[RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
[RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
public class JsonPatchDocumentConverter : JsonConverter<JsonPatchDocument>
{
    /// <inheritdoc/>
    public override JsonPatchDocument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;

        var operations = JsonSerializer.Deserialize<List<Operation>>(ref reader, options);

        return new JsonPatchDocument(operations ?? []);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, JsonPatchDocument value, JsonSerializerOptions options)
    {
        // we write an array of the operations
        JsonSerializer.Serialize(writer, value.Operations, options);
    }
}
