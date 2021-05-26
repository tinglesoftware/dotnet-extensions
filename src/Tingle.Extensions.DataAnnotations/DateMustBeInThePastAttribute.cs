namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies that a data field value is a date in the past.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DateMustBeInThePastAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateMustBeInThePastAttribute"/> class.
        /// </summary>
        public DateMustBeInThePastAttribute() : base("The field {0} must be a date in the past.") { }

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
            // if the value is a valid date type and is in the future, validation fails
            if ((value is DateTimeOffset dto && dto > DateTimeOffset.UtcNow)
                || (value is DateTime dt && dt > DateTime.UtcNow))
            {
                return new ValidationResult(errorMessage: FormatErrorMessage(name: validationContext.DisplayName),
                                            memberNames: new string[] { validationContext.MemberName });
            }

            // any other value type just passes
            return ValidationResult.Success;
        }
    }
}
