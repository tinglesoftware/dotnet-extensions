using Tingle.Extensions.Primitives;

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value is a Currency.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class CurrencyAttribute : DataTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrencyAttribute"/> class.
    /// </summary>
    public CurrencyAttribute() : base(DataType.Currency)
    {
        ErrorMessage = "The field {0} must be a valid Currency.";
    }

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (value is string s && !string.IsNullOrEmpty(s)) return Currency.TryGetFromCode(s, out _);

        if (value is IEnumerable<string> values)
        {
            foreach (var v in values)
            {
                if (v is not string str || string.IsNullOrEmpty(str) || !Currency.TryGetFromCode(v, out _))
                    return false;
            }
        }

        return true;
    }
}
