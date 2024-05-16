using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Serialization;
using Tingle.Extensions.Primitives.Converters;

namespace Tingle.Extensions.Primitives;

/// <summary>
/// Represents details about a known currency with support for ISO-4217.
/// </summary>
[DebuggerDisplay("{Code} ({Name})")]
[JsonConverter(typeof(CurrencyJsonConverter))]
[TypeConverter(typeof(CurrencyTypeConverter))]
public sealed class Currency : IEquatable<Currency?>, IComparable<Currency>, IConvertible
{
    internal Currency(string code, string symbol, string symbolNative, string name, string namePlural, string decimalDigits)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        SymbolNative = symbolNative ?? throw new ArgumentNullException(nameof(symbolNative));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        NamePlural = namePlural ?? throw new ArgumentNullException(nameof(namePlural));
        DecimalDigits = int.Parse(decimalDigits ?? throw new ArgumentNullException(nameof(decimalDigits)));
    }

    /// <summary>3 character alphabetic code.</summary>
    public string Code { get; internal init; }

    /// <summary>The symbol associated.</summary>
    public string Symbol { get; internal init; }

    /// <summary>The symbol associated.</summary>
    public string SymbolNative { get; internal init; }

    /// <summary>Official name of the currency.</summary>
    public string Name { get; internal init; }

    ///
    public string NamePlural { get; internal init; }

    ///
    public int DecimalDigits { get; internal init; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Currency);

    /// <inheritdoc/>
    public bool Equals(Currency? other) => other != null && Code == other.Code;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Code);

    /// <inheritdoc/>
    public int CompareTo(Currency? other) => Code.CompareTo(other?.Code);

    /// <inheritdoc/>
    public override string ToString() => Code;

    /// <inheritdoc/>
    public static bool operator ==(Currency? left, Currency? right)
        => EqualityComparer<Currency?>.Default.Equals(left, right);

    /// <inheritdoc/>
    public static bool operator !=(Currency? left, Currency? right) => !(left == right);

    /// <summary>Converts a <see cref="string"/> to a <see cref="Currency"/>.</summary>
    /// <param name="code"></param>
    public static implicit operator Currency(string code) => FromCode(code);

    /// <summary>Converts a <see cref="Currency"/> to a string.</summary>
    /// <param name="c"></param>
    public static implicit operator string(Currency c) => c.ToString();

    ///
    public static Currency FromCulture(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        if (IsInvariantOrNeutral(culture))
            throw new ArgumentException($"Culture '{culture.Name}' is either neutral or invariant hence no region information can be extracted!", nameof(culture));

        return FromCode(new RegionInfo(culture.Name).ISOCurrencySymbol);
    }

    ///
    public static Currency FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException($"'{nameof(code)}' cannot be null or whitespace.", nameof(code));
        }

        if (TryGetFromCode(code, out var currency)) return currency;
        throw new InvalidOperationException($"Currency code '{code}' not found");
    }

    ///
    public static bool TryGetFromCode(string? code, [NotNullWhen(true)] out Currency? currency)
    {
        currency = null;
        return !string.IsNullOrWhiteSpace(code) && Currencies.Map.TryGetValue(code, out currency);
    }

    /// <summary>The known currency codes.</summary>
    public static IEnumerable<string> Codes => Currencies.Map.Keys;

    /// <summary>The known currencies.</summary>
    public static IEnumerable<Currency> All => Currencies.All;

    /// <summary>Gets the <see cref="Currency"/> that represents the country/region used by the current thread.</summary>
    public static Currency? CurrentCurrency
    {
        get
        {
            var ci = CultureInfo.CurrentUICulture;
            return IsInvariantOrNeutral(ci) ? null : FromCulture(ci);
        }
    }

    private static bool IsInvariantOrNeutral(CultureInfo culture)
    {
        return culture.IsNeutralCulture || culture == CultureInfo.InvariantCulture || culture.LCID == 0x7F;
    }

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
            TypeCode.Object when conversionType == typeof(Currency) => this,
            TypeCode.String => ((IConvertible)this).ToString(provider),
            _ => throw new InvalidCastException(),
        };
    }

    ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new InvalidCastException();
    uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new InvalidCastException();
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new InvalidCastException();

    #endregion

    internal class CurrencyTypeConverter : TypeConverter
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
            return destinationType == typeof(string) && value is Currency currency
                ? currency.Code
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

/// <summary>Represents the known currencies.</summary>
internal static partial class Currencies
{
    internal static IReadOnlyDictionary<string, Currency> Map { get; }

    static Currencies()
    {
        Map = All.ToDictionary(c => c.Code, StringComparer.OrdinalIgnoreCase);
    }
}
