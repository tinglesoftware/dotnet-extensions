using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives;

/// <summary>
/// A convenience for working with durations as per the
/// <see href="https://en.wikipedia.org/wiki/ISO_8601">ISO-8601</see>
/// standard for durations.
/// </summary>
/// <remarks>
/// This class is highly borrowed from https://github.com/thomaslevesque/Iso8601DurationHelper
/// </remarks>
[JsonConverter(typeof(DurationJsonConverter))]
[TypeConverter(typeof(DurationTypeConverter))]
public readonly struct Duration : IEquatable<Duration>, IConvertible
{
    /// <summary>Represents the zero <see cref="Duration"/> value.</summary>
    public static readonly Duration Zero = new(0, 0, 0, 0);

    /// <summary>Initializes a new instance of <see cref="Duration"/>.</summary>
    /// <param name="years">Number of years.</param>
    /// <param name="months">Number of months.</param>
    /// <param name="weeks">Number of weeks.</param>
    /// <param name="days">Number of days.</param>
    /// <param name="hours">Number of hours.</param>
    /// <param name="minutes">Number of minutes.</param>
    /// <param name="seconds">Number of seconds.</param>
    public Duration(uint years, uint months, uint weeks, uint days, uint hours, uint minutes, uint seconds)
    {
        Years = years;
        Months = months;
        Weeks = weeks;
        Days = days;
        Hours = hours;
        Minutes = minutes;
        Seconds = seconds;
    }

    /// <summary>Initializes a new instance of <see cref="Duration"/>.</summary>
    /// <param name="years">Number of years.</param>
    /// <param name="months">Number of months.</param>
    /// <param name="weeks">Number of weeks.</param>
    /// <param name="days">Number of days.</param>
    public Duration(uint years, uint months, uint weeks, uint days) : this(years, months, weeks, days, 0, 0, 0) { }

    /// <summary>Initializes a new instance of <see cref="Duration"/> from a <see cref="TimeSpan"/>.</summary>
    /// <param name="time">The <see cref="TimeSpan"/> to use.</param>
    public Duration(TimeSpan time)
    {
        Years = 0;
        Months = 0;
        Weeks = 0;
        Days = (uint)time.Days;
        Hours = (uint)time.Hours;
        Minutes = (uint)time.Minutes;
        Seconds = (uint)time.Seconds;
    }

    #region Properties

    /// <summary>The number of years.</summary>
    public uint Years { get; }

    /// <summary>The number of months.</summary>
    public uint Months { get; }

    /// <summary>The number of weeks.</summary>
    public uint Weeks { get; }

    /// <summary>The number of days.</summary>
    public uint Days { get; }

    /// <summary>The number of hours.</summary>
    public uint Hours { get; }

    /// <summary>The number of minutes.</summary>
    public uint Minutes { get; }

    /// <summary>The number of seconds.</summary>
    public uint Seconds { get; }

    #endregion

    #region Fromxxx(...) methods

    /// <summary>Create a <see cref="Duration"/> from years only.</summary>
    /// <param name="years">The number of years.</param>
    public static Duration FromYears(uint years) => new(years: years, 0, 0, 0);

    /// <summary>Create a <see cref="Duration"/> from months only.</summary>
    /// <param name="months">The number of months.</param>
    public static Duration FromMonths(uint months) => new(0, months: months, 0, 0);

    /// <summary>Create a <see cref="Duration"/> from weeks only.</summary>
    /// <param name="weeks">The number of weeks.</param>
    public static Duration FromWeeks(uint weeks) => new(0, 0, weeks: weeks, 0);

    /// <summary>Create a <see cref="Duration"/> from days only.</summary>
    /// <param name="days">The number of days.</param>
    public static Duration FromDays(uint days) => new(0, 0, 0, days: days);

    /// <summary>Create a <see cref="Duration"/> from hours only.</summary>
    /// <param name="hours">The number of hours.</param>
    public static Duration FromHours(uint hours) => new(0, 0, 0, 0, hours: hours, 0, 0);

    /// <summary>Create a <see cref="Duration"/> from minutes only.</summary>
    /// <param name="minutes">The number of minutes.</param>
    public static Duration FromMinutes(uint minutes) => new(0, 0, 0, 0, 0, minutes: minutes, 0);

    /// <summary>Create a <see cref="Duration"/> from seconds only.</summary>
    /// <param name="seconds">The number of seconds.</param>
    public static Duration FromSeconds(uint seconds) => new(0, 0, 0, 0, 0, 0, seconds: seconds);

    #endregion

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Duration duration && Equals(duration);

    /// <inheritdoc/>
    public bool Equals(Duration other)
    {
        return Years == other.Years &&
               Months == other.Months &&
               Weeks == other.Weeks &&
               Days == other.Days &&
               Hours == other.Hours &&
               Minutes == other.Minutes &&
               Seconds == other.Seconds;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Years, Months, Weeks, Days, Hours, Minutes, Seconds);

    /// <summary>
    /// Converts this <see cref="Duration"/> to its ISO8601 representation.
    /// </summary>
    /// <returns>The ISO8601 representation of this <see cref="Duration"/>.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder("P");

        // Date components
        AppendComponent(Years, 'Y');
        AppendComponent(Months, 'M');
        AppendComponent(Weeks, 'W');
        AppendComponent(Days, 'D');

        // Time separator
        if (Hours != 0 || Minutes != 0 || Seconds != 0)
            sb.Append('T');

        // Time components
        AppendComponent(Hours, 'H');
        AppendComponent(Minutes, 'M');
        AppendComponent(Seconds, 'S');

        // Empty duration
        if (sb.Length == 1)
            sb.Append("0D");

        return sb.ToString();

        void AppendComponent(uint number, char symbol)
        {
            if (number != 0)
                sb.Append(number).Append(symbol);
        }
    }

    #region Parsing

    /// <summary>Converts a <see cref="string"/> in ISO8601 format into a <see cref="Duration"/>.</summary>
    /// <param name="value">A string containing the value to convert.</param>
    /// <returns>A <see cref="Duration"/> equivalent to the value specified in <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
    /// <exception cref="FormatException"><paramref name="value"/> is not in a correct format.</exception>
    public static Duration Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"'{nameof(value)}' cannot be null or whitespace.", nameof(value));
        }

        if (TryParse(value, out var duration))
            return duration;

        throw new FormatException($"'{value}' is not a valid Duration representation.");
    }

    /// <summary>Converts a <see cref="string"/> in ISO8601 format into a <see cref="Duration"/>.</summary>
    /// <param name="value">A string containing the value to convert.</param>
    /// <param name="result">
    /// When this method returns, contains the value associated parsed,
    /// if successful; otherwise, <see langword="null"/> is returned.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="value"/> could be parsed; otherwise, false.
    /// </returns>
    public static bool TryParse(string value, out Duration result)
    {
        result = default;
        if (value == null) return false;
        if (value.Length < 3) return false;
        if (value[0] != DurationChars.Prefix) return false;

        uint years = 0, months = 0, weeks = 0, days = 0, hours = 0, minutes = 0, seconds = 0;

        var lastComponent = DurationComponent.None;
        int position = 1;
        int numberStart = -1;
        var isTimeSpecified = false;

        while (position < value.Length)
        {
            char c = value[position];
            if (c == DurationChars.Time)
            {
                isTimeSpecified = true;
                lastComponent = DurationComponent.Time;
            }
            else if (char.IsLetter(c))
            {
                if (numberStart < 0 || numberStart >= position)
                    return false; // No number preceding letter

                var numberString = value[numberStart..position];
                if (!uint.TryParse(numberString, out uint n))
                    return false; // Not a valid number

                // Check component order
                var component = GetComponent(c, isTimeSpecified);
                if (component == DurationComponent.None) return false; // invalid character
                if (component > DurationComponent.Time && !isTimeSpecified) return false; // Time component before the time specified
                if (component <= lastComponent) return false; // Components in wrong order

                switch (component)
                {
                    case DurationComponent.Years:
                        years = n;
                        break;
                    case DurationComponent.Months:
                        months = n;
                        break;
                    case DurationComponent.Weeks:
                        weeks = n;
                        break;
                    case DurationComponent.Days:
                        days = n;
                        break;
                    case DurationComponent.Hours:
                        hours = n;
                        break;
                    case DurationComponent.Minutes:
                        minutes = n;
                        break;
                    case DurationComponent.Seconds:
                        seconds = n;
                        break;
                }

                numberStart = -1;
                lastComponent = component;
            }
            else if (char.IsDigit(c))
            {
                if (numberStart < 0)
                    numberStart = position;
            }
            else
            {
                // Invalid character
                return false;
            }

            position++;
        }

        if (lastComponent == DurationComponent.None)
            return false; // No component was specified
        if (isTimeSpecified && lastComponent <= DurationComponent.Time)
            return false; // We've seen the time specifier, but no time component was specified

        result = new Duration(years, months, weeks, days, hours, minutes, seconds);
        return true;
    }

    #endregion

    #region Helpers

    private static DurationComponent GetComponent(char c, bool isTimeSpecified)
    {
        return c switch
        {
            DurationChars.Year => DurationComponent.Years,
            DurationChars.Months when !isTimeSpecified => DurationComponent.Months,
            DurationChars.Weeks => DurationComponent.Weeks,
            DurationChars.Days => DurationComponent.Days,
            DurationChars.Time when !isTimeSpecified => DurationComponent.Time,
            DurationChars.Hours => DurationComponent.Hours,
            DurationChars.Minutes when isTimeSpecified => DurationComponent.Minutes,
            DurationChars.Seconds => DurationComponent.Seconds,
            _ => DurationComponent.None,
        };
    }

    private static class DurationChars
    {
        public const char Prefix = 'P';
        public const char Time = 'T';

        public const char Year = 'Y';
        public const char Months = 'M';
        public const char Weeks = 'W';
        public const char Days = 'D';
        public const char Hours = 'H';
        public const char Minutes = 'M';
        public const char Seconds = 'S';
    }

    private enum DurationComponent
    {
        None = 0,
        Years = None + 1,
        Months = Years + 1,
        Weeks = Months + 1,
        Days = Weeks + 1,
        Time = Days + 1,
        Hours = Time + 1,
        Minutes = Hours + 1,
        Seconds = Minutes + 1
    }

    #endregion

    /// <summary>Add a <see cref="Duration"/> to a <see cref="DateTime"/>.</summary>
    /// <param name="value">The <see cref="DateTime"/> to add the duration to.</param>
    /// <param name="duration">The <see cref="Duration"/> to add.</param>
    /// <returns>A new <see cref="DateTime"/> with is the result of the addition.</returns>
    public static DateTime operator +(DateTime value, Duration duration)
    {
        if (duration.Years != 0) value = value.AddYears((int)duration.Years);
        if (duration.Months != 0) value = value.AddMonths((int)duration.Months);
        if (duration.Weeks != 0) value = value.AddDays(7 * (int)duration.Weeks);
        if (duration.Days != 0) value = value.AddDays((int)duration.Days);
        if (duration.Hours != 0) value = value.AddHours((int)duration.Hours);
        if (duration.Minutes != 0) value = value.AddMinutes((int)duration.Minutes);
        if (duration.Seconds != 0) value = value.AddSeconds((int)duration.Seconds);
        return value;
    }

    /// <summary>Subtract a <see cref="Duration"/> from a <see cref="DateTime"/>.</summary>
    /// <param name="value">The <see cref="DateTime"/> to subtract the duration from.</param>
    /// <param name="duration">The <see cref="Duration"/> to subtract.</param>
    /// <returns>A new <see cref="DateTime"/> with is the result of the subtraction.</returns>
    public static DateTime operator -(DateTime value, Duration duration)
    {
        if (duration.Years != 0) value = value.AddYears(-(int)duration.Years);
        if (duration.Months != 0) value = value.AddMonths(-(int)duration.Months);
        if (duration.Weeks != 0) value = value.AddDays(-7 * (int)duration.Weeks);
        if (duration.Days != 0) value = value.AddDays(-(int)duration.Days);
        if (duration.Hours != 0) value = value.AddHours(-(int)duration.Hours);
        if (duration.Minutes != 0) value = value.AddMinutes(-(int)duration.Minutes);
        if (duration.Seconds != 0) value = value.AddSeconds(-(int)duration.Seconds);
        return value;
    }

    /// <summary>Add a <see cref="Duration"/> to a <see cref="DateTimeOffset"/>.</summary>
    /// <param name="value">The <see cref="DateTimeOffset"/> to add the duration to.</param>
    /// <param name="duration">The <see cref="Duration"/> to add.</param>
    /// <returns>A new <see cref="DateTimeOffset"/> with is the result of the addition.</returns>
    public static DateTimeOffset operator +(DateTimeOffset value, Duration duration)
    {
        if (duration.Years != 0) value = value.AddYears((int)duration.Years);
        if (duration.Months != 0) value = value.AddMonths((int)duration.Months);
        if (duration.Weeks != 0) value = value.AddDays(7 * (int)duration.Weeks);
        if (duration.Days != 0) value = value.AddDays((int)duration.Days);
        if (duration.Hours != 0) value = value.AddHours((int)duration.Hours);
        if (duration.Minutes != 0) value = value.AddMinutes((int)duration.Minutes);
        if (duration.Seconds != 0) value = value.AddSeconds((int)duration.Seconds);
        return value;
    }

    /// <summary>Subtract a <see cref="Duration"/> from a <see cref="DateTimeOffset"/>.</summary>
    /// <param name="value">The <see cref="DateTimeOffset"/> to subtract the duration from.</param>
    /// <param name="duration">The <see cref="Duration"/> to subtract.</param>
    /// <returns>A new <see cref="DateTimeOffset"/> with is the result of the subtraction.</returns>
    public static DateTimeOffset operator -(DateTimeOffset value, Duration duration)
    {
        if (duration.Years != 0) value = value.AddYears(-(int)duration.Years);
        if (duration.Months != 0) value = value.AddMonths(-(int)duration.Months);
        if (duration.Weeks != 0) value = value.AddDays(-7 * (int)duration.Weeks);
        if (duration.Days != 0) value = value.AddDays(-(int)duration.Days);
        if (duration.Hours != 0) value = value.AddHours(-(int)duration.Hours);
        if (duration.Minutes != 0) value = value.AddMinutes(-(int)duration.Minutes);
        if (duration.Seconds != 0) value = value.AddSeconds(-(int)duration.Seconds);
        return value;
    }

    /// <inheritdoc/>
    public static bool operator ==(Duration left, Duration right) => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(Duration left, Duration right) => !(left == right);

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
            TypeCode.Decimal => ((IConvertible)this).ToDecimal(provider),
            TypeCode.Double => ((IConvertible)this).ToDouble(provider),
            TypeCode.Int64 => ((IConvertible)this).ToInt64(provider),
            TypeCode.Object when conversionType == typeof(object) => this,
            TypeCode.Object when conversionType == typeof(Duration) => this,
            TypeCode.String => ((IConvertible)this).ToString(provider),
            TypeCode.UInt64 => ((IConvertible)this).ToUInt64(provider),
            _ => throw new InvalidCastException(),
        };
    }

    ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new InvalidCastException();
    uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new InvalidCastException();
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new InvalidCastException();

    #endregion

    internal class DurationJsonConverter : JsonConverter<Duration>
    {
        public DurationJsonConverter() { }

        /// <inheritdoc/>
        public override Duration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return default;
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new InvalidOperationException("Only strings are supported");
            }

            var str = reader.GetString();
            return Parse(str!);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Duration value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    internal class DurationTypeConverter : TypeConverter
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
            return destinationType == typeof(string) && value is Duration duration
                ? duration.ToString()
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
