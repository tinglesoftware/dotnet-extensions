namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies that a data field value starts with a specified string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PrefixAttribute : ValidationAttribute
    {
        private readonly string prefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrefixAttribute"/> class.
        /// </summary>
        /// <param name="prefix">the prefix to check</param>
        public PrefixAttribute(string prefix) : base("The field {0} must start with '{1}'.")
        {
            this.prefix = prefix;
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

            // check if the string starts with the prefix
            if (s.StartsWith(prefix, Comparison)) return ValidationResult.Success;

            // at this point, we can only fail
            return new ValidationResult(errorMessage: FormatErrorMessage(name: validationContext.DisplayName),
                                        memberNames: new string[] { validationContext.MemberName });
        }

        /// <summary>
        /// Formats the error message to display if the validation fails.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>The formatted error message.</returns>
        public override string FormatErrorMessage(string name) => string.Format(ErrorMessageString, name, prefix);
    }
}
