using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.AspNetCore.JsonPatch.Converters;

internal class TypedJsonPatchDocumentConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(JsonPatchDocument<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var modelType = typeToConvert.GetGenericArguments()[0];
        var conveterType = typeof(JsonPatchDocumentConverter<>).MakeGenericType(modelType);
        return (JsonConverter?)Activator.CreateInstance(conveterType);
    }
}
