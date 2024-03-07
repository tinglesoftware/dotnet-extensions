using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Converters;

/// <summary>A <see cref="JsonConverter{T}"/> for <see cref="Ksuid"/>.</summary>
public class KsuidJsonConverter : JsonConverter<Ksuid>
{
    private readonly string format;

    /// <summary>Creates an instance of <see cref="KsuidJsonConverter"/> using <see cref="Ksuid.Base62Format"/>.</summary>
    public KsuidJsonConverter() : this(Ksuid.Base62Format) { }

    /// <summary>Creates an instance of <see cref="KsuidJsonConverter"/>.</summary>
    /// <param name="format">The format to use.</param>
    public KsuidJsonConverter(string format)
    {
        if (string.IsNullOrWhiteSpace(this.format = format))
        {
            throw new ArgumentException($"'{nameof(format)}' cannot be null or whitespace.", nameof(format));
        }
    }

    /// <inheritdoc/>
    public override Ksuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only strings are supported");
        }

        var value = reader.GetString()!;
        return Ksuid.Parse(value);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Ksuid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(format));
    }
}
