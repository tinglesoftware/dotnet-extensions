using System.ComponentModel;
using System.Globalization;
using System.Text.Json.Serialization;
using Tingle.Extensions.Primitives.Converters;

namespace Tingle.Extensions.Primitives;

/// <summary>
/// A convenience for working with Etags
/// </summary>
[JsonConverter(typeof(EtagJsonConverter))]
[TypeConverter(typeof(EtagTypeConverter))]
public readonly struct Etag : IEquatable<Etag>, IComparable<Etag>, IConvertible, IFormattable
{
    /// <summary>
    /// The default value starting from the beginning (<see cref="ulong.MinValue"/>)
    /// </summary>
    public static readonly Etag Empty = new(ulong.MinValue);

    internal const string Base64Format = "B";
    internal const string DefaultFormat = "D";
    internal const string HeaderFormat = "H";

    private const char QuoteCharacter = '"';
    private const string HexSpecifier = "0x";

    private readonly ulong value;

    /// <summary>
    /// Creates an instance of <see cref="Etag"/>
    /// </summary>
    /// <param name="value">The being tracked</param>
    public Etag(ulong value) => this.value = value;

    /// <summary>
    /// Creates an instance of <see cref="Etag"/>
    /// </summary>
    /// <param name="value">The Base64 encoded string of the value in bytes</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is null or whitespace</exception>
    /// <exception cref="FormatException">
    /// The length of <paramref name="value"/>, ignoring white-space characters, is not zero or a multiple
    /// of 4. -or- The format of <paramref name="value"/> is invalid. <paramref name="value"/> contains
    /// a non-base-64 character, more than two padding characters, or a non-white space-character among the
    /// padding characters.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="value"/> does not have sufficient data to create <see cref="ulong"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="value"/> does not have sufficient data to create <see cref="ulong"/>.
    /// </exception>
    public Etag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"'{nameof(value)}' cannot be null or whitespace", nameof(value));
        }

        // remove quotes and parse the number
        if (value.StartsWith(QuoteCharacter)) value = value.Trim(QuoteCharacter);

        if (value.StartsWith(HexSpecifier, StringComparison.OrdinalIgnoreCase))
        {
            value = value[HexSpecifier.Length..];
            this.value = ulong.Parse(value, NumberStyles.HexNumber);
        }
        else
        {
            var raw = Convert.FromBase64String(value);  // convert from base64 string
            this.value = BitConverter.ToUInt64(raw, 0); // convert to ulong
        }
    }

    /// <summary>
    /// Creates an instance of <see cref="Etag"/>
    /// </summary>
    /// <param name="value">The raw bytes to for a <see cref="ulong"/></param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="value"/> does not have sufficient data to create <see cref="ulong"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="value"/> does not have sufficient data to create <see cref="ulong"/>.
    /// </exception>
    public Etag(byte[] value) : this(value: value, startIndex: 0) { }

    /// <summary>
    /// Creates an instance of <see cref="Etag"/>
    /// </summary>
    /// <param name="value">The raw bytes to for a <see cref="ulong"/></param>
    /// <param name="startIndex">The starting position within value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="startIndex"/> is less than zero or greater than the length of value minus 1.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="startIndex"/> is greater than or equal to the length of value minus 7, and is less
    /// than or equal to the length of value minus 1.
    /// </exception>
    public Etag(byte[] value, int startIndex)
    {
        ArgumentNullException.ThrowIfNull(value);

        this.value = BitConverter.ToUInt64(value, startIndex);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Etag etag && Equals(etag);

    /// <inheritdoc/>
    public bool Equals(Etag other) => value == other.value;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(value);

    /// <inheritdoc/>
    public int CompareTo(Etag other) => value.CompareTo(other.value);

    /// <inheritdoc/>
    public override string ToString() => ToString(Base64Format);

    /// <summary>Returns the string representation of the <see cref="Etag"/>.</summary>
    /// <param name="format">A format string. Valid values are "B" for base64 format, "D" for standard hex format and "H" for header format.</param>
    /// <returns>The formatted string representation of this <see cref="Etag"/>. This includes outer quotes.</returns>
    /// <example>
    /// <code>
    /// Etag tag = Etag.Parse("\"0x0\"");
    /// Console.WriteLine(tag.ToString("B"));
    /// // Displays: AAAAAAAAAAA=
    /// Console.WriteLine(tag.ToString("D"));
    /// // Displays: 0x0
    /// Console.WriteLine(tag.ToString("H"));
    /// // Displays: "0x0"
    /// </code>
    /// </example>
    public string ToString(string? format) => ToString(format, CultureInfo.CurrentCulture);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        format ??= Base64Format;

        // WARNING: passing IFormatProvider when getting the string representation of value is causing a mix-up of formats e.g. 0xAAAAAAA=

        return format.ToUpperInvariant() switch
        {
            Base64Format => Convert.ToBase64String(ToByteArray()),
            DefaultFormat => $"{HexSpecifier}{value:X}",
            HeaderFormat => $"{QuoteCharacter}{HexSpecifier}{value:X}{QuoteCharacter}",
            _ => throw new FormatException($"The {format} format string is not supported."),
        };
    }

    /// <summary>Returns a 4-element byte array that contains the value of this instance.</summary>
    /// <returns>A 4-element byte array.</returns>
    public byte[] ToByteArray() => BitConverter.GetBytes(value);

    /// <summary>
    /// Get the next Etag
    /// </summary>
    /// <returns></returns>
    public Etag Next() => new(value + 1);

    /// <inheritdoc/>
    public static bool operator ==(Etag left, Etag right) => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(Etag left, Etag right) => !(left == right);

    /// <inheritdoc/>
    public static bool operator <(Etag left, Etag right) => left.CompareTo(right) < 0;

    /// <inheritdoc/>
    public static bool operator <=(Etag left, Etag right) => left.CompareTo(right) <= 0;

    /// <inheritdoc/>
    public static bool operator >(Etag left, Etag right) => left.CompareTo(right) > 0;

    /// <inheritdoc/>
    public static bool operator >=(Etag left, Etag right) => left.CompareTo(right) >= 0;

    /// <summary>Converts a <see cref="string"/> to a <see cref="Etag"/>.</summary>
    /// <param name="s"></param>
    public static implicit operator Etag(string s) => new(value: s);

    /// <summary>Converts a <see cref="T:byte[]"/> to a <see cref="Etag"/>.</summary>
    /// <param name="raw"></param>
    public static implicit operator Etag(byte[] raw) => new(value: raw);

    /// <summary>Converts a <see cref="ulong"/> to a <see cref="Etag"/>.</summary>
    /// <param name="value"></param>
    public static implicit operator Etag(ulong value) => new(value: value);

    /// <summary>Converts a <see cref="Etag"/> to a string.</summary>
    /// <param name="etag"></param>
    public static implicit operator string(Etag etag) => etag.ToString();

    /// <summary>Converts a <see cref="Etag"/> to a <see cref="T:byte[]"/>.</summary>
    /// <param name="etag"></param>
    public static implicit operator byte[](Etag etag) => etag.ToByteArray();

    /// <summary>Converts a <see cref="Etag"/> to a <see cref="ulong"/>.</summary>
    /// <param name="etag"></param>
    public static implicit operator ulong(Etag etag) => etag.value;

    /// <summary>Combine multiple instances of <see cref="Etag"/> into one.</summary>
    /// <param name="etags">The instances to be combined.</param>
    /// <returns></returns>
    public static Etag Combine(params Etag[] etags)
    {
        ArgumentNullException.ThrowIfNull(etags);
        if (etags.Length == 0) return Empty;

        var value = 0UL;
        foreach (var e in etags)
        {
            value += e.value;
        }
        return new Etag(value);
    }

    /// <summary>Combine multiple instances of <see cref="Etag"/> into one.</summary>
    /// <param name="etags">The instances to be combined.</param>
    /// <returns></returns>
    public static Etag Combine(IEnumerable<Etag> etags) => Combine(etags.ToArray());

    /// <summary>Combine multiple instances of <see cref="ulong"/> into one <see cref="Etag"/>.</summary>
    /// <param name="etags">The values to be combined.</param>
    /// <returns></returns>
    public static Etag Combine(IEnumerable<ulong> etags) => Combine(etags.Select(e => new Etag(e)));

    /// <summary>Combine multiple instances of <see cref="string"/> into one <see cref="Etag"/>.</summary>
    /// <param name="etags">The values to be combined.</param>
    /// <returns></returns>
    public static Etag Combine(IEnumerable<string> etags) => Combine(etags.Select(e => new Etag(e)));

    /// <summary>Combine multiple instances of <see cref="T:byte[]"/> into one <see cref="Etag"/>.</summary>
    /// <param name="etags">The values to be combined.</param>
    /// <returns></returns>
    public static Etag Combine(IEnumerable<byte[]> etags) => Combine(etags.Select(e => new Etag(e)));

    #region IConvertible

    TypeCode IConvertible.GetTypeCode() => TypeCode.Object;
    bool IConvertible.ToBoolean(IFormatProvider? provider) => throw new InvalidCastException();
    byte IConvertible.ToByte(IFormatProvider? provider) => throw new InvalidCastException();
    char IConvertible.ToChar(IFormatProvider? provider) => throw new InvalidCastException();
    DateTime IConvertible.ToDateTime(IFormatProvider? provider) => throw new InvalidCastException();
    decimal IConvertible.ToDecimal(IFormatProvider? provider) => Convert.ToDecimal(value);
    double IConvertible.ToDouble(IFormatProvider? provider) => Convert.ToDouble(value);
    short IConvertible.ToInt16(IFormatProvider? provider) => throw new InvalidCastException();
    int IConvertible.ToInt32(IFormatProvider? provider) => throw new InvalidCastException();
    long IConvertible.ToInt64(IFormatProvider? provider) => (long)value;
    sbyte IConvertible.ToSByte(IFormatProvider? provider) => throw new InvalidCastException();
    float IConvertible.ToSingle(IFormatProvider? provider) => throw new InvalidCastException();
    string IConvertible.ToString(IFormatProvider? provider) => ToString();

    object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
    {
        return Type.GetTypeCode(conversionType) switch
        {
            TypeCode.Decimal => ((IConvertible)this).ToDecimal(provider),
            TypeCode.Double => ((IConvertible)this).ToDouble(provider),
            TypeCode.Int64 => ((IConvertible)this).ToInt64(provider),
            TypeCode.Object when conversionType == typeof(object) => this,
            TypeCode.Object when conversionType == typeof(Etag) => this,
            TypeCode.String => ((IConvertible)this).ToString(provider),
            TypeCode.UInt64 => ((IConvertible)this).ToUInt64(provider),
            _ => throw new InvalidCastException(),
        };
    }

    ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new InvalidCastException();
    uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new InvalidCastException();
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => value;

    #endregion

    internal class EtagTypeConverter : TypeConverter
    {
        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string);

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType == typeof(string);

        /// <inheritdoc/>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            return value is string s ? new Etag(s) : base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc/>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            return destinationType == typeof(string) && value is Etag etag
                ? etag.ToString()
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
