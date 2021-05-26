using System.Text.RegularExpressions;
using Tingle.Extensions.PhoneValidators.Abstractions;

namespace Tingle.Extensions.PhoneValidators.Telkom
{
    /// <summary>
    /// Implementation of <see cref="IPhoneNumberValidator"/> specifically for Telkom phone numbers
    /// </summary>
    public class TelkomPhoneNumberValidator : AbstractPhoneNumberValidator
    {
        // This regular expression will match numbers with known formats.
        // The intention is to ensure the line number (after country code or local code) are actually standard.
        // At all times, the group at index 1 will give you the line number only i.e. 7xxxxxxxx.
        // The digits after the number 7 have a few possible combinations, whose information is gathered from the internet.
        // A good place to start https://en.wikipedia.org/wiki/Telephone_numbers_in_Kenya
        // The digits are 70-79 when prefixed with 7
        internal const string RegExComplete = @"^(?:254|\+254|0)?(7(?:(?:7[0-9]))[0-9]{6})$";

        private static readonly Regex regex = new Regex(@RegExComplete);

        /// <summary>
        /// Creates an instance of <see cref="TelkomPhoneNumberValidator"/>
        /// </summary>
        public TelkomPhoneNumberValidator() : base() { }

        internal override Regex RegularExpression => regex;
    }
}
