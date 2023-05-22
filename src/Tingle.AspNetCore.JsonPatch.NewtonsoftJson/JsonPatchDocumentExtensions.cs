using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// Extension methods for <see cref="JsonPatchDocument{TModel}"/>
/// </summary>
public static class JsonPatchDocumentExtensions
{
    /// <summary>
    /// Applies JSON patch operations on object and logs errors in <see cref="ModelStateDictionary"/> .
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="patchDoc">The <see cref="JsonPatchDocument{TModel}"/>.</param>
    /// <param name="objectToApplyTo">The entity on which <see cref="JsonPatchDocument{TModel}"/>  is applied.</param>
    /// <param name="modelState">The <see cref="ModelStateDictionary"/>  to add errors.</param>
    /// <param name="immutableProperties">The properties that are not allowed to changed</param>
    public static void ApplyToSafely<T>(this JsonPatchDocument<T> patchDoc,
                                        T objectToApplyTo,
                                        ModelStateDictionary modelState,
                                        IEnumerable<string> immutableProperties)
        where T : class
    {
        if (patchDoc == null) throw new ArgumentNullException(nameof(patchDoc));
        if (objectToApplyTo == null) throw new ArgumentNullException(nameof(objectToApplyTo));
        if (modelState == null) throw new ArgumentNullException(nameof(modelState));
        if (immutableProperties == null) throw new ArgumentNullException(nameof(immutableProperties));

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
    /// <param name="patchDoc">The <see cref="JsonPatchDocument{TModel}"/>.</param>
    /// <param name="objectToApplyTo">The entity on which <see cref="JsonPatchDocument{TModel}"/>  is applied.</param>
    /// <param name="modelState">The <see cref="ModelStateDictionary"/>  to add errors.</param>
    /// <param name="prefix">The prefix to use when looking up values in <see cref="ModelStateDictionary"/>.</param>
    /// <param name="immutableProperties">The properties that are not allowed to changed</param>
    public static void ApplyToSafely<T>(this JsonPatchDocument<T> patchDoc,
                                        T objectToApplyTo,
                                        ModelStateDictionary modelState,
                                        string prefix,
                                        IEnumerable<string> immutableProperties)
        where T : class
    {
        if (patchDoc == null) throw new ArgumentNullException(nameof(patchDoc));
        if (objectToApplyTo == null) throw new ArgumentNullException(nameof(objectToApplyTo));
        if (modelState == null) throw new ArgumentNullException(nameof(modelState));
        if (immutableProperties == null) throw new ArgumentNullException(nameof(immutableProperties));

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
    /// <param name="patchDoc">The <see cref="JsonPatchDocument{TModel}"/>.</param>
    /// <param name="objectToApplyTo">The entity on which <see cref="JsonPatchDocument{TModel}"/>  is applied.</param>
    /// <param name="modelState">The <see cref="ModelStateDictionary"/>  to add errors.</param>
    public static void ApplyToSafely<T>(this JsonPatchDocument<T> patchDoc,
                                        T objectToApplyTo,
                                        ModelStateDictionary modelState)
        where T : class
    {
        if (patchDoc == null) throw new ArgumentNullException(nameof(patchDoc));
        if (objectToApplyTo == null) throw new ArgumentNullException(nameof(objectToApplyTo));
        if (modelState == null) throw new ArgumentNullException(nameof(modelState));

        // if we get here, there are no changes to the immutable properties
        // we can thus proceed to apply the other properties
        patchDoc.ApplyToSafely(objectToApplyTo: objectToApplyTo, modelState: modelState, prefix: string.Empty);
    }

    /// <summary>
    /// Applies JSON patch operations on object and logs errors in <see cref="ModelStateDictionary"/> .
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="patchDoc">The <see cref="JsonPatchDocument{TModel}"/>.</param>
    /// <param name="objectToApplyTo">The entity on which <see cref="JsonPatchDocument{TModel}"/>  is applied.</param>
    /// <param name="modelState">The <see cref="ModelStateDictionary"/>  to add errors.</param>
    /// <param name="prefix">The prefix to use when looking up values in <see cref="ModelStateDictionary"/>.</param>
    public static void ApplyToSafely<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        this JsonPatchDocument<T> patchDoc,
        T objectToApplyTo,
        ModelStateDictionary modelState,
        string prefix) where T : class
    {
        if (patchDoc == null) throw new ArgumentNullException(nameof(patchDoc));
        if (objectToApplyTo == null) throw new ArgumentNullException(nameof(objectToApplyTo));
        if (modelState == null) throw new ArgumentNullException(nameof(modelState));

        var attrs = BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance;
        var properties = typeof(T).GetProperties(attrs).Select(p =>
        {
            var attr = p.GetCustomAttribute<Newtonsoft.Json.JsonPropertyAttribute>();
            var dcc = patchDoc.ContractResolver as DefaultContractResolver;
            return attr?.PropertyName ?? dcc?.GetResolvedPropertyName(p.Name) ?? p.Name;
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
}
