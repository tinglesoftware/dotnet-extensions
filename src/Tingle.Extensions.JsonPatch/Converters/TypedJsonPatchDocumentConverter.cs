using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tingle.Extensions.JsonPatch.Operations;

#if NETCOREAPP
#else
#pragma warning disable IDE0079 // Remove unnecessary suppression
#endif

namespace Tingle.Extensions.JsonPatch.Converters;

public class TypedJsonPatchDocumentConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(JsonPatchDocument<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var modelType = typeToConvert.GetGenericArguments()[0];
#pragma warning disable IL2055 // Either the type on which the MakeGenericType is called can't be statically determined, or the type parameters to be used for generic arguments can't be statically determined.
        var conveterType = typeof(TypedJsonPatchDocumentConverterInner<>).MakeGenericType(modelType);
#pragma warning restore IL2055 // Either the type on which the MakeGenericType is called can't be statically determined, or the type parameters to be used for generic arguments can't be statically determined.
        return (JsonConverter?)Activator.CreateInstance(conveterType);
    }

    public class TypedJsonPatchDocumentConverterInner<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : JsonConverter<JsonPatchDocument<T>> where T : class
    {
        /// <inheritdoc/>
        public override JsonPatchDocument<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return default;

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            var ops = JsonSerializer.Deserialize<List<Operation>>(ref reader, options);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            var operations = ops?.Select(o => new Operation<T>
            {
                path = o.path,
                op = o.op,
                from = o.from,
                value = o.value,
            }).ToList() ?? new List<Operation<T>>();
            return new JsonPatchDocument<T>(operations);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, JsonPatchDocument<T> value, JsonSerializerOptions options)
        {
            // we write an array of the operations
            var ops = value.Operations.Select(o => (Operation)o).ToList();
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            JsonSerializer.Serialize(writer, ops, options);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        }
    }
}
