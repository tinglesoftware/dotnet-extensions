namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Indicates that a class or method is for internal use only.
/// </summary>
[AttributeUsage(
    // controllers; enum members; schemas (whole class/struct or properties); action methods, delegates (minimal APIs), and their parameters
    AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter,
    AllowMultiple = false,
    Inherited = true)]
public sealed class InternalOnlyAttribute : Attribute { }
