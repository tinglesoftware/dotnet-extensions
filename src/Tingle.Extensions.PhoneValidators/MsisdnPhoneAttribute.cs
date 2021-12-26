using PhoneNumbers;

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Specifies that a data field value is a well-formed MSISDN phone number or a list of well-formed MSISDN phone numbers.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class MsisdnPhoneAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MsisdnPhoneAttribute"/> class.
    /// </summary>
    public MsisdnPhoneAttribute() : base("The field {0} must be a valid MSISDN phone number.") { }

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (value is string s && !string.IsNullOrEmpty(s)) return IsValidMsisdn(s);

        if (value is IEnumerable<string> values)
        {
            foreach (var v in values)
            {
                if (v is not string str || string.IsNullOrEmpty(str) || !IsValidMsisdn(v))
                    return false;
            }
        }

        return true;
    }

    private bool IsValidMsisdn(string value)
    {
        if (value.StartsWith("+")) return false;

        try
        {
            var prepended = "+" + value;
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            _ = phoneNumberUtil.Parse(numberToParse: prepended, defaultRegion: null);
            return true;
        }
        catch (Exception ex) when (ex is NumberParseException)
        {
            return false;
        }
    }
}
