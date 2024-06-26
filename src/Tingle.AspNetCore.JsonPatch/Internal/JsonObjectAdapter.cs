﻿using System.Text.Json;
using System.Text.Json.Nodes;
using Tingle.AspNetCore.JsonPatch.Properties;

namespace Tingle.AspNetCore.JsonPatch.Internal;

public class JsonObjectAdapter : IAdapter
{
    public virtual bool TryAdd(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        object? value,
        out string? errorMessage)
    {
        var obj = (JsonObject)target;

        obj[segment] = value != null ? JsonSerializer.SerializeToNode(value) : null;

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
        var obj = (JsonObject)target;

        if (obj.TryGetPropertyValue(segment, out var valueAsNode))
        {
            nextTarget = valueAsNode;
            errorMessage = null;
            return true;
        }

        nextTarget = obj[segment] = new JsonObject();
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
        var obj = (JsonObject)target;

        if (!obj.TryGetPropertyValue(segment, out var valueAsNode))
        {
            value = null;
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        value = valueAsNode;
        errorMessage = null;
        return true;
    }

    public virtual bool TryRemove(
        object target,
        string segment,
        JsonSerializerOptions serializerOptions,
        out string? errorMessage)
    {
        var obj = (JsonObject)target;

        if (!obj.Remove(segment))
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
        var obj = (JsonObject)target;

        if (!obj.ContainsKey(segment))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        obj[segment] = value != null ? JsonValue.Create(value) : JsonValue.Create<object>(null);

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
        var obj = (JsonObject)target;

        if (!obj.TryGetPropertyValue(segment, out var currentValue))
        {
            errorMessage = Resources.FormatTargetLocationAtPathSegmentNotFound(segment);
            return false;
        }

        if (currentValue == null || string.IsNullOrEmpty(currentValue.ToString()))
        {
            errorMessage = Resources.FormatValueForTargetSegmentCannotBeNullOrEmpty(segment);
            return false;
        }

        var comparer = new JsonElementComparer();
        if (!comparer.Equals(JsonDocument.Parse(JsonSerializer.Serialize(currentValue, serializerOptions)).RootElement, JsonDocument.Parse(JsonSerializer.Serialize(value, serializerOptions)).RootElement))
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
        var obj = (JsonObject)target;

        if (!obj.TryGetPropertyValue(segment, out var nextTargetNode))
        {
            nextTarget = null;
            errorMessage = null;
            return false;
        }

        nextTarget = nextTargetNode;
        errorMessage = null;
        return true;
    }
}
