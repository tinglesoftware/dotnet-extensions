namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies that a data field value ends with a specified string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class SuffixAttribute : ValidationAttribute
    {
        private readonly string suffix;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuffixAttribute"/> class.
        /// </summary>
        /// <param name="suffix">the suffix to check</param>
        public SuffixAttribute(string suffix) : base("The field {0} must end with '{1}'.")
        {
            this.suffix = suffix;
        }

        /// <summary>
        /// One of the enumeration values that determines how this string and value are compared.
        /// </summary>
        public StringComparison Comparison { get; set; } = StringComparison.CurrentCulture;

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
            if (s == null) return ValidationResult.Success;

            // check if the string ends with the suffix
            if (s.EndsWith(suffix, Comparison)) return ValidationResult.Success;

            // at this point, we can only fail
            return new ValidationResult(errorMessage: FormatErrorMessage(name: validationContext.DisplayName),
                                        memberNames: new string[] { validationContext.MemberName });
        }

        /// <summary>
        /// Formats the error message to display if the validation fails.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>The formatted error message.</returns>
        public override string FormatErrorMessage(string name) => string.Format(ErrorMessageString, name, suffix);
    }
}
