using Tingle.Extensions.Primitives;

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value is a Language.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class LanguageCodeAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LanguageCodeAttribute"/> class.
    /// </summary>
    public LanguageCodeAttribute() : base("The field {0} must be a valid Language.") { }

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (value is string s && !string.IsNullOrEmpty(s)) return Language.TryGetFromCode(s, out _);

        if (value is IEnumerable<string> values)
        {
            foreach (var v in values)
            {
                if (v is not string str || string.IsNullOrEmpty(str) || !Language.TryGetFromCode(v, out _))
                    return false;
            }
        }

        return true;
    }
}
