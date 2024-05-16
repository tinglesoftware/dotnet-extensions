using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Converters;

/// <summary>
/// A <see cref="JsonConverter{T}"/> for <see cref="Continent"/>
/// </summary>
public class ContinentJsonConverter : JsonConverter<Continent>
{
    /// <inheritdoc/>
    public override Continent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        return string.IsNullOrWhiteSpace(s) ? null : new Continent(s);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Continent value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}
