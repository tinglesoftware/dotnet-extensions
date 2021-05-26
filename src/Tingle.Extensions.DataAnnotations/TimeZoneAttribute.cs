using TimeZoneConverter;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies that a data field value is a well-formed timezone identifier.
    /// This support both Windows and IANA timezone identifiers
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class TimeZoneAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZoneAttribute"/> class.
        /// </summary>
        public TimeZoneAttribute() : base("The field {0} must be a valid Windows or IANA TimeZone identifier.") { }

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

            // Validate the TimeZone
            if (TZConvert.TryGetTimeZoneInfo(windowsOrIanaTimeZoneId: s, timeZoneInfo: out _))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(errorMessage: FormatErrorMessage(name: validationContext.DisplayName),
                                            memberNames: new[] { validationContext.MemberName });
            }
        }
    }
}
