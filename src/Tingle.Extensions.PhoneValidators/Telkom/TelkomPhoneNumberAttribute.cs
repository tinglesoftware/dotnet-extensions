using System.Text.RegularExpressions;
using Tingle.Extensions.PhoneValidators.Telkom;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies that a data field value is a well-formed Telkom phone number.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class TelkomPhoneNumberAttribute : ValidationAttribute
    {
        private static readonly Regex regex = new(TelkomPhoneNumberValidator.RegExComplete);

        /// <summary>
        /// Initializes a new instance of the <see cref="SafaricomPhoneNumberAttribute"/> class.
        /// </summary>
        public TelkomPhoneNumberAttribute() : base("The field {0} must be a valid Telkom phone number.") { }

        /// <inheritdoc/>
        public override bool IsValid(object? value) => value is not string s || string.IsNullOrEmpty(s) || regex.Match(s).Success;
    }
}
