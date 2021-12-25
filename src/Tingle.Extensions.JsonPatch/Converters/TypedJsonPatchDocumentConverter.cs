using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tingle.Extensions.JsonPatch.Operations;

namespace Tingle.Extensions.JsonPatch.Converters;

internal class TypedJsonPatchDocumentConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(JsonPatchDocument<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var modelType = typeToConvert.GetGenericArguments()[0];
        var conveterType = typeof(TypedJsonPatchDocumentConverterInner<>).MakeGenericType(modelType);
        return (JsonConverter?)Activator.CreateInstance(conveterType);
    }

    internal class TypedJsonPatchDocumentConverterInner<T> : JsonConverter<JsonPatchDocument<T>> where T : class
    {
        /// <inheritdoc/>
        public override JsonPatchDocument<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return default;

            var operations = JsonSerializer.Deserialize<List<Operation<T>>>(ref reader, options);
            return new JsonPatchDocument<T>(operations ?? new List<Operation<T>>());
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, JsonPatchDocument<T> value, JsonSerializerOptions options)
        {
            // we write an array of the operations
            JsonSerializer.Serialize(writer, value.Operations, options);
        }
    }
}
