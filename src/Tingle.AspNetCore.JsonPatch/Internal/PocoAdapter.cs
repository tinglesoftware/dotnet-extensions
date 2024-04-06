using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using Tingle.AspNetCore.JsonPatch.Properties;

namespace Tingle.AspNetCore.JsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public class PocoAdapter : IAdapter
{
    public virtual bool TryAdd(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        object? value,
        out string? errorMessage)
    {
        if (!TryGetJsonProperty(target, serializerOptions, segment, out var jsonProperty))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        if (!jsonProperty.Writeable)
        {
            errorMessage = Resources.FormatCannotUpdateProperty(segment);
            return false;
        }

        if (!TryConvertValue(value, jsonProperty.PropertyType, serializerOptions, out var convertedValue))
        {
            errorMessage = Resources.FormatInvalidValueForProperty(value);
            return false;
        }

        jsonProperty.SetValue(target, convertedValue);

        errorMessage = null;
        return true;
    }

    public virtual bool TryCreate(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        out object? nextTarget,
        out string? errorMessage)
    {
        if (!TryGetJsonProperty(target, serializerOptions, segment, out var jsonProperty))
        {
            nextTarget = null;
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        if (!jsonProperty.Writeable)
        {
            nextTarget = null;
            errorMessage = Resources.FormatCannotUpdateProperty(segment);
            return false;
        }

        nextTarget = jsonProperty.GetValue(target);
        if (nextTarget is null)
        {
            nextTarget = Activator.CreateInstance(jsonProperty.PropertyType);
            jsonProperty.SetValue(target, nextTarget);
        }

        errorMessage = null;
        return true;
    }

    public virtual bool TryGet(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        out object? value,
        out string? errorMessage)
    {
        if (!TryGetJsonProperty(target, serializerOptions, segment, out var jsonProperty))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            value = null;
            return false;
        }

        if (!jsonProperty.Readable)
        {
            errorMessage = Resources.FormatCannotReadProperty(segment);
            value = null;
            return false;
        }

        value = jsonProperty.GetValue(target);
        errorMessage = null;
        return true;
    }

    public virtual bool TryRemove(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        out string? errorMessage)
    {
        if (!TryGetJsonProperty(target, serializerOptions, segment, out var jsonProperty))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        if (!jsonProperty.Writeable)
        {
            errorMessage = Resources.FormatCannotUpdateProperty(segment);
            return false;
        }

        jsonProperty.RemoveValue(target);

        errorMessage = null;
        return true;
    }

    public virtual bool TryReplace(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        object? value,
        out string? errorMessage)
    {
        if (!TryGetJsonProperty(target, serializerOptions, segment, out var jsonProperty))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        if (!jsonProperty.Writeable)
        {
            errorMessage = Resources.FormatCannotUpdateProperty(segment);
            return false;
        }

        if (!TryConvertValue(value, jsonProperty.PropertyType, serializerOptions, out var convertedValue))
        {
            errorMessage = Resources.FormatInvalidValueForProperty(value);
            return false;
        }

        jsonProperty.SetValue(target, convertedValue);

        errorMessage = null;
        return true;
    }

    public virtual bool TryTest(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        object? value,
        out string? errorMessage)
    {
        if (!TryGetJsonProperty(target, serializerOptions, segment, out var jsonProperty))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        if (!jsonProperty.Readable)
        {
            errorMessage = Resources.FormatCannotReadProperty(segment);
            return false;
        }

        if (!TryConvertValue(value, jsonProperty.PropertyType, serializerOptions, out var convertedValue))
        {
            errorMessage = Resources.FormatInvalidValueForProperty(value);
            return false;
        }

        var comparer = new JsonElementComparer();
        var currentValue = jsonProperty.GetValue(target);
        if (!comparer.Equals(JsonDocument.Parse(JsonSerializer.Serialize(currentValue, serializerOptions)).RootElement, JsonDocument.Parse(JsonSerializer.Serialize(convertedValue, serializerOptions)).RootElement))
        {
            errorMessage = Resources.FormatValueNotEqualToTestValue(JsonSerializer.SerializeToNode(currentValue), value, segment);
            return false;
        }

        errorMessage = null;
        return true;
    }

    public virtual bool TryTraverse(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        out object? value,
        out string? errorMessage)
    {
        if (target == null)
        {
            value = null;
            errorMessage = null;
            return false;
        }

        if (TryGetJsonProperty(target, serializerOptions, segment, out var jsonProperty))
        {
            value = jsonProperty.GetValue(target);
            errorMessage = null;
            return true;
        }

        value = null;
        errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
        return false;
    }

    protected virtual bool TryGetJsonProperty(
        object target,
        JsonSerializerOptions serializerOptions,
        string segment,
        [NotNullWhen(true)] out IPropertyProxy? jsonProperty)
    {
        return (jsonProperty = FindPropertyByName(segment, target, serializerOptions)) != null;
    }

    private static IPropertyProxy? FindPropertyByName(string name, object target, JsonSerializerOptions serializerOptions)
    {
        var propertyName = serializerOptions.PropertyNamingPolicy?.ConvertName(name) ?? name;

        if (target is JsonArray jsonArray)
        {
            return new JsonArrayProxy(jsonArray, propertyName);
        }

        if (target is JsonNode jsonElement)
        {
            return new JsonNodeProxy(jsonElement, propertyName);
        }

        return PropertyProxyCache.GetPropertyProxy(target.GetType(), propertyName, serializerOptions);
    }

    protected virtual bool TryConvertValue(object? value, Type propertyType, JsonSerializerOptions serializerOptions, [NotNullWhen(true)] out object? convertedValue)
    {
        var conversionResult = ConversionResultProvider.ConvertTo(value, propertyType, serializerOptions);
        if (!conversionResult.CanBeConverted)
        {
            convertedValue = null;
            return false;
        }

        convertedValue = conversionResult.ConvertedInstance;
        return true;
    }
}
