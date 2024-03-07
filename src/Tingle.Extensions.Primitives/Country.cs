using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives;

/// <summary>
/// Represents a country
/// </summary>
[DebuggerDisplay("{ThreeLetterCode} ({Name})")]
[JsonConverter(typeof(CountryJsonConverter))]
[TypeConverter(typeof(CountryTypeConverter))]
public sealed class Country : IEquatable<Country>, IComparable<Country>, IConvertible
{
    internal Country(string name, string continent, string numericCode, string twoLetterCode, string threeLetterCode, string flagUrl, string currencyCode)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Continent = continent ?? throw new ArgumentNullException(nameof(continent));
        NumericCode = numericCode ?? throw new ArgumentNullException(nameof(numericCode));
        TwoLetterCode = twoLetterCode ?? throw new ArgumentNullException(nameof(twoLetterCode));
        ThreeLetterCode = threeLetterCode ?? throw new ArgumentNullException(nameof(threeLetterCode));
        FlagUrl = flagUrl ?? throw new ArgumentNullException(nameof(flagUrl));
        CurrencyCode = currencyCode ?? throw new ArgumentNullException(nameof(currencyCode));
    }

    /// <summary>Official name of the country.</summary>
    /// <example>KENYA</example>
    public string Name { get; internal init; }

    /// <summary>Name of continent that the country belongs to.</summary>
    /// <example>AFRICA</example>
    public string Continent { get; internal init; }

    ///
    public string NumericCode { get; internal init; }

    /// <summary>2-letter ISO-3166-3 code for the country.</summary>
    /// <example>KE</example>
    public string TwoLetterCode { get; internal init; }

    /// <summary>3-letter ISO-3166-3 code for the country.</summary>
    /// <example>KEN</example>
    public string ThreeLetterCode { get; internal init; }

    /// <summary>URL for the country's flag usually in SVG format.</summary>
    public string FlagUrl { get; internal init; }

    /// <summary>3-letter ISO-4217 currency code for the country.</summary>
    /// <example>KES</example>
    public string CurrencyCode { get; internal init; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Country);

    /// <inheritdoc/>
    public bool Equals(Country? other) => other != null && ThreeLetterCode == other.ThreeLetterCode;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(TwoLetterCode, ThreeLetterCode);

    /// <inheritdoc/>
    public int CompareTo(Country? other) => ThreeLetterCode.CompareTo(other?.ThreeLetterCode);

    /// <inheritdoc/>
    public override string ToString() => ThreeLetterCode;

    /// <inheritdoc/>
    public static bool operator ==(Country? left, Country? right)
        => EqualityComparer<Country?>.Default.Equals(left, right);

    /// <inheritdoc/>
    public static bool operator !=(Country? left, Country? right) => !(left == right);

    /// <summary>Converts a <see cref="string"/> to a <see cref="Country"/>.</summary>
    /// <param name="code"></param>
    public static implicit operator Country(string code) => FromCode(code);

    /// <summary>Converts a <see cref="Country"/> to a string.</summary>
    /// <param name="c"></param>
    public static implicit operator string(Country c) => c.ToString();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code">Either the 2-letter or 3-letter code.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static Country FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException($"'{nameof(code)}' cannot be null or whitespace.", nameof(code));
        }

        if (TryGetFromCode(code, out var country)) return country;
        throw new InvalidOperationException($"Country code '{code}' not found");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code">Either the 2-letter or 3-letter code.</param>
    /// <param name="country"></param>
    /// <returns></returns>
    public static bool TryGetFromCode(string? code, [NotNullWhen(true)] out Country? country)
    {
        country = null;
        if (string.IsNullOrWhiteSpace(code)) return false;

        return Countries.MapNumeric.TryGetValue(code, out country)
            || Countries.MapTwoLetter.TryGetValue(code, out country)
            || Countries.MapThreeLetter.TryGetValue(code, out country);
    }

    /// <summary>The known numeric country codes.</summary>
    public static IEnumerable<string> NumericCodes => Countries.MapNumeric.Keys;

    /// <summary>The known 2-letter country codes.</summary>
    public static IEnumerable<string> TwoLetterCodes => Countries.MapTwoLetter.Keys;

    /// <summary>The known 3-letter country codes.</summary>
    public static IEnumerable<string> ThreeLetterCodes => Countries.MapThreeLetter.Keys;

    /// <summary>The known countries.</summary>
    public static IEnumerable<Country> All => Countries.All;

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
            TypeCode.Object when conversionType == typeof(Country) => this,
            TypeCode.String => ((IConvertible)this).ToString(provider),
            _ => throw new InvalidCastException(),
        };
    }

    ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new InvalidCastException();
    uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new InvalidCastException();
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new InvalidCastException();

    #endregion

    internal class CountryJsonConverter : JsonConverter<Country>
    {
        /// <inheritdoc/>
        public override Country? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            return string.IsNullOrWhiteSpace(s) ? null : FromCode(s);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Country value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ThreeLetterCode);
        }
    }

    internal class CountryTypeConverter : TypeConverter
    {
        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string);

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType == typeof(string);

        /// <inheritdoc/>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            return value is string s ? FromCode(s) : base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc/>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            return destinationType == typeof(string) && value is Country country
                ? country.ThreeLetterCode
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

/// <summary>Represents the known countries.</summary>
internal static partial class Countries
{
    internal static IReadOnlyDictionary<string, Country> MapNumeric { get; }
    internal static IReadOnlyDictionary<string, Country> MapTwoLetter { get; }
    internal static IReadOnlyDictionary<string, Country> MapThreeLetter { get; }

    static Countries()
    {
        MapNumeric = All.ToDictionary(c => c.NumericCode, StringComparer.OrdinalIgnoreCase);
        MapTwoLetter = All.ToDictionary(c => c.TwoLetterCode, StringComparer.OrdinalIgnoreCase);
        MapThreeLetter = All.ToDictionary(c => c.ThreeLetterCode, StringComparer.OrdinalIgnoreCase);
    }
}
