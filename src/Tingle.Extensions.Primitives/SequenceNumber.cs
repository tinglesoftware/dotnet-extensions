using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives;

/// <summary>
/// A sequential number inspired by Twitter's (late) Snowflake project and Instagram's implementation.
/// </summary>
/// <remarks>
/// This struct implementation is backed by <see cref="long"/> which can be used to store in the database
/// engine of choice using either <see cref="ulong"/> or <see cref="long"/> because the sign bit is never set.
/// <br/>
/// The 64-bit number is arranged as: <br/>
/// | Range   |  Count  |                Value              | <br/>
/// | ------- | ------- | --------------------------------- | <br/>
/// | 63      |  1 bit  | Sign bit  (always 0 for positive) | <br/>
/// | 21 - 62 | 41 bits |   Timestamp (since 2015-01-01)    | <br/>
/// | 12 - 21 | 10 bits |          Generator (Random)       | <br/>
/// |  0 - 11 | 12 bits |              Sequence             | <br/>
/// <br/>
/// The range for the timestamp component is zero (0) to 2,199,023,255,551.
/// Using ticks for the timestamp would not bit the usable 63 bits, whereas using seconds only would give
/// a very wide range that we do not need without a high enough precision. As such, using milliseconds is ideal.
/// <br/>
/// When using milliseconds, we have a range of 25,451 days or approximately 69 years. Therefore, instead of using
/// the standard Epoch (<c>1970-01-01T00:00:00+00:00</c>), we advance 15 years ahead, as Instagram did, to offset from
/// <c>2015-01-01T00:00:00+00:00</c>.
/// </remarks>
[JsonConverter(typeof(SequenceNumberJsonConverter))]
[TypeConverter(typeof(SequenceNumberTypeConverter))]
public readonly struct SequenceNumber : IComparable<SequenceNumber>, IEquatable<SequenceNumber>, IConvertible, IFormattable
{
    /// <summary>Gets an instance of SequenceNumber where the value is empty.</summary>
    public static readonly SequenceNumber Empty = default;

    private static readonly DateTimeOffset origin = new(2015, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
    private static readonly ushort generatorId = MakeGenerator();
    private static int __staticSequence = new Random().Next();

    private readonly long value;

    /// <summary>
    /// Creates a <see cref="SequenceNumber"/> from a <see cref="long"/>.
    /// This intended for converters (e.g. from/to Json or a database engine) where the value is fixed.
    /// </summary>
    public SequenceNumber(long value)
    {
        this.value = value;
    }

    /// <summary>Creates a <see cref="SequenceNumber"/> from parts.</summary>
    internal SequenceNumber(DateTimeOffset timestamp, ushort generator, ushort sequence)
        : this(GetTimestampFromDateTime(timestamp), generator, sequence) { }

    /// <summary>Creates a <see cref="SequenceNumber"/> from parts.</summary>
    internal SequenceNumber(long timestamp, ushort generator, ushort sequence)
    {
        if (((ulong)timestamp & 0XFFFF_FE00_0000_0000) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timestamp), "The timestamp must be between 0 and 2199023255551 (it must fit in 41 bits)");
        }

        if ((generator & 0XFC00) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(generator), "The generator must be between 0 and 1023 (it must fit in 10 bits)");
        }

        if ((sequence & 0XF000) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sequence), "The sequence must be between 0 and 4095 (it must fit in 12 bits)");
        }

        value = timestamp << 22 | (long)generator << 12 | sequence;
    }

    /// <summary>
    /// Gets the raw backing value.
    /// This intended for converters (e.g. from/to Json or a database engine) where the value is fixed.
    /// </summary>
    public long Value => value;

    /// <summary>Gets the timestamp represented in the instance.</summary>
    public long Timestamp => value >> 22;

    /// <summary>Gets the timestamp represented in the instance.</summary>
    public DateTimeOffset Created => origin.AddMilliseconds(Timestamp);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SequenceNumber number && Equals(number);

    /// <inheritdoc/>
    public bool Equals(SequenceNumber other) => value == other.value;

    /// <inheritdoc/>
    public override int GetHashCode() => value.GetHashCode();

    /// <inheritdoc/>
    public override string ToString() => value.ToString();

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider) => value.ToString(format, formatProvider);

    /// <inheritdoc/>
    public int CompareTo(SequenceNumber other) => value.CompareTo(other.value);

    /// <inheritdoc/>
    public static bool operator ==(SequenceNumber left, SequenceNumber right) => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(SequenceNumber left, SequenceNumber right) => !(left == right);

    /// <inheritdoc/>
    public static bool operator <(SequenceNumber left, SequenceNumber right) => left.CompareTo(right) < 0;

    /// <inheritdoc/>
    public static bool operator <=(SequenceNumber left, SequenceNumber right) => left.CompareTo(right) <= 0;

    /// <inheritdoc/>
    public static bool operator >=(SequenceNumber left, SequenceNumber right) => left.CompareTo(right) >= 0;

    /// <inheritdoc/>
    public static bool operator >(SequenceNumber left, SequenceNumber right) => left.CompareTo(right) > 0;

    /// <summary>Converts a <see cref="long"/> to a <see cref="SequenceNumber"/>.</summary>
    public static implicit operator SequenceNumber(long value) => new(value: value);

    /// <summary>Converts a <see cref="SequenceNumber"/> to a <see cref="long"/>.</summary>
    public static implicit operator long(SequenceNumber etag) => etag.value;

    /// <summary>Generates a new <see cref="SequenceNumber"/> with a unique value.</summary>
    public static SequenceNumber Generate() => Generate(DateTimeOffset.UtcNow);

    /// <summary>
    /// Generates a new <see cref="SequenceNumber"/> with a unique value and the timestamp component based on a given <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp component.</param>
    public static SequenceNumber Generate(DateTimeOffset timestamp) => Generate(GetTimestampFromDateTime(timestamp));

    /// <summary>
    /// Generates a new <see cref="SequenceNumber"/> with a unique value and the timestamp component based on a given <see cref="long"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp component.</param>
    internal static SequenceNumber Generate(long timestamp)
    {
        var sequence = Interlocked.Increment(ref __staticSequence) & 0x00000fff; // only use low order 12 bits
        if (sequence < 0 || sequence > 0xFFF)
        {
            throw new InvalidOperationException("The sequence value must be between 0 and 4095 (it must fit in 12 bits).");
        }

        return new SequenceNumber(timestamp, generatorId, (ushort)(short)sequence);
    }

    private static ushort MakeGenerator()
    {
        var machineHash = Environment.MachineName.GetHashCode() & 0x00ffffff; // use first 3 bytes of hash
        var seed = (int)DateTimeOffset.UtcNow.Ticks ^ machineHash ^ (short)Environment.ProcessId;
        var random = new Random(seed);
        var value = random.Next();
        value &= 0x03FF; // only use low order 10 bits
        return (ushort)value;
    }

    private static long GetTimestampFromDateTime(DateTimeOffset timestamp)
    {
        return timestamp < origin
            ? throw new ArgumentOutOfRangeException(nameof(timestamp), $"The timestamp must be after ${origin:O}")
            : Convert.ToInt64((timestamp - origin).TotalMilliseconds);
    }

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
    long IConvertible.ToInt64(IFormatProvider? provider) => value;
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
            TypeCode.Object when conversionType == typeof(SequenceNumber) => this,
            TypeCode.Object when conversionType == typeof(byte[]) => BitConverter.GetBytes(value),
            TypeCode.String => ((IConvertible)this).ToString(provider),
            TypeCode.UInt64 => ((IConvertible)this).ToUInt64(provider),
            _ => throw new InvalidCastException(),
        };
    }

    ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new InvalidCastException();
    uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new InvalidCastException();
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => (ulong)value;

    #endregion

    internal class SequenceNumberJsonConverter : JsonConverter<SequenceNumber>
    {
        /// <inheritdoc/>
        public override SequenceNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new InvalidOperationException("Only numbers are supported");
            }

            var value = reader.GetInt64();
            return new SequenceNumber(value);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, SequenceNumber value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Value);
        }
    }

    internal class SequenceNumberTypeConverter : TypeConverter
    {
        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(long);

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType == typeof(long);

        /// <inheritdoc/>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is long l) return (object?)new SequenceNumber(l);
            else if (value is string s) return (object?)new SequenceNumber(long.Parse(s));
            else return base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc/>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            return destinationType == typeof(long) && value is SequenceNumber sn
                ? sn.Value
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
