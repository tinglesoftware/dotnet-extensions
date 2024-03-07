using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Converters;

/// <summary>A <see cref="JsonConverter{T}"/> for <see cref="ByteSize"/>.</summary>
/// <param name="binary">Whether to use binary format or not.</param>
public class ByteSizeJsonConverter(bool binary) : JsonConverter<ByteSize>
{
    /// <summary>Creates an instance of <see cref="ByteSizeJsonConverter"/> that uses binary format.</summary>
    public ByteSizeJsonConverter() : this(true) { }

    /// <inheritdoc/>
    public override ByteSize Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only strings are supported");
        }

        var value = reader.GetString();
        return ByteSize.Parse(value!);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, ByteSize value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(binary ? value.ToBinaryString() : value.ToString());
    }
}
