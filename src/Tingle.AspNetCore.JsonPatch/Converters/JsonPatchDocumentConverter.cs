using System.Text.Json;
using System.Text.Json.Serialization;
using Tingle.AspNetCore.JsonPatch.Operations;

namespace Tingle.AspNetCore.JsonPatch.Converters;

public class JsonPatchDocumentConverter : JsonConverter<JsonPatchDocument>
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(JsonPatchDocument);

    /// <inheritdoc/>
    public override JsonPatchDocument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;

        var operations = JsonSerializer.Deserialize<List<Operation>>(ref reader, options);
        return new JsonPatchDocument(operations ?? [], options);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, JsonPatchDocument value, JsonSerializerOptions options)
    {
        // we write an array of the operations
        JsonSerializer.Serialize(writer, value.Operations, options);
    }
}

public class JsonPatchDocumentConverter<TModel> : JsonConverter<JsonPatchDocument<TModel>> where TModel : class
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(JsonPatchDocument<TModel>);

    /// <inheritdoc/>
    public override JsonPatchDocument<TModel>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;

        var operations = JsonSerializer.Deserialize<List<Operation<TModel>>>(ref reader, options);
        return new JsonPatchDocument<TModel>(operations ?? [], options);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, JsonPatchDocument<TModel> value, JsonSerializerOptions options)
    {
        // we write an array of the operations
        JsonSerializer.Serialize(writer, value.Operations, options);
    }
}
