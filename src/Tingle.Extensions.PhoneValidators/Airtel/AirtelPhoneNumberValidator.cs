using System.Text.RegularExpressions;
using Tingle.Extensions.PhoneValidators.Abstractions;

namespace Tingle.Extensions.PhoneValidators.Airtel
{
    /// <summary>
    /// Implementation of <see cref="IPhoneNumberValidator"/> specifically for Airtel phone numbers
    /// </summary>
    public class AirtelPhoneNumberValidator : AbstractPhoneNumberValidator
    {
        // This regular expression will match numbers with known formats.
        // The intention is to ensure the line number (after country code or local code) are actually standard.
        // At all times, the group at index 1 will give you the line number only i.e. 7xxxxxxxx or 10xxxxxxx.
        // The digits after the number 7 or 1 have a few possible combinations, whose information is gathered from the internet.
        // A good place to start https://en.wikipedia.org/wiki/Telephone_numbers_in_Kenya
        // The digits are 30-39, 50-56, 85-89 when prefixed with 7 and 00-02 when prefixed with 1
        internal const string RegExComplete = @"^(?:254|\+254|0)?((?:(?:7(?:(?:3[0-9])|(?:5[0-6])|(8[5-9])))|(?:1(?:[0][0-2])))[0-9]{6})$";

        private static readonly Regex regex = new(@RegExComplete);

        /// <summary>
        /// Creates an instance of <see cref="AirtelPhoneNumberValidator"/>
        /// </summary>
        public AirtelPhoneNumberValidator() : base() { }

        internal override Regex RegularExpression => regex;
    }
}
