using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace Tingle.AspNetCore.JsonPatch;

/// <summary>
/// Extensions for <see cref="JsonPatchMergeDocument{T}"/>
/// </summary>
public static class JsonPatchMergeDocumentExtensions
{
    /// <summary>
    /// Applies JSON patch operations on object and logs errors in <see cref="ModelStateDictionary"/> .
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="patchDoc">The <see cref="JsonPatchMergeDocument{TModel}"/>.</param>
    /// <param name="objectToApplyTo">The entity on which <see cref="JsonPatchMergeDocument{TModel}"/>  is applied.</param>
    /// <param name="modelState">The <see cref="ModelStateDictionary"/>  to add errors.</param>
    /// <param name="immutableProperties">The properties that are not allowed to changed</param>
    public static void ApplyToSafely<T>(this JsonPatchMergeDocument<T> patchDoc,
                                        T objectToApplyTo,
                                        ModelStateDictionary modelState,
                                        IEnumerable<string> immutableProperties)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(patchDoc);
        ArgumentNullException.ThrowIfNull(objectToApplyTo);
        ArgumentNullException.ThrowIfNull(modelState);
        ArgumentNullException.ThrowIfNull(immutableProperties);

        // if we get here, there are no changes to the immutable properties
        // we can thus proceed to apply the other properties
        patchDoc.ApplyToSafely(objectToApplyTo: objectToApplyTo,
                               modelState: modelState,
                               immutableProperties: immutableProperties,
                               prefix: string.Empty);
    }

    /// <summary>
    /// Applies JSON patch operations on object and logs errors in <see cref="ModelStateDictionary"/> .
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="patchDoc">The <see cref="JsonPatchMergeDocument{TModel}"/>.</param>
    /// <param name="objectToApplyTo">The entity on which <see cref="JsonPatchMergeDocument{TModel}"/>  is applied.</param>
    /// <param name="modelState">The <see cref="ModelStateDictionary"/>  to add errors.</param>
    /// <param name="prefix">The prefix to use when looking up values in <see cref="ModelStateDictionary"/>.</param>
    /// <param name="immutableProperties">The properties that are not allowed to changed</param>
    public static void ApplyToSafely<T>(this JsonPatchMergeDocument<T> patchDoc,
                                        T objectToApplyTo,
                                        ModelStateDictionary modelState,
                                        string prefix,
                                        IEnumerable<string> immutableProperties)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(patchDoc);
        ArgumentNullException.ThrowIfNull(objectToApplyTo);
        ArgumentNullException.ThrowIfNull(modelState);
        ArgumentNullException.ThrowIfNull(immutableProperties);

        // check each operation
        foreach (var op in patchDoc.Operations)
        {
            // only consider when the operation path is present
            if (!string.IsNullOrWhiteSpace(op.path))
            {
                var path = op.path.Trim('/').ToLowerInvariant();
                if (immutableProperties.Contains(path, StringComparer.OrdinalIgnoreCase))
                {
                    var affectedObjectName = objectToApplyTo.GetType().Name;
                    var key = string.IsNullOrEmpty(prefix) ? affectedObjectName : prefix + "." + affectedObjectName;
                    modelState.TryAddModelError(key, $"The property at path '{op.path}' is immutable.");
                    return;
                }
            }
        }

        // if we get here, there are no changes to the immutable properties
        // we can thus proceed to apply the other properties
        patchDoc.ApplyTo(objectToApplyTo: objectToApplyTo, modelState: modelState, prefix: prefix);
    }

    /// <summary>
    /// Applies JSON patch operations on object and logs errors in <see cref="ModelStateDictionary"/> .
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="patchDoc">The <see cref="JsonPatchMergeDocument{TModel}"/>.</param>
    /// <param name="objectToApplyTo">The entity on which <see cref="JsonPatchMergeDocument{TModel}"/>  is applied.</param>
    /// <param name="modelState">The <see cref="ModelStateDictionary"/>  to add errors.</param>
    public static void ApplyToSafely<T>(this JsonPatchMergeDocument<T> patchDoc,
                                        T objectToApplyTo,
                                        ModelStateDictionary modelState)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(patchDoc);
        ArgumentNullException.ThrowIfNull(objectToApplyTo);
        ArgumentNullException.ThrowIfNull(modelState);

        // if we get here, there are no changes to the immutable properties
        // we can thus proceed to apply the other properties
        patchDoc.ApplyToSafely(objectToApplyTo: objectToApplyTo, modelState: modelState, prefix: string.Empty);
    }

    /// <summary>
    /// Applies JSON patch operations on object and logs errors in <see cref="ModelStateDictionary"/> .
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="patchDoc">The <see cref="JsonPatchMergeDocument{TModel}"/>.</param>
    /// <param name="objectToApplyTo">The entity on which <see cref="JsonPatchMergeDocument{TModel}"/>  is applied.</param>
    /// <param name="modelState">The <see cref="ModelStateDictionary"/>  to add errors.</param>
    /// <param name="prefix">The prefix to use when looking up values in <see cref="ModelStateDictionary"/>.</param>
    public static void ApplyToSafely<T>(this JsonPatchMergeDocument<T> patchDoc,
                                        T objectToApplyTo,
                                        ModelStateDictionary modelState,
                                        string prefix)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(patchDoc);
        ArgumentNullException.ThrowIfNull(objectToApplyTo);
        ArgumentNullException.ThrowIfNull(modelState);

        var attrs = BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance;
        var properties = typeof(T).GetProperties(attrs).Select(p =>
        {
            var attr = p.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>();
            return attr?.Name ?? patchDoc.SerializerOptions.PropertyNamingPolicy?.ConvertName(p.Name) ?? p.Name;
        }).ToList();

        // check each operation
        foreach (var op in patchDoc.Operations)
        {
            // only consider when the operation path is present
            if (!string.IsNullOrWhiteSpace(op.path))
            {
                var segments = op.path.TrimStart('/').Split('/');
                var target = segments.First();
                if (!properties.Contains(target, StringComparer.OrdinalIgnoreCase))
                {
                    var key = string.IsNullOrEmpty(prefix) ? target : prefix + "." + target;
                    modelState.TryAddModelError(key, $"The property at path '{op.path}' is immutable or does not exist.");
                    return;
                }
            }
        }

        // if we get here, there are no changes to the immutable properties
        // we can thus proceed to apply the other properties
        patchDoc.ApplyTo(objectToApplyTo: objectToApplyTo, modelState: modelState, prefix: prefix);
    }

    /// <summary>
    /// Applies JSON patch operations on object and logs errors in <see cref="ModelStateDictionary"/>.
    /// </summary>
    /// <param name="patchDoc">The <see cref="JsonPatchMergeDocument{T}"/>.</param>
    /// <param name="objectToApplyTo">The entity on which <see cref="JsonPatchMergeDocument{T}"/> is applied.</param>
    /// <param name="modelState">The <see cref="ModelStateDictionary"/> to add errors.</param>
    public static void ApplyTo<T>(this JsonPatchMergeDocument<T> patchDoc, T objectToApplyTo, ModelStateDictionary modelState) where T : class
    {
        ArgumentNullException.ThrowIfNull(patchDoc);
        ArgumentNullException.ThrowIfNull(objectToApplyTo);
        ArgumentNullException.ThrowIfNull(modelState);

        patchDoc.ApplyTo(objectToApplyTo, modelState, prefix: string.Empty);
    }

    /// <summary>
    /// Applies JSON patch operations on object and logs errors in <see cref="ModelStateDictionary"/>.
    /// </summary>
    /// <param name="patchDoc">The <see cref="JsonPatchMergeDocument{T}"/>.</param>
    /// <param name="objectToApplyTo">The entity on which <see cref="JsonPatchMergeDocument{T}"/> is applied.</param>
    /// <param name="modelState">The <see cref="ModelStateDictionary"/> to add errors.</param>
    /// <param name="prefix">The prefix to use when looking up values in <see cref="ModelStateDictionary"/>.</param>
    public static void ApplyTo<T>(this JsonPatchMergeDocument<T> patchDoc, T objectToApplyTo, ModelStateDictionary modelState, string prefix) where T : class
    {
        ArgumentNullException.ThrowIfNull(patchDoc);
        ArgumentNullException.ThrowIfNull(objectToApplyTo);
        ArgumentNullException.ThrowIfNull(modelState);

        patchDoc.ApplyTo(objectToApplyTo, jsonPatchError =>
        {
            var affectedObjectName = jsonPatchError.AffectedObject.GetType().Name;
            var key = string.IsNullOrEmpty(prefix) ? affectedObjectName : prefix + "." + affectedObjectName;

            modelState.TryAddModelError(key, jsonPatchError.ErrorMessage);
        });
    }
}