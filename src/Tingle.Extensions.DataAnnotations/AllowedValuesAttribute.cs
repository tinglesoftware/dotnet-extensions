using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies that a data field value is allowed. When applied on an array, all its elements must be allowed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class AllowedValuesAttribute : ValidationAttribute
    {
        private readonly IEnumerable<object> allowedValues;
        private readonly IEqualityComparer<object> comparer = EqualityComparer<object>.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllowedValuesAttribute"/> class.
        /// </summary>
        public AllowedValuesAttribute(IEnumerable<object> allowedValues) : base("The value(s) '{0}' is/are not allowed for field {1}.")
        {
            this.allowedValues = allowedValues ?? throw new ArgumentNullException(nameof(allowedValues));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AllowedValuesAttribute"/> class.
        /// </summary>
        public AllowedValuesAttribute(params object[] allowedValues) : this(allowedValues.AsEnumerable()) { }

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
            if (value is null) return ValidationResult.Success;

            // if the value is an enumerable, create values from each, otherwise its just the value
            var values = !(value is string) && value is IEnumerable ie
                ? ie.Cast<object>().ToList()
                : new List<object> { value };

            // find the values not allowed
            var unknown = values.Where(o => !allowedValues.Contains(o, comparer: comparer))
                                .ToList();

            // if there are any, create validation error
            if (unknown.Any())
            {
                var em = string.Format(ErrorMessageString, string.Join(",", unknown), validationContext.DisplayName);
                return new ValidationResult(errorMessage: em,
                                            memberNames: new string[] { validationContext.MemberName });
            }

            // any other value type just passes
            return ValidationResult.Success;
        }
    }
}
