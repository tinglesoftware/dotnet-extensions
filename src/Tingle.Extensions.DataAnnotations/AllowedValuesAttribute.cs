#if !NET8_0_OR_GREATER
using System.Collections;

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value is allowed. When applied on an array, all its elements must be allowed.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class AllowedValuesAttribute(IEnumerable<object> allowedValues) : ValidationAttribute("The field {0} only permits: {1}.")
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AllowedValuesAttribute"/> class.
    /// </summary>
    public AllowedValuesAttribute(params object[] allowedValues) : this(allowedValues.AsEnumerable()) { }

    /// <summary>
    /// An equality comparer to compare values.
    /// Defaults to <see cref="EqualityComparer{T}.Default"/>.
    /// </summary>
    public IEqualityComparer<object> Comparer { get; set; } = EqualityComparer<object>.Default;

    /// <summary>
    /// Formats the error message to display if the validation fails.
    /// </summary>
    /// <param name="name">The name of the field that caused the validation failure.</param>
    /// <returns>The formatted error message.</returns>
    public override string FormatErrorMessage(string name) => string.Format(ErrorMessageString, name, string.Join(",", allowedValues));

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (value is null) return true;

        // if the value is an enumerable, create values from each, otherwise its just the value
        var values = value is not string && value is IEnumerable ie
            ? ie.Cast<object>().ToList()
            : [value];

        // find the values not allowed
        var unknown = values.Where(o => !allowedValues.Contains(o, comparer: Comparer))
                            .ToList();

        // succeed only if there no unknown values
        return !unknown.Any();
    }
}
#endif
