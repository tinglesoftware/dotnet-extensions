using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tingle.Extensions.PhoneValidators.Abstractions
{
    /// <summary>
    /// Abstract implementation of <see cref="IPhoneNumberValidator"/>
    /// </summary>
    public abstract class AbstractPhoneNumberValidator : IPhoneNumberValidator
    {
        /// <summary>
        /// Creates an instance of <see cref="AbstractPhoneNumberValidator"/>
        /// </summary>
        public AbstractPhoneNumberValidator() { }

        /// <summary>
        /// The phone number prefix denoting the country
        /// </summary>
        public virtual string CountryPrefix { get; set; } = "254";

        internal abstract Regex RegularExpression { get; }

        /// <inheritdoc/>
        public bool IsValid(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return false;
            return RegularExpression.Match(phoneNumber).Success;
        }

        /// <inheritdoc/>
        public IEnumerable<string>? MakePossibleValues(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return null;

            var match = RegularExpression.Match(phoneNumber);
            if (!match.Success) return Array.Empty<string>();

            var linenumber = match.Groups[1].Value;
            return new string[] { $"{CountryPrefix}{linenumber}", $"0{linenumber}", linenumber };
        }

        /// <inheritdoc/>
        public string? ToMsisdn(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return null;

            var match = RegularExpression.Match(phoneNumber);
            if (match.Success) return $"{CountryPrefix}{match.Groups[1].Value}";

            return null;
        }

        /// <inheritdoc/>
        public string? ToE164(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return null;

            var match = RegularExpression.Match(phoneNumber);
            if (match.Success) return $"+{CountryPrefix}{match.Groups[1].Value}";

            return null;
        }
    }
}
