using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives;

/// <summary>
/// A <see cref="JsonConverter{T}"/> for <see cref="Language"/>
/// </summary>
public class LanguageJsonConverter : JsonConverter<Language>
{
    /// <inheritdoc/>
    public override Language? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        return string.IsNullOrWhiteSpace(s) ? null : Language.FromCode(s);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Language value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ThreeLetterCode);
    }
}
