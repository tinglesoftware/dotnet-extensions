using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Converters;

/// <summary>
/// <see cref="JsonConverter{T}"/> to convert <see cref="IPAddress"/> to and from strings.
/// </summary>
public class JsonIPAddressConverter : JsonConverter<IPAddress>
{
    /// <inheritdoc/>
    public override IPAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.String)
        {
            throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(typeof(IPAddress));
        }

        var value = reader.GetString()!;

        try
        {
            return IPAddress.Parse(value);
        }
        catch (Exception ex)
        {
            throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(typeof(IPAddress), value, ex);
        }
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}
