namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies the decimal numeric range for the value of a data field must be between 0 and 10.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class TenStarRatingAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TenStarRatingAttribute"/> class.
    /// </summary>
    public TenStarRatingAttribute() : base(0, 10.0f) { }
}
