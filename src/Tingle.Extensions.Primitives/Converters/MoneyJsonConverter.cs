using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Converters;

/// <summary>
/// A <see cref="JsonConverter{T}"/> for <see cref="Money"/>
/// </summary>
public class MoneyJsonConverter : JsonConverter<Money>
{
    private readonly string format;

    /// <summary>Creates an instance of <see cref="MoneyJsonConverter"/> using <c>I</c> format.</summary>
    public MoneyJsonConverter() : this("I") { }

    /// <summary>Creates an instance of <see cref="MoneyJsonConverter"/>.</summary>
    /// <param name="format">The format to use.</param>
    public MoneyJsonConverter(string format)
    {
        if (string.IsNullOrWhiteSpace(this.format = format))
        {
            throw new ArgumentException($"'{nameof(format)}' cannot be null or whitespace.", nameof(format));
        }
    }

    /// <inheritdoc/>
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only strings are supported");
        }

        var str = reader.GetString()!;
        return Money.Parse(str);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(format));
    }
}
