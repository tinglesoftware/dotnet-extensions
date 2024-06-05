using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Converters;

/// <summary>
/// A <see cref="JsonConverter{T}"/> for <see cref="Etag"/>
/// </summary>
public class EtagJsonConverter : JsonConverter<Etag>
{
    private readonly string format;

    /// <summary>Creates an instance of <see cref="EtagJsonConverter"/> using <see cref="Etag.Base64Format"/>.</summary>
    public EtagJsonConverter() : this(Etag.Base64Format) { }

    /// <summary>Creates an instance of <see cref="EtagJsonConverter"/>.</summary>
    /// <param name="format">The format to use.</param>
    public EtagJsonConverter(string format)
    {
        if (string.IsNullOrWhiteSpace(this.format = format))
        {
            throw new ArgumentException($"'{nameof(format)}' cannot be null or whitespace.", nameof(format));
        }
    }

    /// <inheritdoc/>
    public override Etag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only strings are supported");
        }

        var str = reader.GetString();
        return Etag.Parse(str!);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Etag value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(format));
    }
}
