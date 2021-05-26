namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies the integer numeric range for the value of a data field must be more than zero.
    /// This only works for integers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class GreaterThanZeroAttribute : RangeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanZeroAttribute"/> class.
        /// </summary>
        public GreaterThanZeroAttribute() : base(1, int.MaxValue) { }
    }
}
