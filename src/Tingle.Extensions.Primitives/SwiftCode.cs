using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Tingle.Extensions.Primitives.Converters;

namespace Tingle.Extensions.Primitives;

/// <summary>
/// Represents a SWIFT Code broken down into its components. The format of a Swift Code is as specified under ISO-9362.
/// </summary>
/// <param name="institution">the code</param>
/// <param name="country"></param>
/// <param name="location"></param>
/// <param name="branch"></param>
[JsonConverter(typeof(SwiftCodeJsonConverter))]
[TypeConverter(typeof(SwiftCodeTypeConverter))]
public sealed partial class SwiftCode(string institution, string country, string location, string? branch = null) : IEquatable<SwiftCode>, IComparable<SwiftCode>, IConvertible
{
    /// <summary>
    /// A 4-letter representation of the institution.
    /// </summary>
    public string Institution { get; } = institution;

    /// <summary>
    /// The 2-letter code of the country where the institution is located. It is compliant with ISO 3166-1 alpha-2.
    /// </summary>
    public string Country { get; } = country;

    /// <summary>
    /// A 2-character representation of the institution's location. Can be letters and digits.
    /// The second letter has predefined usages:
    /// <code>0 -> then it is typically a test BIC as opposed to a BIC used on the live network.</code>
    /// <code>1 -> then it denotes a passive participant in the SWIFT network.</code>
    /// <code>
    /// 2 -> then it typically indicates a reverse billing BIC, where the recipient pays for the message
    /// as opposed to the more usual mode whereby the sender pays for the message.
    /// </code>
    /// </summary>
    public string Location { get; } = location;

    /// <summary>
    /// A 3-character representation of the institution's branch. This value is an optional.
    /// When set to 'XXX' refers to a primary office. Can be letters and digits.
    /// </summary>
    public string? Branch { get; } = string.IsNullOrEmpty(branch) ? null : branch;

    /// <summary>
    /// Determines if the code is used in a test environment
    /// </summary>
    public bool Test => Location.LastOrDefault() == '0';

    /// <summary>
    /// Determines if the code points to a passive participant in the SWIFT network
    /// </summary>
    public bool Passive => Location.LastOrDefault() == '1';

    /// <summary>
    /// Determines if the code is meant for reverse billing, where the recipient pays for the message
    /// as opposed to the more usual mode whereby the sender pays for the message.
    /// </summary>
    public bool Reverse => Location.LastOrDefault() == '2';

    /// <summary>
    /// Determines if the value specified for <see cref="Branch"/> represents the primary office of the institution
    /// </summary>
    public bool Primary => Branch == null || string.Equals("XXX", Branch, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns the 8-character ISO-9362 string representation of this instance.
    /// </summary>
    /// <returns></returns>
    public string ToEightCharacter() => $"{Institution}{Country}{Location}";

    /// <summary>
    /// Returns the 11-character ISO-9362 string representation of this instance.
    /// </summary>
    /// <returns></returns>
    public string ToElevenCharacter() => $"{Institution}{Country}{Location}{Branch ?? "XXX"}";

    /// <summary>
    /// Returns ISO-9362 string representation of this instance.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => ToElevenCharacter();

    /// <inheritdoc/>
    public bool Equals(SwiftCode? other)
    {
        return other is not null && string.Equals(ToString(), other.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as SwiftCode);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Institution, Country, Location, Branch, Test, Passive, Reverse, Primary);
    }

    /// <inheritdoc/>
    public int CompareTo(SwiftCode? other) => ToElevenCharacter().CompareTo(other?.ToElevenCharacter());

    /// <summary>
    /// Parses a code into <see cref="SwiftCode"/>. The format of a Swift Code is as specified under ISO-9362.
    /// The Swift code can be either 8 or 11 characters long, an 8 digits code implies the primary office.
    /// The code consists of 4 separate section, and the format arrange in the following manner: <c>AAAA BB CC DDD</c>.
    /// <code>The first 4 characters ("AAAA") specify the institution. Only letters.</code>
    /// <code>
    /// The next 2 characters("BB") specify the country where the institution's located. The code follows the format
    /// of ISO 3166-1 alpha-2 country code. Only letters.
    /// </code>
    /// <code>
    /// The next 2 characters ("CC") specify the institution's location. Can be letters and digits.
    /// Passive participants will have "1" in the second character.
    /// </code>
    /// <code>
    /// The last 3 characters("DDD") specify the institution's branch. This section is an optional.
    /// When set to 'XXX' refers to a primary office. Can be letters and digits.
    /// </code>
    /// The default expression used for validation is <c>^([a-zA-Z]{4})([a-zA-Z]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{3})?$</c>
    /// </summary>
    /// <param name="code">the string representation of a swift code as specified under ISO-9362.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="code"/> is null.</exception>
    /// <exception cref="FormatException"><paramref name="code"/> does not have a valid format.</exception>
    public static SwiftCode Parse(string code)
    {
        ArgumentNullException.ThrowIfNull(code);

        if (TryParse(code, out var result)) return result;
        throw new FormatException($"'{code}' is not a valid Swift code.");
    }

    /// <summary>
    /// Parses a code into <see cref="SwiftCode"/>. The format of a Swift Code is as specified under ISO-9362.
    /// The Swift code can be either 8 or 11 characters long, an 8 digits code implies the primary office.
    /// The code consists of 4 separate section, and the format arrange in the following manner: <c>AAAA BB CC DDD</c>.
    /// <code>The first 4 characters ("AAAA") specify the institution. Only letters.</code>
    /// <code>
    /// The next 2 characters("BB") specify the country where the institution's located. The code follows the format
    /// of ISO 3166-1 alpha-2 country code. Only letters.
    /// </code>
    /// <code>
    /// The next 2 characters ("CC") specify the institution's location. Can be letters and digits.
    /// Passive participants will have "1" in the second character.
    /// </code>
    /// <code>
    /// The last 3 characters("DDD") specify the institution's branch. This section is an optional.
    /// When set to 'XXX' refers to a primary office. Can be letters and digits.
    /// </code>
    /// The default expression used for validation is <c>^([a-zA-Z]{4})([a-zA-Z]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{3})?$</c>
    /// </summary>
    /// <param name="code">the string representation of a swift code as specified under ISO-9362.</param>
    /// <param name="value">
    /// When this method returns, contains the value associated parsed,
    /// if successful; otherwise, <see langword="null"/> is returned.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="code"/> could be parsed; otherwise, false.
    /// </returns>
    public static bool TryParse(string code, [NotNullWhen(true)] out SwiftCode? value)
    {
        value = null;
        var match = GetPattern().Match(code);
        if (!match.Success) return false;

        value = new SwiftCode(institution: match.Groups[1].Value,
                                  country: match.Groups[2].Value,
                                  location: match.Groups[3].Value,
                                  branch: match.Groups[4].Value);

        return true;
    }

    /// <inheritdoc/>
    public static bool operator ==(SwiftCode left, SwiftCode right) => EqualityComparer<SwiftCode>.Default.Equals(left, right);

    /// <inheritdoc/>
    public static bool operator !=(SwiftCode left, SwiftCode right) => !(left == right);

    /// <summary>Converts a string to a <see cref="SwiftCode"/>.</summary>
    /// <param name="code">the string representation of the code</param>
    public static implicit operator SwiftCode(string code) => Parse(code: code);

    /// <summary>Converts a <see cref="SwiftCode"/> to a string.</summary>
    /// <param name="code">the string representation of the code</param>
    public static implicit operator string(SwiftCode code) => code.ToString();

    #region IConvertible

    TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

    bool IConvertible.ToBoolean(IFormatProvider? provider) => throw new InvalidCastException();
    byte IConvertible.ToByte(IFormatProvider? provider) => throw new InvalidCastException();
    char IConvertible.ToChar(IFormatProvider? provider) => throw new InvalidCastException();
    DateTime IConvertible.ToDateTime(IFormatProvider? provider) => throw new InvalidCastException();
    decimal IConvertible.ToDecimal(IFormatProvider? provider) => throw new InvalidCastException();
    double IConvertible.ToDouble(IFormatProvider? provider) => throw new InvalidCastException();
    short IConvertible.ToInt16(IFormatProvider? provider) => throw new InvalidCastException();
    int IConvertible.ToInt32(IFormatProvider? provider) => throw new InvalidCastException();
    long IConvertible.ToInt64(IFormatProvider? provider) => throw new InvalidCastException();
    sbyte IConvertible.ToSByte(IFormatProvider? provider) => throw new InvalidCastException();
    float IConvertible.ToSingle(IFormatProvider? provider) => throw new InvalidCastException();
    string IConvertible.ToString(IFormatProvider? provider) => ToString();

    object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
    {
        return Type.GetTypeCode(conversionType) switch
        {
            TypeCode.Object when conversionType == typeof(object) => this,
            TypeCode.Object when conversionType == typeof(SwiftCode) => this,
            TypeCode.String => ((IConvertible)this).ToString(provider),
            _ => throw new InvalidCastException(),
        };
    }

    ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new InvalidCastException();
    uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new InvalidCastException();
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new InvalidCastException();

    #endregion

    internal class SwiftCodeTypeConverter : TypeConverter
    {
        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string);

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType == typeof(string);

        /// <inheritdoc/>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            return value is string s ? Parse(s) : base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc/>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            return destinationType == typeof(string) && value is SwiftCode code
                ? code.ToString()
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [GeneratedRegex("^([a-zA-Z]{4})([a-zA-Z]{2})([a-zA-Z0-9]{2})([a-zA-Z0-9]{3})?$", RegexOptions.Compiled)]
    private static partial Regex GetPattern();
}
