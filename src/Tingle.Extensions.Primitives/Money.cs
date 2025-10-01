using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Serialization;
using Tingle.Extensions.Primitives.Converters;

namespace Tingle.Extensions.Primitives;

/// <summary>
/// A convenience for working with Money pattern as per 
/// <see href="https://martinfowler.com/eaaCatalog/money.html">Martin Fowler's guide</see>
/// </summary>
/// <remarks>Creates an instance of <see cref="Money"/>.</remarks>
/// <param name="currency">Currency for the money.</param>
/// <param name="amount">Amount in the smallest currency unit.</param>
/// <exception cref="ArgumentNullException"></exception>
[JsonConverter(typeof(MoneyJsonConverter))]
[TypeConverter(typeof(MoneyTypeConverter))]
public readonly struct Money(Currency currency, long amount) : IEquatable<Money>, IComparable<Money>, IConvertible, IFormattable, IParsable<Money>
{
    private readonly Currency currency = currency ?? throw new ArgumentNullException(nameof(currency));
    private readonly long amount = amount;

    /// <summary>Creates an instance of <see cref="Money"/>.</summary>
    /// <param name="currencyCode">
    /// Currency code for the money.
    /// This is used with <see cref="Currency.FromCode(string)"/> to get the actual currency.
    /// </param>
    /// <param name="amount">Amount in the smallest currency unit.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Money(string currencyCode, long amount) : this(Currency.FromCode(currencyCode), amount) { }

    /// <summary>Currency for the money.</summary>
    public Currency Currency => currency;

    /// <summary>Amount in the smallest currency unit.</summary>
    public long Amount => amount;

    /// <summary>Deconstructs the current instance into its components.</summary>
    /// <param name="amount">The Amount of money as <see langword="long"/>.</param>
    /// <param name="currency">The Currency of the money.</param>
    public void Deconstruct(out long amount, out Currency currency)
    {
        amount = this.amount;
        currency = this.currency;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Money money && Equals(money);

    /// <inheritdoc/>
    public bool Equals(Money other) => currency == other.currency && amount == other.amount;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(currency, amount);

    /// <inheritdoc/>
    public int CompareTo(Money other)
    {
        if (currency != other.currency) throw new InvalidOperationException("Comparison can only be done for the same currency");
        return amount.CompareTo(other.amount);
    }

    #region ToString

    /// <inheritdoc/>
    public override string ToString() => ToString(null, null);

    /// <inheritdoc/>
    public string ToString(string? format) => ToString(format, null);

    /// <inheritdoc/>
    public string ToString(IFormatProvider? provider) => ToString(null, provider);

    // https://github.com/DynamicHands/NodaMoney/blob/c4c9a621fd002abecb855273581632a6814c0c6c/src/NodaMoney/Money.Formattable.cs
    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? provider)
    {
        if (!string.IsNullOrWhiteSpace(format) && format.StartsWith('I') && format.Length >= 1 && format.Length <= 2)
        {
            format = format.Replace("I", "C", StringComparison.Ordinal);
            provider = GetFormatProvider(currency, provider, true);
        }
        else
        {
            provider = GetFormatProvider(currency, provider);
        }

        if (format == null || format == "G")
        {
            format = "C";
        }

        var amountD = (double)amount;
        if (currency.DecimalDigits > 0) amountD /= Math.Pow(10, currency.DecimalDigits);

        if (format.StartsWith('F'))
        {
            format = format.Replace("F", "N", StringComparison.Ordinal);
            if (format.Length == 1)
            {
                format += currency.DecimalDigits;
            }

            return $"{amountD.ToString(format, provider)} {(amountD == 1 ? currency.Name : currency.NamePlural)}";
        }

        // For "N" format, explicitly specify decimal places to match currency precision
        // The "N" format doesn't automatically use CurrencyDecimalDigits like "C" format does
        if (format == "N")
        {
            format = $"N{currency.DecimalDigits}";
        }

        return amountD.ToString(format ?? "C", provider);
    }

    #endregion

    #region Parsing

    /// <summary>Converts a <see cref="string"/> into a <see cref="Money"/>.</summary>
    /// <param name="s">A string containing the value to convert.</param>
    /// <returns>A <see cref="Money"/> equivalent to the value specified in <paramref name="s"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException"><paramref name="s"/> is not in a correct format.</exception>
    public static Money Parse(string s) => Parse(s, null, null);

    /// <summary>Converts a <see cref="string"/> into a <see cref="Money"/>.</summary>
    /// <param name="s">A string containing the value to convert.</param>
    /// <param name="currency">The currency to use for parsing the string representation.</param>
    /// <returns>A <see cref="Money"/> equivalent to the value specified in <paramref name="s"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException"><paramref name="s"/> is not in a correct format.</exception>
    public static Money Parse(string s, Currency currency) => Parse(s, null, currency);

    /// <summary>Converts a <see cref="string"/> into a <see cref="Money"/>.</summary>
    /// <param name="s">A string containing the value to convert.</param>
    /// <param name="provider">An object that supplies culture-specific parsing information about <paramref name="s"/>.</param>
    /// <returns>A <see cref="Money"/> equivalent to the value specified in <paramref name="s"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException"><paramref name="s"/> is not in a correct format.</exception>
    public static Money Parse(string s, IFormatProvider? provider) => Parse(s, provider, null);

    /// <summary>Converts a <see cref="string"/> into a <see cref="Money"/>.</summary>
    /// <param name="s">A string containing the value to convert.</param>
    /// <param name="provider">An object that supplies culture-specific parsing information about <paramref name="s"/>.</param>
    /// <param name="currency">The currency to use for parsing the string representation.</param>
    /// <returns>A <see cref="Money"/> equivalent to the value specified in <paramref name="s"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is null.</exception>
    /// <exception cref="FormatException"><paramref name="s"/> is not in a correct format.</exception>
    public static Money Parse(string s, IFormatProvider? provider, Currency? currency)
    {
        if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(nameof(s));

        currency ??= ExtractCurrencyFromString(s, out s);
        provider = GetFormatProvider(currency, provider);
        if (TryParse(s, provider, currency, out var result)) return result;
        throw new FormatException($"'{s}' is not a valid Money representation.");
    }

    /// <summary>Converts a <see cref="string"/> into a <see cref="Money"/>.</summary>
    /// <param name="s">A string containing the value to convert.</param>
    /// <param name="result">
    /// When this method returns, contains the value associated parsed,
    /// if successful; otherwise, <see langword="null"/> is returned.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="s"/> could be parsed; otherwise, false.
    /// </returns>
    public static bool TryParse([NotNullWhen(true)] string? s, out Money result) => TryParse(s, null, null, out result);

    /// <summary>Converts a <see cref="string"/> into a <see cref="Money"/>.</summary>
    /// <param name="s">A string containing the value to convert.</param>
    /// <param name="currency">The currency to use for parsing the string representation.</param>
    /// <param name="result">
    /// When this method returns, contains the value associated parsed,
    /// if successful; otherwise, <see langword="null"/> is returned.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <see langword="true"/> if <paramref name="s"/> could be parsed; otherwise, false.
    public static bool TryParse([NotNullWhen(true)] string? s, Currency? currency, out Money result) => TryParse(s, null, currency, out result);

    /// <summary>Converts a <see cref="string"/> into a <see cref="Money"/>.</summary>
    /// <param name="s">A string containing the value to convert.</param>
    /// <param name="provider">An object that supplies culture-specific parsing information about <paramref name="s"/>.</param>
    /// <param name="result">
    /// When this method returns, contains the value associated parsed,
    /// if successful; otherwise, <see langword="null"/> is returned.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <see langword="true"/> if <paramref name="s"/> could be parsed; otherwise, false.
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Money result) => TryParse(s, provider, null, out result);

    /// <summary>Converts a <see cref="string"/> into a <see cref="Money"/>.</summary>
    /// <param name="s">A string containing the value to convert.</param>
    /// <param name="currency">The currency to use for parsing the string representation.</param>
    /// <param name="provider">An object that supplies culture-specific parsing information about <paramref name="s"/>.</param>
    /// <param name="result">
    /// When this method returns, contains the value associated parsed,
    /// if successful; otherwise, <see langword="null"/> is returned.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <see langword="true"/> if <paramref name="s"/> could be parsed; otherwise, false.
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, Currency? currency, out Money result)
    {
        result = default;
        if (currency is null && !TryExtractCurrencyFromString(s, out s, out currency)) return false;

        if (double.TryParse(s, NumberStyles.Currency, GetFormatProvider(currency, provider), out var amountD))
        {
            var amount = Convert.ToInt64(amountD * Math.Pow(10, currency.DecimalDigits));
            result = new Money(currency, amount);
            return true;
        }

        return false;
    }

    #endregion

    #region Helpers

    private static NumberFormatInfo GetFormatProvider(Currency currency, IFormatProvider? provider, bool useCode = false)
    {
        var cc = CultureInfo.CurrentCulture;

        // var numberFormatInfo = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
        var numberFormatInfo = (NumberFormatInfo)cc.NumberFormat.Clone();

        if (provider != null)
        {
            if (provider is CultureInfo ci) numberFormatInfo = (NumberFormatInfo)ci.NumberFormat.Clone();
            if (provider is NumberFormatInfo nfi) numberFormatInfo = (NumberFormatInfo)nfi.Clone();
        }

        numberFormatInfo.CurrencyDecimalDigits = Math.Max(0, currency.DecimalDigits);
        numberFormatInfo.CurrencySymbol = currency.Symbol;

        if (useCode)
        {
            // Replace symbol with the code
            numberFormatInfo.CurrencySymbol = currency.Code;

            // Add spacing to PositivePattern and NegativePattern
            if (numberFormatInfo.CurrencyPositivePattern == 0) // $n
                numberFormatInfo.CurrencyPositivePattern = 2; // $ n
            if (numberFormatInfo.CurrencyPositivePattern == 1) // n$
                numberFormatInfo.CurrencyPositivePattern = 3; // n $

            switch (numberFormatInfo.CurrencyNegativePattern)
            {
                case 0: // ($n)
                    numberFormatInfo.CurrencyNegativePattern = 14; // ($ n)
                    break;
                case 1: // -$n
                    numberFormatInfo.CurrencyNegativePattern = 9; // -$ n
                    break;
                case 2: // $-n
                    numberFormatInfo.CurrencyNegativePattern = 12; // $ -n
                    break;
                case 3: // $n-
                    numberFormatInfo.CurrencyNegativePattern = 11; // $ n-
                    break;
                case 4: // (n$)
                    numberFormatInfo.CurrencyNegativePattern = 15; // (n $)
                    break;
                case 5: // -n$
                    numberFormatInfo.CurrencyNegativePattern = 8; // -n $
                    break;
                case 6: // n-$
                    numberFormatInfo.CurrencyNegativePattern = 13; // n- $
                    break;
                case 7: // n$-
                    numberFormatInfo.CurrencyNegativePattern = 10; // n $-
                    break;
            }
        }

        return numberFormatInfo;
    }

    private static Currency ExtractCurrencyFromString(string value, out string repaired)
    {
        repaired = value;
        var extracted = new string([.. value.ToCharArray().Where(IsNotNumericCharacter())]);

        var current = Currency.CurrentCurrency;
        if (current is not null && extracted.Length == 0) return current;

        var matching = Currency.All.Where(c => c.Symbol == extracted || c.Code == extracted).ToList();
        if (matching.Count == 0) throw new FormatException($"{extracted} is an unknown currency sign or code!");
        if (matching.Count > 1)
        {
            throw new FormatException($"Currency sign {extracted} matches with multiple known currencies! Specify currency or culture explicit.");
        }

        var currency = matching[0];
        repaired = value.Replace(extracted, currency.Symbol); // repair the currency to allow for parsing
        return currency;
    }

    private static bool TryExtractCurrencyFromString([NotNullWhen(true)] string? value, out string? repaired, [MaybeNullWhen(false)] out Currency currency)
    {
        repaired = value;
        currency = default;
        if (string.IsNullOrWhiteSpace(value)) return false;

        var extracted = new string([.. value.ToCharArray().Where(IsNotNumericCharacter())]);
        if (extracted.Length == 0) return false;

        var matching = Currency.All.Where(c => c.Symbol == extracted || c.Code == extracted).ToList();
        if (matching.Count == 0) return false;
        if (matching.Count > 1) return false;

        currency = matching[0];
        repaired = value.Replace(extracted, currency.Symbol); // repair the currency to allow for parsing
        return true;
    }

    private static Func<char, bool> IsNotNumericCharacter()
    {
        return character => !char.IsDigit(character) && !char.IsWhiteSpace(character) && character != '.' && character != ','
            && character != '(' && character != ')' && character != '+' && character != '-';
    }

    #endregion

    /// <inheritdoc/>
    public static bool operator ==(Money left, Money right) => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(Money left, Money right) => !(left == right);

    /// <inheritdoc/>
    public static bool operator <(Money left, Money right) => left.CompareTo(right) < 0;

    /// <inheritdoc/>
    public static bool operator <=(Money left, Money right) => left.CompareTo(right) <= 0;

    /// <inheritdoc/>
    public static bool operator >(Money left, Money right) => left.CompareTo(right) > 0;

    /// <inheritdoc/>
    public static bool operator >=(Money left, Money right) => left.CompareTo(right) >= 0;

    /// <summary>Converts a <see cref="string"/> to a <see cref="Money"/>.</summary>
    /// <param name="s"></param>
    public static implicit operator Money(string s) => Parse(s);

    /// <summary>Converts a <see cref="Money"/> to a string.</summary>
    /// <param name="etag"></param>
    public static implicit operator string(Money etag) => etag.ToString();

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
            TypeCode.Object when conversionType == typeof(Money) => this,
            TypeCode.String => ((IConvertible)this).ToString(provider),
            TypeCode.UInt64 => ((IConvertible)this).ToUInt64(provider),
            _ => throw new InvalidCastException(),
        };
    }

    ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new InvalidCastException();
    uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new InvalidCastException();
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new InvalidCastException();

    #endregion

    internal class MoneyTypeConverter : TypeConverter
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
            return destinationType == typeof(string) && value is Money money
                ? money.ToString()
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
