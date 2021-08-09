using System.Collections.Generic;

namespace Tingle.Extensions.PhoneValidators
{
    /// <summary>
    /// Abstraction for validating phone numbers
    /// </summary>
    public interface IPhoneNumberValidator
    {
        /// <summary>
        /// Check if a phone number is valid.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns></returns>
        bool IsValid(string? phoneNumber);

        /// <summary>
        /// Make an <see href="https://en.wikipedia.org/wiki/MSISDN">MSISDN</see> version of a phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns></returns>
        string? ToMsisdn(string phoneNumber);

        /// <summary>
        /// Generate all possible values for a phone number
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns></returns>
        IEnumerable<string> MakePossibleValues(string phoneNumber);

        /// <summary>
        /// Make an <see href="https://en.wikipedia.org/wiki/E.164">E.164</see> version of a phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns></returns>
        string? ToE164(string phoneNumber);
    }
}
