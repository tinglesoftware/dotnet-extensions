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
        /// Formats the error message to display if the validation fails.
        /// </summary>
        /// <param name="name">The name of the field that caused the validation failure.</param>
        /// <returns>The formatted error message.</returns>
        public override string FormatErrorMessage(string name) => string.Format(ErrorMessageString, name, prefix);

        /// <inheritdoc/>
        public override bool IsValid(object? value) => value is not string s || string.IsNullOrEmpty(s) || s.StartsWith(prefix, Comparison);
    }
}
