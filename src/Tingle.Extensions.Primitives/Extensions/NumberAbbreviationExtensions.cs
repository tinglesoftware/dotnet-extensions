using System.Globalization;

namespace System;

/// <summary>
/// Extensions for shortening numbers
/// </summary>
public static class NumberAbbreviationExtensions
{
    private const string FormatBillions = "0,,,.###B";
    private const string FormatMillions = "0,,.##M";
    private const string FormatKilo = "0,.#K";

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation.
    /// </summary>
    /// <param name="source">the numeric value</param>
    /// <returns>The string representation of the value of this instance as specified by format and provider.</returns>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static string ToStringAbbreviated(this long source) => source.ToStringAbbreviated(CultureInfo.InvariantCulture);

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation
    /// using the specified culture-specific format information.
    /// </summary>
    /// <param name="source">the numeric value</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <returns>The string representation of the value of this instance as specified by format and provider.</returns>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static string ToStringAbbreviated(this long source, IFormatProvider? provider)
    {
        if (source > 999999999 || source < -999999999) return source.ToString(FormatBillions, provider);
        else if (source > 999999 || source < -999999) return source.ToString(FormatMillions, provider);
        else if (source > 999 || source < -999) return source.ToString(FormatKilo, provider);
        return source.ToString(provider);
    }

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation.
    /// </summary>
    /// <param name="source">the numeric value</param>
    /// <returns>The string representation of the value of this instance as specified by format and provider.</returns>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static string ToStringAbbreviated(this int source) => source.ToStringAbbreviated(CultureInfo.InvariantCulture);

    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation
    /// using the specified culture-specific format information.
    /// </summary>
    /// <param name="source">the numeric value</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <returns>The string representation of the value of this instance as specified by format and provider.</returns>
    /// <exception cref="FormatException">format is invalid or not supported.</exception>
    public static string ToStringAbbreviated(this int source, IFormatProvider? provider)
    {
        if (source > 999999999 || source < -999999999) return source.ToString(FormatBillions, provider);
        else if (source > 999999 || source < -999999) return source.ToString(FormatMillions, provider);
        else if (source > 999 || source < -999) return source.ToString(FormatKilo, provider);
        return source.ToString(provider);
    }
}
