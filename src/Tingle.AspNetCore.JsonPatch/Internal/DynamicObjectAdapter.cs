using Microsoft.CSharp.RuntimeBinder;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Tingle.AspNetCore.JsonPatch.Properties;

namespace Tingle.AspNetCore.JsonPatch.Internal;

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
public class DynamicObjectAdapter : IAdapter
{
    public virtual bool TryAdd(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        object? value,
        out string? errorMessage)
    {
        if (!TrySetDynamicObjectProperty(target, serializerOptions, segment, value, out errorMessage))
        {
            return false;
        }

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
        if (TryGetDynamicObjectProperty(target, serializerOptions, segment, out nextTarget, out errorMessage))
        {
            return true;
        }

        nextTarget = new ExpandoObject();
        if (!TrySetDynamicObjectProperty(target, serializerOptions, segment, nextTarget, out errorMessage))
        {
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
        if (!TryGetDynamicObjectProperty(target, serializerOptions, segment, out value, out errorMessage))
        {
            value = null;
            return false;
        }

        errorMessage = null;
        return true;
    }

    public virtual bool TryRemove(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        out string? errorMessage)
    {
        if (!TryGetDynamicObjectProperty(target, serializerOptions, segment, out var property, out errorMessage))
        {
            return false;
        }

        // Setting the value to "null" will use the default value in case of value types, and
        // null in case of reference types
        object? value = null;
        if (property.GetType().IsValueType
            && Nullable.GetUnderlyingType(property.GetType()) == null)
        {
            value = Activator.CreateInstance(property.GetType());
        }

        if (!TrySetDynamicObjectProperty(target, serializerOptions, segment, value, out errorMessage))
        {
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
        if (!TryGetDynamicObjectProperty(target, serializerOptions, segment, out var property, out errorMessage))
        {
            return false;
        }

        if (!TryConvertValue(value, property.GetType(), serializerOptions, out var convertedValue))
        {
            errorMessage = Resources.FormatInvalidValueForProperty(value);
            return false;
        }

        if (!TryRemove(target, segment, serializerOptions, out errorMessage))
        {
            return false;
        }

        if (!TrySetDynamicObjectProperty(target, serializerOptions, segment, convertedValue, out errorMessage))
        {
            return false;
        }

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
        if (!TryGetDynamicObjectProperty(target, serializerOptions, segment, out var property, out errorMessage))
        {
            return false;
        }

        if (!TryConvertValue(value, property.GetType(), serializerOptions, out var convertedValue))
        {
            errorMessage = Resources.FormatInvalidValueForProperty(value);
            return false;
        }

        var comparer = new JsonElementComparer();
        if (!comparer.Equals(JsonDocument.Parse(JsonSerializer.Serialize(property, serializerOptions)).RootElement, JsonDocument.Parse(JsonSerializer.Serialize(convertedValue, serializerOptions)).RootElement))
        {
            errorMessage = Resources.FormatValueNotEqualToTestValue(property, value, segment);
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
        if (!TryGetDynamicObjectProperty(target, serializerOptions, segment, out var property, out errorMessage))
        {
            nextTarget = null;
            return false;
        }

        nextTarget = property;
        errorMessage = null;
        return true;
    }

    protected virtual bool TryGetDynamicObjectProperty(
        object target,
        JsonSerializerOptions serializerOptions,
        string segment,
        [NotNullWhen(true)] out object? value,
        out string? errorMessage)
    {
        var propertyName = serializerOptions.PropertyNamingPolicy?.ConvertName(segment) ?? segment;

        var binder = Binder.GetMember(
            CSharpBinderFlags.None,
            propertyName,
            target.GetType(),
            [CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)]);

        var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);

        try
        {
            value = callsite.Target(callsite, target);
            errorMessage = null;
            return true;
        }
        catch (RuntimeBinderException)
        {
            value = null;
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }
    }

    protected virtual bool TrySetDynamicObjectProperty(
        object target,
        JsonSerializerOptions serializerOptions,
        string segment,
        object? value,
        out string? errorMessage)
    {
        var propertyName = serializerOptions.PropertyNamingPolicy?.ConvertName(segment) ?? segment;

        var binder = Binder.SetMember(
            CSharpBinderFlags.None,
            propertyName,
            target.GetType(),
            [
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
            ]);

        var callsite = CallSite<Func<CallSite, object, object?, object>>.Create(binder);

        try
        {
            callsite.Target(callsite, target, value);
            errorMessage = null;
            return true;
        }
        catch (RuntimeBinderException)
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }
    }

    protected virtual bool TryConvertValue(object? value, Type propertyType, JsonSerializerOptions serializerOptions, out object? convertedValue)
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
