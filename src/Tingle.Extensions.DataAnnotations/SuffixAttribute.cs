namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value ends with a specified string.
/// </summary>
/// <param name="suffix">the suffix to check</param>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class SuffixAttribute(string suffix) : ValidationAttribute("The field {0} must end with '{1}'.")
{
    /// <summary>
    /// One of the enumeration values that determines how this string and value are compared.
    /// </summary>
    public StringComparison Comparison { get; set; } = StringComparison.CurrentCulture;

    /// <summary>
    /// Formats the error message to display if the validation fails.
    /// </summary>
    /// <param name="name">The name of the field that caused the validation failure.</param>
    /// <returns>The formatted error message.</returns>
    public override string FormatErrorMessage(string name) => string.Format(ErrorMessageString, name, suffix);

    /// <inheritdoc/>
    public override bool IsValid(object? value) => value is not string s || s == null || s.EndsWith(suffix, Comparison);
}
