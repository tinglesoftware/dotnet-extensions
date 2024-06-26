﻿using System.Text.RegularExpressions;

namespace Tingle.Extensions.PhoneValidators.Abstractions;

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
    public bool IsValid(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return false;
        return RegularExpression.Match(phoneNumber).Success;
    }

    /// <inheritdoc/>
    public IEnumerable<string> MakePossibleValues(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException($"'{nameof(phoneNumber)}' cannot be null or whitespace.", nameof(phoneNumber));
        }

        var match = RegularExpression.Match(phoneNumber);
        if (!match.Success) return [];

        var linenumber = match.Groups[1].Value;
        return [$"{CountryPrefix}{linenumber}", $"0{linenumber}", linenumber];
    }

    /// <inheritdoc/>
    public string? ToMsisdn(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException($"'{nameof(phoneNumber)}' cannot be null or whitespace.", nameof(phoneNumber));
        }

        var match = RegularExpression.Match(phoneNumber);
        return match.Success ? $"{CountryPrefix}{match.Groups[1].Value}" : null;
    }

    /// <inheritdoc/>
    public string? ToE164(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException($"'{nameof(phoneNumber)}' cannot be null or whitespace.", nameof(phoneNumber));
        }

        var match = RegularExpression.Match(phoneNumber);
        return match.Success ? $"+{CountryPrefix}{match.Groups[1].Value}" : null;
    }
}
