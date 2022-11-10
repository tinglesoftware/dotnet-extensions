using System.Collections;
using System.Reflection;

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Defines a helper class that can be used to validate objects, properties, and methods recursively
/// when it is included in their associated <see cref="ValidationAttribute"/> attributes.
/// </summary>
public static class RecursiveValidator
{
    /// <summary>
    /// Determines whether the specified object and its children are valid using the validation context and
    /// validation results collection.
    /// </summary>
    /// <param name="instance">The object to validate.</param>
    /// <param name="validationResults">A collection to hold each failed validation.</param>
    /// <param name="validationContextItems">
    /// A dictionary of key/value pairs to make available to the service consumers. This parameter is optional.
    /// </param>
    /// <returns>true if the object validates; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">instance is null.</exception>
    public static bool TryValidateObject(object instance,
                                         ICollection<ValidationResult> validationResults,
                                         IDictionary<object, object?>? validationContextItems = null)
    {
        var context = new ValidationContext(instance, null, validationContextItems);
        return Validator.TryValidateObject(instance: instance,
                                           validationContext: context,
                                           validationResults: validationResults,
                                           validateAllProperties: true);
    }

    /// <summary>
    /// Determines whether the specified object and its children are valid using the validation context.
    /// </summary>
    /// <param name="instance">The object to validate.</param>
    /// <param name="validationContextItems">
    /// A dictionary of key/value pairs to make available to the service consumers. This parameter is optional.
    /// </param>
    /// <exception cref="ValidationException">instance is not valid.</exception>
    /// <exception cref="ArgumentNullException">instance is null.</exception>
    public static void ValidateObject(object instance, IDictionary<object, object?>? validationContextItems = null)
    {
        var context = new ValidationContext(instance, null, validationContextItems);
        Validator.ValidateObject(instance: instance,
                                 validationContext: context,
                                 validateAllProperties: true);
    }

    /// <summary>
    /// Determines whether the specified object and its children are valid using the validation context and
    /// validation results collection.
    /// </summary>
    /// <param name="instance">The object to validate.</param>
    /// <param name="validationResults">A collection to hold each failed validation.</param>
    /// <param name="validationContextItems">
    /// A dictionary of key/value pairs to make available to the service consumers. This parameter is optional.
    /// </param>
    /// <returns>true if the object validates; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">instance is null.</exception>
    public static bool TryValidateObjectRecursive(object instance,
                                                  ICollection<ValidationResult> validationResults,
                                                  IDictionary<object, object?>? validationContextItems = null)
    {
        return TryValidateObjectRecursive(instance: instance,
                                          validationResults: validationResults,
                                          validatedObjects: new HashSet<object>(),
                                          validationContextItems: validationContextItems);
    }

    /// <summary>
    /// Determines whether the specified object and its children are valid using the validation context.
    /// </summary>
    /// <param name="instance">The object to validate.</param>
    /// <param name="validationContextItems">
    /// A dictionary of key/value pairs to make available to the service consumers. This parameter is optional.
    /// </param>
    /// <exception cref="ValidationException">instance is not valid.</exception>
    /// <exception cref="ArgumentNullException">instance is null.</exception>
    public static void ValidateObjectRecursive(object instance, IDictionary<object, object?>? validationContextItems = null)
    {
        ValidateObjectRecursive(instance: instance,
                                validatedObjects: new HashSet<object>(),
                                validationContextItems: validationContextItems);
    }

    private static bool TryValidateObjectRecursive(object instance,
                                                   ICollection<ValidationResult> validationResults,
                                                   ISet<object> validatedObjects,
                                                   IDictionary<object, object?>? validationContextItems = null)
    {
        // short-circuit to avoid infinite loops on cyclical object graphs
        if (validatedObjects.Contains(instance))
        {
            return true;
        }

        validatedObjects.Add(instance);
        bool result = TryValidateObject(instance, validationResults, validationContextItems);

        var properties = instance.GetType().GetProperties().Where(prop => prop.CanRead
            && !prop.GetCustomAttributes(typeof(SkipRecursiveValidationAttribute), false).Any()
            && prop.GetIndexParameters().Length == 0).ToList();

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType) continue;

            var value = instance.GetPropertyValue(property.Name);

            if (value == null) continue;

            if (value is IEnumerable asEnumerable)
            {
                foreach (var enumObj in asEnumerable)
                {
                    if (enumObj != null)
                    {
                        var nestedResults = new List<ValidationResult>();
                        if (!TryValidateObjectRecursive(enumObj, nestedResults, validatedObjects, validationContextItems))
                        {
                            result = false;
                            foreach (var validationResult in nestedResults)
                            {
                                PropertyInfo property1 = property;
                                validationResults.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => property1.Name + '.' + x)));
                            }
                        };
                    }
                }
            }
            else
            {
                var nestedResults = new List<ValidationResult>();
                if (!TryValidateObjectRecursive(value, nestedResults, validatedObjects, validationContextItems))
                {
                    result = false;
                    foreach (var validationResult in nestedResults)
                    {
                        PropertyInfo property1 = property;
                        validationResults.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => property1.Name + '.' + x)));
                    }
                };
            }
        }

        return result;
    }

    private static void ValidateObjectRecursive(object instance,
                                                ISet<object> validatedObjects,
                                                IDictionary<object, object?>? validationContextItems = null)
    {
        // short-circuit to avoid infinite loops on cyclical object graphs
        if (validatedObjects.Contains(instance))
        {
            return;
        }

        validatedObjects.Add(instance);
        ValidateObject(instance, validationContextItems);

        var properties = instance.GetType().GetProperties().Where(prop => prop.CanRead
            && !prop.GetCustomAttributes(typeof(SkipRecursiveValidationAttribute), false).Any()
            && prop.GetIndexParameters().Length == 0).ToList();

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType) continue;

            var value = instance.GetPropertyValue(property.Name);

            if (value == null) continue;

            if (value is IEnumerable asEnumerable)
            {
                foreach (var enumObj in asEnumerable)
                {
                    if (enumObj != null)
                    {
                        ValidateObjectRecursive(enumObj, validatedObjects, validationContextItems);
                    }
                }
            }
            else
            {
                ValidateObjectRecursive(value, validatedObjects, validationContextItems);
            }
        }
    }

}
internal static class ObjectExtensions
{
    public static object GetPropertyValue(this object o, string propertyName)
    {
        var propertyInfo = o.GetType().GetProperty(propertyName);
        return propertyInfo?.GetValue(o, null) ?? string.Empty;
    }
}
