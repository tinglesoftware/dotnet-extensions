namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value is a well-formed SWIFT Code using a regular expression for SWIFT Codes.
/// The standard is specified under ISO-9362.
/// The Swift code can be either 8 or 11 characters long, and 8 digits code refers to the primary office.
/// The code consists of 4 separate section, and the format arrange in the following manner: <c>AAAA BB CC DDD</c>.
/// <code>The first 4 characters ("AAAA") specify the institution. Only letters.</code>
/// <code>
/// The next 2 characters("BB") specify the country where the instituion's located.The code follows the format
/// of ISO 3166-1 alpha-2 country code. Only letters.
/// </code>
/// <code>
/// The next 2 characters ("CC") specify the instiution's location. Can be letters and digits.
/// Passive participants will have "1" in the second character.
/// </code>
/// <code>
/// The last 3 characters("DDD") specify the institution's branch. This section is an optional.
/// When set to 'XXX' refers to a primary office. Can be letters and digits.
/// </code>
/// The default expression used for validation is <c>^[a-zA-Z]{4}[a-zA-Z]{2}[a-zA-Z0-9]{2}([a-zA-Z0-9]{3})?$</c>
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class SwiftCodeAttribute : RegularExpressionAttribute
{
    /// <summary></summary>
    public const string RegEx = @"^([a-zA-Z]{4})([a-zA-Z]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{3})?$";

    /// <summary>
    /// Initializes a new instance of the <see cref="SwiftCodeAttribute"/> class.
    /// </summary>
    /// <param name="pattern">
    /// The regular expression that is used to validate the data field value.
    /// Defaults to <c>^[a-zA-Z]{4}[a-zA-Z]{2}[a-zA-Z0-9]{2}([a-zA-Z0-9]{3})?$</c>
    /// </param>
    public SwiftCodeAttribute(string pattern = RegEx) : base(pattern) { }
}
