using System.Text.RegularExpressions;
using Tingle.Extensions.PhoneValidators.Abstractions;

namespace Tingle.Extensions.PhoneValidators.Safaricom;

/// <summary>
/// Implementation of <see cref="IPhoneNumberValidator"/> specifically for Safaricom phone numbers
/// </summary>
public class SafaricomPhoneNumberValidator : AbstractPhoneNumberValidator
{
    // This regular expression will match numbers with known formats.
    // The intention is to ensure the line number (after country code or local code) are actually standard.
    // At all times, the group at index 1 will give you the line number only i.e. 7xxxxxxxx or 11xxxxxxx.
    // The digits after the number 7 or 1 have a few possible combinations, whose information is gathered from the internet.
    // A good place to start https://en.wikipedia.org/wiki/Telephone_numbers_in_Kenya
    // The digits are 00-09, 10-19, 20-29, 40-49, 90-99, 57-59, 68-69 when prefixed with 7 and 10-15 when prefixed with 1
    internal const string RegExComplete = @"^(?:254|\+254|0)?((?:(?:7(?:(?:[01249][0-9])|(?:5[789])|(?:6[89])))|(?:1(?:[1][0-5])))[0-9]{6})$";

    private static readonly Regex regex = new(@RegExComplete);

    /// <summary>
    /// Creates a instance of <see cref="SafaricomPhoneNumberValidator"/>
    /// </summary>
    public SafaricomPhoneNumberValidator() : base() { }

    internal override Regex RegularExpression => regex;
}
