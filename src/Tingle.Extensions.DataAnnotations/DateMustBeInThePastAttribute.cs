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

        /// <inheritdoc/>
        public override bool IsValid(object? value)
        {
            return !((value is DateTimeOffset dto && dto > DateTimeOffset.UtcNow)
                  || (value is DateTime dt && dt > DateTime.UtcNow));
        }
    }
}
