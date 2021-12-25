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
    public override bool IsValid(object? value) => value is not string s || string.IsNullOrEmpty(s) || regex.Match(s).Success;
}
