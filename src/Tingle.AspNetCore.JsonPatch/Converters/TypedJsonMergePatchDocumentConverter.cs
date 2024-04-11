using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.AspNetCore.JsonPatch.Converters;

internal class TypedJsonMergePatchDocumentConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(JsonMergePatchDocument<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var modelType = typeToConvert.GetGenericArguments()[0];
        var conveterType = typeof(JsonMergePatchDocumentConverter<>).MakeGenericType(modelType);
        return (JsonConverter?)Activator.CreateInstance(conveterType);
    }
}
