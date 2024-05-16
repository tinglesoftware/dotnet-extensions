using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Converters;

/// <summary>
/// A <see cref="JsonConverter{T}"/> for <see cref="ConnectionStringBuilder"/>
/// </summary>
public class ConnectionStringBuilderJsonConverter : JsonConverter<ConnectionStringBuilder>
{
    /// <inheritdoc/>
    public override ConnectionStringBuilder Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new InvalidOperationException("Only strings are supported");
        }

        var str = reader.GetString();
        return new ConnectionStringBuilder(str!);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, ConnectionStringBuilder value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
