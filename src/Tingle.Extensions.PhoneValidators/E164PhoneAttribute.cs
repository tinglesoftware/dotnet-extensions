using PhoneNumbers;

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value is a well-formed E.164 phone number or a list of well-formed E.164 phone numbers.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class E164PhoneAttribute : ValidationAttribute
{
    private readonly string? defaultRegion;

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
    public E164PhoneAttribute(string? defaultRegion = null) : base("The field {0} must be a valid E.164 phone number.")
    {
        this.defaultRegion = defaultRegion?.ToUpperInvariant();

        // when the region is specified, it must be supported
        if (this.defaultRegion != null)
        {
            var regions = PhoneNumberUtil.GetInstance().GetSupportedRegions();
            if (!regions.Contains(this.defaultRegion))
                throw new ArgumentOutOfRangeException(nameof(defaultRegion), $"'{defaultRegion}' is not a known region");
        }
    }

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (value is string s && !string.IsNullOrEmpty(s)) return IsValidE164(s);

        if (value is IEnumerable<string> values)
        {
            foreach (var v in values)
            {
                if (string.IsNullOrEmpty(v) || !IsValidE164(v))
                    return false;
            }
        }

        return true;
    }

    private bool IsValidE164(string value)
    {
        try
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            _ = phoneNumberUtil.Parse(numberToParse: value, defaultRegion: defaultRegion);
            return true;
        }
        catch (Exception ex) when (ex is NumberParseException)
        {
            return false;
        }
    }
}
