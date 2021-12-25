namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that the object should not be validated recursively.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class SkipRecursiveValidationAttribute : Attribute
{
}
