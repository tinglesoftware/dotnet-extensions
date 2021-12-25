namespace System.ComponentModel.DataAnnotations;

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

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (value is not string s || string.IsNullOrEmpty(s)) return true;
        // attempt to convert from base64
        try
        {
            _ = Convert.FromBase64String(s);
            return true;
        }
        catch (Exception ex) when (ex is FormatException)
        {
            return false;
        }
    }
}
