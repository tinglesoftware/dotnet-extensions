using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Json
{
    /// <summary>
    /// A <see cref="JsonConverter{T}"/> for <see cref="TimeSpan"/>
    /// TODO: Remove once officially suported: https://github.com/dotnet/runtime/issues/29932
    /// </summary>
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        /// <inheritdoc/>
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                return TimeSpan.Parse(s);
            }

            return default;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue(value.ToString());
        }
    }
}
