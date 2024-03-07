using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives;

/// <summary>
/// Represents details about a known language.
/// </summary>
[DebuggerDisplay("{ThreeLetterCode} ({Name})")]
[JsonConverter(typeof(LanguageJsonConverter))]
[TypeConverter(typeof(LanguageTypeConverter))]
public sealed class Language : IEquatable<Language?>, IComparable<Language>, IConvertible
{
    internal Language(string name, string threeLetterCode, string? twoLetterCode, LanguageType type, LanguageScope scope)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ThreeLetterCode = threeLetterCode ?? throw new ArgumentNullException(nameof(threeLetterCode));
        TwoLetterCode = twoLetterCode;
        Type = type;
        Scope = scope;
    }

    /// <summary>Official name of the language.</summary>
    public string Name { get; internal init; }

    /// <summary>3-letter ISO-639-3 code for the language.</summary>
    /// <example>eng</example>
    public string ThreeLetterCode { get; internal init; }

    /// <summary>2-letter ISO-639-1 code for the language, if assigned.</summary>
    /// <example>en</example>
    public string? TwoLetterCode { get; internal init; }

    /// <summary>Type of language.</summary>
    /// <seealso href="https://iso639-3.sil.org/about/types"/>
    public LanguageType Type { get; }

    /// <summary>Scope of the language definition.</summary>
    /// <seealso href="https://iso639-3.sil.org/about/scope"/>
    public LanguageScope Scope { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Language);

    /// <inheritdoc/>
    public bool Equals(Language? other) => other != null && ThreeLetterCode == other.ThreeLetterCode;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(ThreeLetterCode);

    /// <inheritdoc/>
    public int CompareTo(Language? other) => ThreeLetterCode.CompareTo(other?.ThreeLetterCode);

    /// <inheritdoc/>
    public override string ToString() => ThreeLetterCode;

    /// <inheritdoc/>
    public static bool operator ==(Language? left, Language? right)
        => EqualityComparer<Language?>.Default.Equals(left, right);

    /// <inheritdoc/>
    public static bool operator !=(Language? left, Language? right) => !(left == right);

    /// <summary>Converts a <see cref="string"/> to a <see cref="Language"/>.</summary>
    /// <param name="code"></param>
    public static implicit operator Language(string code) => FromCode(code);

    /// <summary>Converts a <see cref="Language"/> to a string.</summary>
    /// <param name="c"></param>
    public static implicit operator string(Language c) => c.ToString();

    ///
    public static Language FromCulture(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        if (IsInvariantOrNeutral(culture))
            throw new ArgumentException($"Culture '{culture.Name}' is either neutral or invariant hence no region information can be extracted!", nameof(culture));

        return FromCode(culture.ThreeLetterISOLanguageName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code">Either the 2-letter or 3-letter code.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static Language FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException($"'{nameof(code)}' cannot be null or whitespace.", nameof(code));
        }

        if (TryGetFromCode(code, out var language)) return language;
        throw new InvalidOperationException($"Language code '{code}' not found");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code">Either the 2-letter or 3-letter code.</param>
    /// <param name="language"></param>
    /// <returns></returns>
    public static bool TryGetFromCode(string? code, [NotNullWhen(true)] out Language? language)
    {
        language = null;
        if (string.IsNullOrWhiteSpace(code)) return false;

        return Languages.MapTwoLetter.TryGetValue(code, out language)
            || Languages.MapThreeLetter.TryGetValue(code, out language);
    }

    /// <summary>The known 2-letter language codes.</summary>
    public static IEnumerable<string> TwoLetterCodes => Languages.MapTwoLetter.Keys;

    /// <summary>The known 3-letter language codes.</summary>
    public static IEnumerable<string> ThreeLetterCodes => Languages.MapThreeLetter.Keys;

    /// <summary>The known languages.</summary>
    public static IEnumerable<Language> All => Languages.All;

    /// <summary>Gets the <see cref="Language"/> that represents the language used by the current thread.</summary>
    public static Language? CurrentLanguage
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
        return System.Type.GetTypeCode(conversionType) switch
        {
            TypeCode.Object when conversionType == typeof(object) => this,
            TypeCode.Object when conversionType == typeof(Language) => this,
            TypeCode.String => ((IConvertible)this).ToString(provider),
            _ => throw new InvalidCastException(),
        };
    }

    ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new InvalidCastException();
    uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new InvalidCastException();
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new InvalidCastException();

    #endregion

    /// <summary>Describes a ISO-639 language type.</summary>
    public enum LanguageType
    {
        /// <summary>
        /// A language that existed in ancient times (i.e. pre-5th century)
        /// </summary>
        /// <seealso href="https://en.wikipedia.org/wiki/Ancient_language"/>
        Ancient,

        /// <summary>
        /// An artificially created or invented language, not developed naturally.
        /// </summary>
        /// <seealso href="https://en.wikipedia.org/wiki/Constructed_language"/>
        Constructed,

        /// <summary>
        /// A language that no longer exists in spoken form.
        /// </summary>
        /// <seealso href="https://en.wikipedia.org/wiki/Extinct_language"/>
        Extinct,

        /// <summary>
        /// A language that were spoken in a historical period, but differ from their moder form.
        /// </summary>
        /// <seealso href="https://en.wikipedia.org/wiki/Historical_language"/>
        Historical,

        /// <summary>
        /// A modern language still in use.
        /// </summary>
        /// <seealso href="https://en.wikipedia.org/wiki/Modern_language"/>
        Living,

        /// <summary>
        /// A language with a specific and/or context-dependent type.
        /// </summary>
        Special
    }

    /// <summary>Describes the scope of a language.</summary>
    /// <seealso href="https://iso639-3.sil.org/about/scope"/>
    public enum LanguageScope
    {
        /// <summary>
        /// Represents a group or parent of a individual languages.
        /// </summary>
        Collective,

        /// <summary>
        /// A distinct individual language.
        /// </summary>
        Individual,

        /// <summary>
        /// A language that is defined locally, and may be different outside of that context.
        /// </summary>
        Local,

        /// <summary>
        /// A super-set definition of a more basic defined language (i.e. same language, different locale).
        /// </summary>
        MacroLanguage,

        /// <summary>
        /// A generic non-specific scope that is context-dependent.
        /// </summary>
        Special
    }

    internal class LanguageJsonConverter : JsonConverter<Language>
    {
        /// <inheritdoc/>
        public override Language? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            return string.IsNullOrWhiteSpace(s) ? null : FromCode(s);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Language value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ThreeLetterCode);
        }
    }

    internal class LanguageTypeConverter : TypeConverter
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
            return destinationType == typeof(string) && value is Language language
                ? language.ThreeLetterCode
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

/// <summary>Represents the known language codes.</summary>
internal static partial class Languages
{
    internal static IReadOnlyDictionary<string, Language> MapTwoLetter { get; }
    internal static IReadOnlyDictionary<string, Language> MapThreeLetter { get; }

    static Languages()
    {
        MapTwoLetter = All.Where(p => !string.IsNullOrWhiteSpace(p.TwoLetterCode)).ToDictionary(c => c.TwoLetterCode!, StringComparer.OrdinalIgnoreCase);
        MapThreeLetter = All.ToDictionary(c => c.ThreeLetterCode, StringComparer.OrdinalIgnoreCase);
    }
}
