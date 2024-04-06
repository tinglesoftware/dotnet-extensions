using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Tingle.AspNetCore.JsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public static class ConversionResultProvider
{
    internal static ConversionResult ConvertTo(object? value, Type typeToConvertTo, JsonSerializerOptions serializerOptions)
    {
        if (value == null) return new ConversionResult(IsNullableType(typeToConvertTo), null);

        // If already in the type, not conversion required
        if (typeToConvertTo.IsAssignableFrom(value.GetType()))
        {
            // No need to convert
            return new ConversionResult(true, value);
        }

        // If type conversion can work, it is faster
        var converter = TypeDescriptor.GetConverter(typeToConvertTo);
        if (converter.CanConvertFrom(value.GetType()))
        {
            try
            {
                var converted = converter.ConvertFrom(value);
                return new ConversionResult(true, converted);
            }
            catch { } // fall back to next conversion mechanism
        }

        // Lastly, try out
        try
        {
            object? deserialized;
            if (typeToConvertTo == typeof(JsonNode)) deserialized = JsonSerializer.SerializeToNode(value, serializerOptions);
            else if (typeToConvertTo == typeof(JsonDocument)) deserialized = JsonSerializer.SerializeToDocument(value, serializerOptions);
            else if (typeToConvertTo == typeof(JsonElement)) deserialized = JsonSerializer.SerializeToElement(value, serializerOptions);
            else deserialized = JsonSerializer.Deserialize(JsonSerializer.Serialize(value, serializerOptions), typeToConvertTo, serializerOptions);
            return new ConversionResult(true, deserialized);
        }
        catch
        {
            return new ConversionResult(canBeConverted: false, convertedInstance: null);
        }
    }

    public static ConversionResult CopyTo(object? value, Type typeToConvertTo)
    {
        var targetType = typeToConvertTo;
        if (value == null)
        {
            return new ConversionResult(canBeConverted: true, convertedInstance: null);
        }
        else if (typeToConvertTo.IsAssignableFrom(value.GetType()))
        {
            // Keep original type
            targetType = value.GetType();
        }
        try
        {
            var deserialized = JsonSerializer.Deserialize(json: JsonSerializer.Serialize(value: value), returnType: targetType);
            return new ConversionResult(true, deserialized);
        }
        catch
        {
            return new ConversionResult(canBeConverted: false, convertedInstance: null);
        }
    }

    private static bool IsNullableType(Type type)
    {
        if (type.IsValueType)
        {
            // value types are only nullable if they are Nullable<T>
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        else
        {
            // reference types are always nullable
            return true;
        }
    }
}
