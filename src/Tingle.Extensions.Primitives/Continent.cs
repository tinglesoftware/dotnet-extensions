using System.ComponentModel;
using System.Globalization;
using System.Text.Json.Serialization;
using Tingle.Extensions.Primitives.Converters;

namespace Tingle.Extensions.Primitives;

/// <summary>
/// Represents a continent
/// </summary>
[JsonConverter(typeof(ContinentJsonConverter))]
[TypeConverter(typeof(ContinentTypeConverter))]
public sealed class Continent : IEquatable<Continent>, IComparable<Continent>, IConvertible
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static readonly Continent Asia = new(name: "Asia");
    public static readonly Continent Europe = new(name: "Europe");
    public static readonly Continent Americas = new(name: "Americas");
    public static readonly Continent Africa = new(name: "Africa");
    public static readonly Continent NorthAmerica = new(name: "North America", otherNames: "NorthAmerica");
    public static readonly Continent SouthAmerica = new(name: "South America", otherNames: "SouthAmerica");
    public static readonly Continent Antarctica = new(name: "Antarctica");
    public static readonly Continent Oceania = new(name: "Oceania");
    public static readonly Continent Australia = new(name: "Australia");
    public static readonly Continent Kenorland = new(name: "Kenorland");
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// A list of known continents
    /// </summary>
    public static readonly Continent[] KnownContinents = [
        Asia,
        Europe,
        Americas,
        Africa,
        NorthAmerica,
        SouthAmerica,
        Antarctica,
        Oceania,
        Australia,
        Kenorland,
    ];

    /// <summary>
    /// Creates an instance of <see cref="Continent"/>
    /// </summary>
    /// <param name="name">the default name</param>
    /// <param name="otherNames">other representations for the name</param>
    public Continent(string name, params string[] otherNames) : this(name, (IEnumerable<string>)otherNames) { }

    /// <summary>
    /// Creates an instance of <see cref="Continent"/>
    /// </summary>
    /// <param name="name">the default name</param>
    /// <param name="otherNames">other representations for the name</param>
    public Continent(string name, IEnumerable<string> otherNames)
    {
        Name = name;
        OtherName = otherNames ?? [];
    }

    /// <summary>
    /// The name of the continent
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Other representations for the name
    /// </summary>
    public IEnumerable<string> OtherName { get; }

    private IEnumerable<string> PossibleNames => new[] { Name }.Concat(OtherName);

    /// <summary>
    /// Checks if a continent is valid. This checks if it is contained in <see cref="KnownContinents"/>.
    /// </summary>
    /// <returns></returns>
    public bool IsKnown() => KnownContinents.Contains(this);

    /// <summary>
    /// Checks if a continent is valid. This checks if it is contained in <see cref="KnownContinents"/>.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static bool IsKnown(Continent c) => c?.IsKnown() ?? false;

    /// <inheritdoc/>
    public bool Equals(Continent? other)
    {
        return other is not null && PossibleNames.Contains(other.Name, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Continent);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Name, OtherName, PossibleNames);

    /// <inheritdoc/>
    public override string ToString() => Name;

    /// <inheritdoc/>
    public int CompareTo(Continent? other) => Name.CompareTo(other?.Name);

    /// <inheritdoc/>
    public static bool operator ==(Continent left, Continent right) => EqualityComparer<Continent>.Default.Equals(left, right);

    /// <inheritdoc/>
    public static bool operator !=(Continent left, Continent right) => !(left == right);

    /// <summary>Converts a <see cref="string"/> to a <see cref="Continent"/>.</summary>
    /// <param name="name"></param>
    public static implicit operator Continent(string name) => new(name: name);

    /// <summary>Converts a <see cref="Continent"/> to a <see cref="string"/>.</summary>
    /// <param name="continent"></param>
    public static implicit operator string(Continent continent) => continent.ToString();

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
            TypeCode.String => ((IConvertible)this).ToString(provider),
            _ => throw new InvalidCastException(),
        };
    }

    ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new InvalidCastException();
    uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new InvalidCastException();
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new InvalidCastException();

    #endregion

    internal class ContinentTypeConverter : TypeConverter
    {
        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string);

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType == typeof(string);

        /// <inheritdoc/>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            return value is string s ? new Continent(s) : base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc/>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            return destinationType == typeof(string) && value is Continent continent
                ? continent.Name
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
