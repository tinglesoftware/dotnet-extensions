using PhoneNumbers;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies that a data field value is a well-formed E.164 phone number.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class E164PhoneAttribute : ValidationAttribute
    {
        private readonly string defaultRegion;

        /// <summary>
        /// Initializes a new instance of the <see cref="E164PhoneAttribute"/> class.
        /// </summary>
        /// <param name="defaultRegion">
        /// Region that we are expecting the number to be from. This is only used if the
        /// number being parsed is not written in international format. The country_code
        /// for the number in this case would be stored as that of the default region supplied.
        /// If the number is guaranteed to start with a '+' followed by the country calling
        /// code, then "ZZ" or null can be supplied.
        /// </param>
        public E164PhoneAttribute(string defaultRegion = null) : base("The field {0} must be a valid E.164 phone number.")
        {
            this.defaultRegion = defaultRegion?.ToUpperInvariant();

            // when the region is specified, it must be supported
            if (this.defaultRegion != null)
            {
                var regions = PhoneNumberUtil.GetInstance().GetSupportedRegions();
                if (!regions.Contains(this.defaultRegion))
                    throw new ArgumentOutOfRangeException(nameof(defaultRegion), $"'{defaultRegion}' is a known region");
            }
        }

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

            // attempt to parse phone number
            try
            {
                var phoneNumberUtil = PhoneNumberUtil.GetInstance();
                _ = phoneNumberUtil.Parse(numberToParse: s, defaultRegion: defaultRegion);
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
