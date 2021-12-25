namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies the decimal numeric range for the value of a data field must be between 0 and 5.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class FiveStarRatingAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FiveStarRatingAttribute"/> class.
    /// </summary>
    public FiveStarRatingAttribute() : base(0, 5.0f) { }
}
