using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Converters;

/// <summary>
/// A <see cref="JsonConverter{T}"/> for <see cref="Currency"/>
/// </summary>
public class CurrencyJsonConverter : JsonConverter<Currency>
{
    /// <inheritdoc/>
    public override Currency? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        return string.IsNullOrWhiteSpace(s) ? null : Currency.FromCode(s);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Currency value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Code);
    }
}
