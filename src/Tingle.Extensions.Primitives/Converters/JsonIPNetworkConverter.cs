using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Converters;

/// <summary>
/// <see cref="JsonConverter{T}"/> to convert <see cref="IPNetwork"/> to and from strings.
/// </summary>
public class JsonIPNetworkConverter : JsonConverter<IPNetwork>
{
    /// <inheritdoc/>
    public override IPNetwork Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.String)
        {
            throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(typeof(IPNetwork));
        }

        var value = reader.GetString()!;

        try
        {
            return IPNetwork.Parse(value);
        }
        catch (Exception ex)
        {
            throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(typeof(IPNetwork), value, ex);
        }
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, IPNetwork value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
