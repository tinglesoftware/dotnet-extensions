using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Converters;

/// <summary>
/// A <see cref="JsonConverter{T}"/> for <see cref="SequenceNumber"/>
/// </summary>
public class SequenceNumberJsonConverter : JsonConverter<SequenceNumber>
{
    /// <inheritdoc/>
    public override SequenceNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new InvalidOperationException("Only numbers are supported");
        }

        var value = reader.GetInt64();
        return new SequenceNumber(value);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, SequenceNumber value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
