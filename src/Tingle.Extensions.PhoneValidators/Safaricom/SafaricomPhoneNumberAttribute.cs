using System.Text.RegularExpressions;
using Tingle.Extensions.PhoneValidators.Safaricom;

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value is a well-formed Safaricom phone number or a list of well-formed Safaricom phone numbers.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class SafaricomPhoneNumberAttribute : ValidationAttribute
{
    private static readonly Regex regex = new(SafaricomPhoneNumberValidator.RegExComplete);

    /// <summary>
    /// Initializes a new instance of the <see cref="SafaricomPhoneNumberAttribute"/> class.
    /// </summary>
    public SafaricomPhoneNumberAttribute() : base("The field {0} must be a valid Safaricom phone number.") { }

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
