using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.AspNetCore.JsonPatch.Converters;

internal class TypedJsonPatchMergeDocumentConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(JsonPatchMergeDocument<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var modelType = typeToConvert.GetGenericArguments()[0];
        var conveterType = typeof(JsonPatchMergeDocumentConverter<>).MakeGenericType(modelType);
        return (JsonConverter?)Activator.CreateInstance(conveterType);
    }
}
