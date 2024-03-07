using Tingle.Extensions.Primitives;

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value is a Country Code.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class CountryCodeAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CountryCodeAttribute"/> class.
    /// </summary>
    public CountryCodeAttribute() : base("The field {0} must be a valid Country Code.") { }

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (value is string s && !string.IsNullOrEmpty(s)) return Country.TryGetFromCode(s, out _);

        if (value is IEnumerable<string> values)
        {
            foreach (var v in values)
            {
                if (v is not string str || string.IsNullOrEmpty(str) || !Country.TryGetFromCode(v, out _))
                    return false;
            }
        }

        return true;
    }
}
