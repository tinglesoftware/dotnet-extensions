namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies that a data field value is a well-formed base 64 string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class Base64Attribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Base64Attribute"/> class.
        /// </summary>
        public Base64Attribute() : base("The field {0} must be a valid base64 string.") { }

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

            // attempt to convert from base64
            try
            {
                _ = Convert.FromBase64String(s);
                return ValidationResult.Success;
            }
            catch (Exception ex) when (ex is FormatException)
            {
                return new ValidationResult(errorMessage: FormatErrorMessage(name: validationContext.DisplayName),
                                            memberNames: new string[] { validationContext.MemberName });
            }
        }
    }
}
