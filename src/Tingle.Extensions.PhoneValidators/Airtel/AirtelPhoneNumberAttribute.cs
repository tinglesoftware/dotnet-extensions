using System.Text.RegularExpressions;
using Tingle.Extensions.PhoneValidators.Airtel;

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value is a well-formed Airtel phone number.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class AirtelPhoneNumberAttribute : ValidationAttribute
{
    private static readonly Regex regex = new(AirtelPhoneNumberValidator.RegExComplete);

    /// <summary>
    /// Initializes a new instance of the <see cref="SafaricomPhoneNumberAttribute"/> class.
    /// </summary>
    public AirtelPhoneNumberAttribute() : base("The field {0} must be a valid Airtel phone number.") { }

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (value is string s && !string.IsNullOrEmpty(s)) return IsValidByRegEx(s);

        if (value is IEnumerable<string> values)
        {
            foreach (var v in values)
            {
                if (v is not string str || string.IsNullOrEmpty(str) || !IsValidByRegEx(v))
                    return false;
            }
        }

        return true;
    }

    private bool IsValidByRegEx(string value) => regex.IsMatch(value);
}
