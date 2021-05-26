﻿using PhoneNumbers;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies that a data field value is a well-formed MSISDN phone number.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class MsisdnPhoneAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsisdnPhoneAttribute"/> class.
        /// </summary>
        public MsisdnPhoneAttribute() : base("The field {0} must be a valid MSISDN phone number.") { }

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

            // if the value already starts with a plus, we have failed
            if (s.StartsWith("+"))
            {
                return new ValidationResult(errorMessage: FormatErrorMessage(name: validationContext.DisplayName),
                                            memberNames: new string[] { validationContext.MemberName });
            }

            // attempt to parse phone number
            try
            {
                var prepended = "+" + s;
                var phoneNumberUtil = PhoneNumberUtil.GetInstance();
                _ = phoneNumberUtil.Parse(numberToParse: prepended, defaultRegion: null);
                return ValidationResult.Success;
            }
            catch (Exception ex) when (ex is NumberParseException)
            {
                return new ValidationResult(errorMessage: FormatErrorMessage(name: validationContext.DisplayName),
                                            memberNames: new string[] { validationContext.MemberName });
            }
        }
    }
}