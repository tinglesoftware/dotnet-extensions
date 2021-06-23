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
        /// An equality comparer to compare values.
        /// Defaults to <see cref="EqualityComparer{T}.Default"/>.
        /// </summary>
        public IEqualityComparer<object> Comparer { get; set; } = EqualityComparer<object>.Default;

        /// <inheritdoc/>
        public override bool IsValid(object? value)
        {
            if (value is null) return true;

            // if the value is an enumerable, create values from each, otherwise its just the value
            var values = !(value is string) && value is IEnumerable ie
                ? ie.Cast<object>().ToList()
                : new List<object> { value };

            // find the values not allowed
            var unknown = values.Where(o => !allowedValues.Contains(o, comparer: Comparer))
                                .ToList();

            // succeed only if there no unknown values
            return !unknown.Any();
        }
    }
}
