using System.Text.Json;

namespace Tingle.Extensions.JsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public static class ConversionResultProvider
{
    public static ConversionResult ConvertTo(object value, Type typeToConvertTo)
    {
        if (value == null)
        {
            return new ConversionResult(IsNullableType(typeToConvertTo), null);
        }
        else if (typeToConvertTo.IsAssignableFrom(value.GetType()))
        {
            // No need to convert
            return new ConversionResult(true, value);
        }
        else
        {
            try
            {
                var deserialized = JsonSerializer.Deserialize(json: JsonSerializer.Serialize(value: value), returnType: typeToConvertTo);
                return new ConversionResult(true, deserialized);
            }
            catch
            {
                return new ConversionResult(canBeConverted: false, convertedInstance: null);
            }
        }
    }

    public static ConversionResult CopyTo(object value, Type typeToConvertTo)
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
