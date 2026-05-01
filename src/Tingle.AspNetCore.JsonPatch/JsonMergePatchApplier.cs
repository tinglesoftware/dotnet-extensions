using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;

namespace Tingle.AspNetCore.JsonPatch;

internal static class JsonMergePatchApplier
{
    internal static void Apply(object patchDocument, object objectToApplyTo, JsonSerializerOptions serializerOptions, Action<JsonPatchError>? logErrorAction = null)
    {
        ArgumentNullException.ThrowIfNull(patchDocument);
        ArgumentNullException.ThrowIfNull(objectToApplyTo);
        ArgumentNullException.ThrowIfNull(serializerOptions);

        try
        {
            var patchNode = JsonSerializer.SerializeToNode(patchDocument, patchDocument.GetType(), serializerOptions);
            if (patchNode is not JsonObject patchObject)
            {
                throw new InvalidOperationException("Only objects are supported.");
            }

            var targetNode = JsonSerializer.SerializeToNode(objectToApplyTo, objectToApplyTo.GetType(), serializerOptions);
            var mergedNode = Merge(targetNode, patchObject);
            var mergedObject = mergedNode?.Deserialize(objectToApplyTo.GetType(), serializerOptions)
                               ?? throw new InvalidOperationException("Unable to materialize merge patch result.");

            CopyValues(mergedObject, objectToApplyTo);
        }
        catch (Exception ex) when (logErrorAction is not null)
        {
            logErrorAction(new JsonPatchError(objectToApplyTo, new Operation { op = "replace", path = "/" }, ex.Message));
        }
    }

    private static JsonNode? Merge(JsonNode? target, JsonNode? patch)
    {
        if (patch is JsonObject patchObject)
        {
            var result = target as JsonObject;
            result = result is null ? new JsonObject() : (JsonObject)result.DeepClone();

            foreach (var pair in patchObject)
            {
                if (pair.Value is null)
                {
                    result.Remove(pair.Key);
                    continue;
                }

                result[pair.Key] = Merge(result[pair.Key], pair.Value);
            }

            return result;
        }

        return patch?.DeepClone();
    }

    private static void CopyValues(object source, object destination)
    {
        if (ReferenceEquals(source, destination)) return;

        if (source is IDictionary sourceDict && destination is IDictionary destinationDict)
        {
            destinationDict.Clear();
            foreach (DictionaryEntry pair in sourceDict)
            {
                destinationDict[pair.Key] = pair.Value;
            }

            return;
        }

        if (source is IList sourceList &&
            destination is IList destinationList &&
            !destination.GetType().IsArray)
        {
            destinationList.Clear();
            foreach (var value in sourceList)
            {
                destinationList.Add(value);
            }

            return;
        }

        var properties = destination.GetType()
                                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                    .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            var sourceValue = property.GetValue(source);
            if (property.CanWrite)
            {
                property.SetValue(destination, sourceValue);
                continue;
            }

            if (sourceValue is null || IsSimpleType(property.PropertyType)) continue;

            var destinationValue = property.GetValue(destination);
            if (destinationValue is null) continue;

            CopyValues(sourceValue, destinationValue);
        }
    }

    private static bool IsSimpleType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        return type.IsPrimitive ||
               type.IsEnum ||
               type == typeof(string) ||
               type == typeof(decimal) ||
               type == typeof(DateTime) ||
               type == typeof(DateTimeOffset) ||
               type == typeof(Guid) ||
               type == typeof(TimeSpan);
    }
}
