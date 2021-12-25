using System.Text.RegularExpressions;
using Tingle.Extensions.PhoneValidators.Safaricom;

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value is a well-formed Safaricom phone number.
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
    public override bool IsValid(object? value) => value is not string s || string.IsNullOrEmpty(s) || regex.Match(s).Success;
}
