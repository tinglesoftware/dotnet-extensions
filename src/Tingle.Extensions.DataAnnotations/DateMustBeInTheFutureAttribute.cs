namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value is a date in the future.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class DateMustBeInTheFutureAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateMustBeInTheFutureAttribute"/> class.
    /// </summary>
    public DateMustBeInTheFutureAttribute() : base("The field {0} must be a date in the future.") { }

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        return !((value is DateTimeOffset dto && dto < DateTimeOffset.UtcNow)
              || (value is DateTime dt && dt < DateTime.UtcNow));
    }
}
