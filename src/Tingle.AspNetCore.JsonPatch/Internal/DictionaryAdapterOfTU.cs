using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Tingle.AspNetCore.JsonPatch.Properties;

namespace Tingle.AspNetCore.JsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public class DictionaryAdapter<TKey, TValue> : IAdapter
{
    public virtual bool TryAdd(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        object? value,
        out string? errorMessage)
    {
        var key = MakeKeyFromSegment(target, segment, serializerOptions);
        var dictionary = (IDictionary<TKey, TValue?>)target;

        // As per JsonPatch spec, if a key already exists, adding should replace the existing value
        if (!TryConvertKey(key, serializerOptions, out var convertedKey, out errorMessage))
        {
            return false;
        }

        if (!TryConvertValue(value, serializerOptions, out var convertedValue, out errorMessage))
        {
            return false;
        }

        dictionary[convertedKey] = convertedValue;
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
        var key = MakeKeyFromSegment(target, segment, serializerOptions);
        var dictionary = (IDictionary<TKey, TValue?>)target;

        if (!TryConvertKey(key, serializerOptions, out var convertedKey, out errorMessage))
        {
            nextTarget = null;
            return false;
        }

        if (dictionary.ContainsKey(convertedKey))
        {
            nextTarget = null;
            return true;
        }

        try
        {
            nextTarget = dictionary[convertedKey] = Activator.CreateInstance<TValue>();
        }
        catch (Exception)
        {
            nextTarget = null;
            return false;
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
        var key = MakeKeyFromSegment(target, segment, serializerOptions);
        var dictionary = (IDictionary<TKey, TValue?>)target;

        if (!TryConvertKey(key, serializerOptions, out var convertedKey, out errorMessage))
        {
            value = null;
            return false;
        }

        if (!dictionary.TryGetValue(convertedKey, out var valueAsT))
        {
            value = null;
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        value = valueAsT;
        errorMessage = null;
        return true;
    }

    public virtual bool TryRemove(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        out string? errorMessage)
    {
        var key = MakeKeyFromSegment(target, segment, serializerOptions);
        var dictionary = (IDictionary<TKey, TValue?>)target;

        if (!TryConvertKey(key, serializerOptions, out var convertedKey, out errorMessage))
        {
            return false;
        }

        // As per JsonPatch spec, the target location must exist for remove to be successful
        if (!dictionary.Remove(convertedKey))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

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
        var key = MakeKeyFromSegment(target, segment, serializerOptions);
        var dictionary = (IDictionary<TKey, TValue?>)target;

        if (!TryConvertKey(key, serializerOptions, out var convertedKey, out errorMessage))
        {
            return false;
        }

        // As per JsonPatch spec, the target location must exist for remove to be successful
        if (!dictionary.ContainsKey(convertedKey))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        if (!TryConvertValue(value, serializerOptions, out var convertedValue, out errorMessage))
        {
            return false;
        }

        dictionary[convertedKey] = convertedValue;

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
        var key = MakeKeyFromSegment(target, segment, serializerOptions);
        var dictionary = (IDictionary<TKey, TValue?>)target;

        if (!TryConvertKey(key, serializerOptions, out var convertedKey, out errorMessage))
        {
            return false;
        }

        // As per JsonPatch spec, the target location must exist for test to be successful
        if (!dictionary.ContainsKey(convertedKey))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        if (!TryConvertValue(value, serializerOptions, out var convertedValue, out errorMessage))
        {
            return false;
        }

        var currentValue = dictionary[convertedKey];

        // The target segment does not have an assigned value to compare the test value with
        if (currentValue == null)
        {
            errorMessage = Resources.FormatValueForTargetSegmentCannotBeNullOrEmpty(segment);
            return false;
        }

        var comparer = new JsonElementComparer();
        if (!comparer.Equals(JsonDocument.Parse(JsonSerializer.Serialize(currentValue, serializerOptions)).RootElement, JsonDocument.Parse(JsonSerializer.Serialize(convertedValue, serializerOptions)).RootElement))
        {
            errorMessage = Resources.FormatValueNotEqualToTestValue(currentValue, value, segment);
            return false;
        }

        errorMessage = null;
        return true;
    }

    public virtual bool TryTraverse(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        out object? nextTarget,
        out string? errorMessage)
    {
        var key = MakeKeyFromSegment(target, segment, serializerOptions);
        var dictionary = (IDictionary<TKey, TValue?>)target;

        if (!TryConvertKey(key, serializerOptions, out var convertedKey, out errorMessage))
        {
            nextTarget = null;
            return false;
        }

        if (dictionary.TryGetValue(convertedKey, out var valueAsT))
        {
            nextTarget = valueAsT;
            errorMessage = null;
            return true;
        }

        nextTarget = null;
        errorMessage = null;
        return false;
    }

    protected virtual bool TryConvertKey(string key, JsonSerializerOptions serializerOptions, [NotNullWhen(true)] out TKey? convertedKey, out string? errorMessage)
    {
        var conversionResult = ConversionResultProvider.ConvertTo(key, typeof(TKey), serializerOptions);
        if (conversionResult.CanBeConverted)
        {
            errorMessage = null;
            convertedKey = (TKey)conversionResult.ConvertedInstance;
            return true;
        }

        errorMessage = Resources.FormatInvalidPathSegment(key);
        convertedKey = default;
        return false;
    }

    protected virtual bool TryConvertValue(object? value, JsonSerializerOptions serializerOptions, out TValue? convertedValue, out string? errorMessage)
    {
        var conversionResult = ConversionResultProvider.ConvertTo(value, typeof(TValue), serializerOptions);
        if (conversionResult.CanBeConverted)
        {
            errorMessage = null;
            convertedValue = (TValue)conversionResult.ConvertedInstance;
            return true;
        }

        errorMessage = Resources.FormatInvalidValueForProperty(value);
        convertedValue = default;
        return false;
    }

    private static string MakeKeyFromSegment(object target, string segment, JsonSerializerOptions serializerOptions)
    {
        return target is System.Dynamic.ExpandoObject
            ? serializerOptions.PropertyNamingPolicy?.ConvertName(segment) ?? segment
            : serializerOptions.DictionaryKeyPolicy?.ConvertName(segment) ?? segment;
    }
}
