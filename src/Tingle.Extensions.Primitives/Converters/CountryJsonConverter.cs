using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Converters;

/// <summary>
/// A <see cref="JsonConverter{T}"/> for <see cref="Country"/>
/// </summary>
public class CountryJsonConverter : JsonConverter<Country>
{
    /// <inheritdoc/>
    public override Country? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        return string.IsNullOrWhiteSpace(s) ? null : Country.FromCode(s);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Country value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ThreeLetterCode);
    }
}
