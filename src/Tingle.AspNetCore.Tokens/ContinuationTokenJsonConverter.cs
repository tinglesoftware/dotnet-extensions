using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.AspNetCore.Tokens;

/// <summary>
/// A <see cref="JsonConverter{T}"/> for types implementing <see cref="IToken"/> (special purpose tokens) such as:
/// <list type="number">
/// <item><see cref="ContinuationToken{T}"/></item> and
/// <item><see cref="TimedContinuationToken{T}"/></item>
/// </list>
/// </summary>
public class ContinuationTokenJsonConverter : JsonConverter<IToken>
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeof(IToken).IsAssignableFrom(typeToConvert);

    /// <inheritdoc/>
    public override IToken Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException("Tokens cannot be deserialized because they are protected (obscure) data."
                                         + " Use model binding instead.");

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, IToken value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.GetProtected());
}
