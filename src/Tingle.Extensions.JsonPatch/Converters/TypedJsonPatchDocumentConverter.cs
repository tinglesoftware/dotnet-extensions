using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tingle.Extensions.JsonPatch.Operations;

namespace Tingle.Extensions.JsonPatch.Converters;

[RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
[RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
public class TypedJsonPatchDocumentConverter : JsonConverterFactory
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

    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    public class TypedJsonPatchDocumentConverterInner<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : JsonConverter<JsonPatchDocument<T>> where T : class
    {
        /// <inheritdoc/>
        public override JsonPatchDocument<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return default;

            var ops = JsonSerializer.Deserialize<List<Operation>>(ref reader, options);
            var operations = ops?.Select(o => new Operation<T>
            {
                path = o.path,
                op = o.op,
                from = o.from,
                value = o.value,
            }).ToList() ?? [];
            return new JsonPatchDocument<T>(operations);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, JsonPatchDocument<T> value, JsonSerializerOptions options)
        {
            // we write an array of the operations
            var ops = value.Operations.Select(o => (Operation)o).ToList();
            JsonSerializer.Serialize(writer, ops, options);
        }
    }
}
