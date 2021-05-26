using System.Text.RegularExpressions;
using Tingle.Extensions.PhoneValidators.Safaricom;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies that a data field value is a well-formed Safaricom phone number.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class SafaricomPhoneNumberAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafaricomPhoneNumberAttribute"/> class.
        /// </summary>
        public SafaricomPhoneNumberAttribute() : base("The field {0} must be a valid Safaricom phone number.") { }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <param name="validationContext">
        /// The <see cref="ValidationContext"/> object that describes
        /// the context where the validation checks are performed. This parameter cannot
        /// be null.
        /// </param>
        /// <exception cref="ValidationException">Validation failed.</exception>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // convert the object to a string and ensure that it is not null
            if (!(value is string s)) return ValidationResult.Success;
            if (string.IsNullOrEmpty(s)) return ValidationResult.Success;

            // check if phone number matches RegEx
            var regex = new Regex(SafaricomPhoneNumberValidator.RegExComplete);
            if (regex.Match(s).Success)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(errorMessage: FormatErrorMessage(name: validationContext.DisplayName),
                                            memberNames: new string[] { validationContext.MemberName });
            }
        }

    }
}
