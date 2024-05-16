using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Converters;

/// <summary>
/// A <see cref="JsonConverter{T}"/> for <see cref="SwiftCode"/>
/// </summary>
public class SwiftCodeJsonConverter : JsonConverter<SwiftCode>
{
    /// <inheritdoc/>
    public override SwiftCode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        return string.IsNullOrWhiteSpace(s) ? null : SwiftCode.Parse(s);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, SwiftCode value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
