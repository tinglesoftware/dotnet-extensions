using System.ComponentModel;
using System.Globalization;
using System.Text.Json.Serialization;
using Tingle.Extensions.Primitives.Converters;

namespace Tingle.Extensions.Primitives;

/// <summary>
/// Represents a byte size value with support for decimal (KiloByte) and binary values (KibiByte).
/// </summary>
/// <param name="bytes">Number of bytes.</param>
[JsonConverter(typeof(ByteSizeJsonConverter))]
[TypeConverter(typeof(ByteSizeTypeConverter))]
public readonly struct ByteSize(long bytes) : IEquatable<ByteSize>, IComparable<ByteSize>, IConvertible, IFormattable
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static readonly ByteSize MinValue = FromBytes(long.MinValue);
    public static readonly ByteSize MaxValue = FromBytes(long.MaxValue);

    public const long BytesInKiloByte = 1_000;
    public const long BytesInMegaByte = 1_000_000;
    public const long BytesInGigaByte = 1_000_000_000;
    public const long BytesInTeraByte = 1_000_000_000_000;
    public const long BytesInPetaByte = 1_000_000_000_000_000;
    public const long BytesInKibiByte = 1_024;
    public const long BytesInMebiByte = 1_048_576;
    public const long BytesInGibiByte = 1_073_741_824;
    public const long BytesInTebiByte = 1_099_511_627_776;
    public const long BytesInPebiByte = 1_125_899_906_842_624;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    internal const string ByteSymbol = "B";
    internal const string KiloByteSymbol = "KB";
    internal const string MegaByteSymbol = "MB";
    internal const string GigaByteSymbol = "GB";
    internal const string TeraByteSymbol = "TB";
    internal const string PetaByteSymbol = "PB";
    internal const string KibiByteSymbol = "KiB";
    internal const string MebiByteSymbol = "MiB";
    internal const string GibiByteSymbol = "GiB";
    internal const string TebiByteSymbol = "TiB";
    internal const string PebiByteSymbol = "PiB";

    #region Properties

    /// <summary>Number of bytes.</summary>
    public long Bytes { get; } = bytes;

    internal string LargestWholeNumberBinarySymbol
    {
        get
        {
            // Absolute value is used to deal with negative values
            if (Math.Abs(PebiBytes) >= 1) return PebiByteSymbol;
            else if (Math.Abs(TebiBytes) >= 1) return TebiByteSymbol;
            else if (Math.Abs(GibiBytes) >= 1) return GibiByteSymbol;
            else if (Math.Abs(MebiBytes) >= 1) return MebiByteSymbol;
            else if (Math.Abs(KibiBytes) >= 1) return KibiByteSymbol;
            return ByteSymbol;
        }
    }

    internal string LargestWholeNumberDecimalSymbol
    {
        get
        {
            // Absolute value is used to deal with negative values
            if (Math.Abs(PetaBytes) >= 1) return PetaByteSymbol;
            else if (Math.Abs(TeraBytes) >= 1) return TeraByteSymbol;
            else if (Math.Abs(GigaBytes) >= 1) return GigaByteSymbol;
            else if (Math.Abs(MegaBytes) >= 1) return MegaByteSymbol;
            else if (Math.Abs(KiloBytes) >= 1) return KiloByteSymbol;
            return ByteSymbol;
        }
    }

    internal double LargestWholeNumberBinaryValue
    {
        get
        {
            // Absolute value is used to deal with negative values
            if (Math.Abs(PebiBytes) >= 1) return PebiBytes;
            else if (Math.Abs(TebiBytes) >= 1) return TebiBytes;
            else if (Math.Abs(GibiBytes) >= 1) return GibiBytes;
            else if (Math.Abs(MebiBytes) >= 1) return MebiBytes;
            else if (Math.Abs(KibiBytes) >= 1) return KibiBytes;
            return Bytes;
        }
    }

    internal double LargestWholeNumberDecimalValue
    {
        get
        {
            // Absolute value is used to deal with negative values
            if (Math.Abs(PetaBytes) >= 1) return PetaBytes;
            else if (Math.Abs(TeraBytes) >= 1) return TeraBytes;
            else if (Math.Abs(GigaBytes) >= 1) return GigaBytes;
            else if (Math.Abs(MegaBytes) >= 1) return MegaBytes;
            else if (Math.Abs(KiloBytes) >= 1) return KiloBytes;
            return Bytes;
        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public double KiloBytes => (double)Bytes / BytesInKiloByte;
    public double MegaBytes => (double)Bytes / BytesInMegaByte;
    public double GigaBytes => (double)Bytes / BytesInGigaByte;
    public double TeraBytes => (double)Bytes / BytesInTeraByte;
    public double PetaBytes => (double)Bytes / BytesInPetaByte;
    public double KibiBytes => (double)Bytes / BytesInKibiByte;
    public double MebiBytes => (double)Bytes / BytesInMebiByte;
    public double GibiBytes => (double)Bytes / BytesInGibiByte;
    public double TebiBytes => (double)Bytes / BytesInTebiByte;
    public double PebiBytes => (double)Bytes / BytesInPebiByte;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    #endregion

    #region Fromxxx(...) methods

    /// <summary>Create a <see cref="ByteSize"/> from a given number of bytes.</summary>
    /// <param name="value">The number of bytes.</param>
    public static ByteSize FromBytes(long value) => new(value);


    /// <summary>Create a <see cref="ByteSize"/> from a given number of kilo bytes.</summary>
    /// <param name="value">The number of kilo bytes.</param>
    public static ByteSize FromKiloBytes(double value) => new((long)(value * BytesInKiloByte));

    /// <summary>Create a <see cref="ByteSize"/> from a given number of mega bytes.</summary>
    /// <param name="value">The number of mega bytes.</param>
    public static ByteSize FromMegaBytes(double value) => new((long)(value * BytesInMegaByte));

    /// <summary>Create a <see cref="ByteSize"/> from a given number of giga bytes.</summary>
    /// <param name="value">The number of giga bytes.</param>
    public static ByteSize FromGigaBytes(double value) => new((long)(value * BytesInGigaByte));

    /// <summary>Create a <see cref="ByteSize"/> from a given number of tera bytes.</summary>
    /// <param name="value">The number of tera bytes.</param>
    public static ByteSize FromTeraBytes(double value) => new((long)(value * BytesInTeraByte));

    /// <summary>Create a <see cref="ByteSize"/> from a given number of peta bytes.</summary>
    /// <param name="value">The number of peta bytes.</param>
    public static ByteSize FromPetaBytes(double value) => new((long)(value * BytesInPetaByte));


    /// <summary>Create a <see cref="ByteSize"/> from a given number of kibi bytes.</summary>
    /// <param name="value">The number of kibi bytes.</param>
    public static ByteSize FromKibiBytes(double value) => new((long)(value * BytesInKibiByte));

    /// <summary>Create a <see cref="ByteSize"/> from a given number of mebi bytes.</summary>
    /// <param name="value">The number of mebi bytes.</param>
    public static ByteSize FromMebiBytes(double value) => new((long)(value * BytesInMebiByte));

    /// <summary>Create a <see cref="ByteSize"/> from a given number of gibi bytes.</summary>
    /// <param name="value">The number of gibi bytes.</param>
    public static ByteSize FromGibiBytes(double value) => new((long)(value * BytesInGibiByte));

    /// <summary>Create a <see cref="ByteSize"/> from a given number of tebi bytes.</summary>
    /// <param name="value">The number of tebi bytes.</param>
    public static ByteSize FromTebiBytes(double value) => new((long)(value * BytesInTebiByte));

    /// <summary>Create a <see cref="ByteSize"/> from a given number of pebi bytes.</summary>
    /// <param name="value">The number of pebi bytes.</param>
    public static ByteSize FromPebiBytes(double value) => new((long)(value * BytesInPebiByte));

    #endregion

    #region ToString(...) methods

    /// <summary>
    /// Converts the value of the current object to a string.
    /// The prefix symbol (bit, byte, kilo, mebi, gibi, tebi) used is the
    /// largest prefix such that the corresponding value is greater than or
    /// equal to one.
    /// </summary>
    public override string ToString() => ToString("0.##", CultureInfo.CurrentCulture);

    /// <summary>Formats the value of the current instance using the specified format.</summary>
    /// <param name="format">
    /// The format to use. -or- A null reference (Nothing in Visual Basic) to use the
    /// default format defined for the type of the System.IFormattable implementation.
    /// </param>
    /// <returns>The value of the current instance in the specified format.</returns>
    public string ToString(string? format) => ToString(format, CultureInfo.CurrentCulture);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider) => ToString(format, formatProvider, useBinaryByte: false);

    /// <summary>Formats the value of the current instance in binary format.</summary>
    /// <returns>The value of the current instance in the specified format.</returns>
    public string ToBinaryString() => ToString("0.##", CultureInfo.CurrentCulture, useBinaryByte: true);

    /// <summary>Formats the value of the current instance in binary format.</summary>
    /// <param name="formatProvider">
    /// The provider to use to format the value. -or- A null reference (Nothing in Visual
    /// Basic) to obtain the numeric format information from the current locale setting
    /// of the operating system.
    /// </param>
    /// <returns>The value of the current instance in the specified format.</returns>
    public string ToBinaryString(IFormatProvider? formatProvider) => ToString("0.##", formatProvider, useBinaryByte: true);

    /// <summary>Formats the value of the current instance using the specified format.</summary>
    /// <param name="format">
    /// The format to use. -or- A null reference (Nothing in Visual Basic) to use the
    /// default format defined for the type of the System.IFormattable implementation.
    /// </param>
    /// <param name="formatProvider">
    /// The provider to use to format the value. -or- A null reference (Nothing in Visual
    /// Basic) to obtain the numeric format information from the current locale setting
    /// of the operating system.
    /// </param>
    /// <param name="useBinaryByte">
    /// Whether to use binary format
    /// </param>
    /// <returns>The value of the current instance in the specified format.</returns>
    public string ToString(string? format, IFormatProvider? formatProvider, bool useBinaryByte)
    {
        format ??= "0.##";
        formatProvider ??= CultureInfo.CurrentCulture;

        if (!format.Contains('#') && !format.Contains('0'))
            format = "0.## " + format;

        bool has(string s) => format.Contains(s, StringComparison.CurrentCultureIgnoreCase);
        string output(double n) => n.ToString(format, formatProvider);

        // Binary
        if (has("PiB")) return output(PebiBytes);
        if (has("TiB")) return output(TebiBytes);
        if (has("GiB")) return output(GibiBytes);
        if (has("MiB")) return output(MebiBytes);
        if (has("KiB")) return output(KibiBytes);

        // Decimal
        if (has("PB")) return output(PetaBytes);
        if (has("TB")) return output(TeraBytes);
        if (has("GB")) return output(GigaBytes);
        if (has("MB")) return output(MegaBytes);
        if (has("KB")) return output(KiloBytes);

        // Byte and Bit symbol must be case-sensitive
        if (format.Contains(ByteSymbol, StringComparison.CurrentCulture))
            return output(Bytes);

        return useBinaryByte
            ? string.Format("{0} {1}", LargestWholeNumberBinaryValue.ToString(format, formatProvider), LargestWholeNumberBinarySymbol)
            : string.Format("{0} {1}", LargestWholeNumberDecimalValue.ToString(format, formatProvider), LargestWholeNumberDecimalSymbol);
    }

    #endregion

    /// <inheritdoc/>
    public override bool Equals(object? value) => value is ByteSize size && Equals(size);

    /// <inheritdoc/>
    public bool Equals(ByteSize value) => Bytes == value.Bytes;

    /// <inheritdoc/>
    public override int GetHashCode() => Bytes.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(ByteSize other) => Bytes.CompareTo(other.Bytes);

    #region Arithmetic methods

    /// <summary>
    /// Returns a new <see cref="ByteSize"/> object whose value is the sum of the specified
    /// <see cref="ByteSize"/> object and this instance.
    /// </summary>
    /// <param name="bs">The byte size to add.</param>
    /// <returns>
    /// A new byte size whose value is the result of the value of this instance plus the
    /// value of <paramref name="bs"/>.
    /// </returns>
    /// <exception cref="OverflowException">
    /// The return value is less than <see cref="MinValue"/> or greater than <see cref="MaxValue"/>.
    /// </exception>
    public ByteSize Add(ByteSize bs) => new(Bytes + bs.Bytes);

    /// <summary>
    /// Returns a new <see cref="ByteSize"/> object whose value is the difference between the
    /// specified <see cref="ByteSize"/> object and this instance.
    /// </summary>
    /// <param name="bs">The byte size to be subtracted.</param>
    /// <returns>
    /// A new byte size whose value is the result of the value of this instance minus the
    /// value of <paramref name="bs"/>.
    /// </returns>
    /// <exception cref="OverflowException">
    /// The return value is less than <see cref="MinValue"/> or greater than <see cref="MaxValue"/>.
    /// </exception>
    public ByteSize Subtract(ByteSize bs) => new(Bytes - bs.Bytes);

    #endregion

    /// <inheritdoc/>
    public static ByteSize operator +(ByteSize b1, ByteSize b2) => new(b1.Bytes + b2.Bytes);

    /// <inheritdoc/>
    public static ByteSize operator -(ByteSize b) => new(-b.Bytes);

    /// <inheritdoc/>
    public static ByteSize operator -(ByteSize b1, ByteSize b2) => new(b1.Bytes - b2.Bytes);

    /// <inheritdoc/>
    public static ByteSize operator *(ByteSize a, ByteSize b) => new(a.Bytes * b.Bytes);

    /// <inheritdoc/>
    public static ByteSize operator /(ByteSize a, ByteSize b) => b.Bytes == 0 ? throw new DivideByZeroException() : new ByteSize(a.Bytes / b.Bytes);

    /// <inheritdoc/>
    public static bool operator ==(ByteSize b1, ByteSize b2) => b1.Bytes == b2.Bytes;

    /// <inheritdoc/>
    public static bool operator !=(ByteSize b1, ByteSize b2) => b1.Bytes != b2.Bytes;

    /// <inheritdoc/>
    public static bool operator <(ByteSize b1, ByteSize b2) => b1.Bytes < b2.Bytes;

    /// <inheritdoc/>
    public static bool operator <=(ByteSize b1, ByteSize b2) => b1.Bytes <= b2.Bytes;

    /// <inheritdoc/>
    public static bool operator >(ByteSize b1, ByteSize b2) => b1.Bytes > b2.Bytes;

    /// <inheritdoc/>
    public static bool operator >=(ByteSize b1, ByteSize b2) => b1.Bytes >= b2.Bytes;

    #region Parsing

    /// <summary>Converts a <see cref="string"/> into a <see cref="ByteSize"/>.</summary>
    /// <param name="s">A string containing the value to convert.</param>
    /// <returns>A <see cref="ByteSize"/> equivalent to the value specified in <paramref name="s"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException"><paramref name="s"/> is not in a correct format.</exception>
    public static ByteSize Parse(string s) => Parse(s, NumberFormatInfo.CurrentInfo);

    /// <summary>Converts a <see cref="string"/> into a <see cref="ByteSize"/> in a specified culture-specific format.</summary>
    /// <param name="s">A string containing the value to convert.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
    /// <returns>A <see cref="ByteSize"/> equivalent to the value specified in <paramref name="s"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException"><paramref name="s"/> is not in a correct format.</exception>
    public static ByteSize Parse(string s, IFormatProvider formatProvider) => Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, formatProvider);

    /// <summary>Converts a <see cref="string"/> into a <see cref="ByteSize"/> in a specified style and culture-specific format.</summary>
    /// <param name="s">A string containing the value to convert.</param>
    /// <param name="style">
    /// A bitwise combination of <see cref="NumberStyles"/> values that indicates the permitted format of <paramref name="s"/>.
    /// A typical value to specify is <see cref="NumberStyles.Float"/> combined with <see cref="NumberStyles.AllowThousands"/>.
    /// </param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
    /// <returns>A <see cref="ByteSize"/> equivalent to the value specified in <paramref name="s"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="style"/> is not a <see cref="NumberStyles"/> value.
    /// -or- style includes the <see cref="NumberStyles.AllowHexSpecifier"/> value.
    /// </exception>
    /// <exception cref="FormatException"><paramref name="s"/> is not in a correct format.</exception>
    public static ByteSize Parse(string s, NumberStyles style, IFormatProvider formatProvider)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            throw new ArgumentException($"'{nameof(s)}' cannot be null or whitespace.", nameof(s));
        }

        // Get the index of the first non-digit character
        s = s.TrimStart(); // Protect against leading spaces

        var found = false;

        var numberFormatInfo = NumberFormatInfo.GetInstance(formatProvider);
        var decimalSeparator = Convert.ToChar(numberFormatInfo.NumberDecimalSeparator);
        var groupSeparator = Convert.ToChar(numberFormatInfo.NumberGroupSeparator);


        int num;
        // Pick first non-digit number
        for (num = 0; num < s.Length; num++)
        {
            if (!(char.IsDigit(s[num]) || s[num] == decimalSeparator || s[num] == groupSeparator))
            {
                found = true;
                break;
            }
        }

        if (found == false)
            throw new FormatException($"No byte indicator found in value '{s}'.");

        int lastNumber = num;

        // Cut the input string in half
        string numberPart = s[..lastNumber].Trim();
        string sizePart = s[lastNumber..].Trim();

        // Get the numeric part
        if (!double.TryParse(numberPart, style, formatProvider, out var number))
            throw new FormatException($"No number found in value '{s}'.");

        // Get the magnitude part
        if (sizePart == "B")
        {
            if (number % 1 != 0) // Can't have partial bytes
                throw new FormatException($"Can't have partial bytes for value '{s}'.");

            return FromBytes((long)number);
        }

        return sizePart.ToLowerInvariant() switch
        {
            // Binary
            "kib" => FromKibiBytes(number),
            "mib" => FromMebiBytes(number),
            "gib" => FromGibiBytes(number),
            "tib" => FromTebiBytes(number),
            "pib" => FromPebiBytes(number),
            // Decimal
            "kb" => FromKiloBytes(number),
            "mb" => FromMegaBytes(number),
            "gb" => FromGigaBytes(number),
            "tb" => FromTeraBytes(number),
            "pb" => FromPetaBytes(number),
            _ => throw new FormatException($"Bytes of magnitude '{sizePart}' is not supported."),
        };
    }

    /// <summary>
    /// Converts the <see cref="string"/> into a <see cref="ByteSize"/>.
    /// A return value indicates whether the conversion succeeded or failed.
    /// </summary>
    /// <param name="s">A string containing the value to convert.</param>
    /// <param name="result">
    /// When this method returns, contains the <see cref="ByteSize"/> equivalent of the <paramref name="s"/> parameter,
    /// if the conversion succeeded, or default if the conversion failed.
    /// The conversion fails if the <paramref name="s"/> parameter is <see langword="null"/> or <see cref="string.Empty"/>,
    /// is not in a valid format, or represents a value less than <see cref="MinValue"/>
    /// or greater than <see cref="MaxValue"/>. This parameter is passed uninitialized;
    /// any value originally supplied in result will be overwritten.
    /// </param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was converted successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string s, out ByteSize result)
    {
        try
        {
            result = Parse(s);
            return true;
        }
        catch
        {
            result = new ByteSize();
            return false;
        }
    }

    /// <summary>
    /// Converts the <see cref="string"/> into a <see cref="ByteSize"/> in a specified style and culture-specific format.
    /// A return value indicates whether the conversion succeeded or failed.
    /// </summary>
    /// <param name="s">A string containing the value to convert.</param>
    /// <param name="style">
    /// A bitwise combination of <see cref="NumberStyles"/> values that indicates the permitted format of <paramref name="s"/>.
    /// A typical value to specify is <see cref="NumberStyles.Float"/> combined with <see cref="NumberStyles.AllowThousands"/>.
    /// </param>
    /// <param name="formatProvider">
    /// An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about <paramref name="s"/>.
    /// </param>
    /// <param name="result">
    /// When this method returns, contains the <see cref="ByteSize"/> equivalent of the <paramref name="s"/> parameter,
    /// if the conversion succeeded, or default if the conversion failed.
    /// The conversion fails if the <paramref name="s"/> parameter is <see langword="null"/> or <see cref="string.Empty"/>,
    /// is not in a valid format, or represents a value less than <see cref="MinValue"/>
    /// or greater than <see cref="MaxValue"/>. This parameter is passed uninitialized;
    /// any value originally supplied in result will be overwritten.
    /// </param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was converted successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string s, NumberStyles style, IFormatProvider formatProvider, out ByteSize result)
    {
        try
        {
            result = Parse(s, style, formatProvider);
            return true;
        }
        catch
        {
            result = new ByteSize();
            return false;
        }
    }

    #endregion

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
            TypeCode.Object when conversionType == typeof(ByteSize) => this,
            TypeCode.String => ((IConvertible)this).ToString(provider),
            _ => throw new InvalidCastException(),
        };
    }

    ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new InvalidCastException();
    uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new InvalidCastException();
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new InvalidCastException();

    #endregion

    internal class ByteSizeTypeConverter : TypeConverter
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
            return destinationType == typeof(string) && value is ByteSize bs
                ? bs.ToBinaryString()
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
