using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Serialization;
using Tingle.Extensions.Primitives.Converters;
using Tingle.Extensions.Primitives.Properties;

namespace Tingle.Extensions.Primitives;

/// <summary>
/// A builder for connection strings
/// </summary>
[JsonConverter(typeof(ConnectionStringBuilderJsonConverter))]
[TypeConverter(typeof(ConnectionStringBuilderTypeConverter))]
public struct ConnectionStringBuilder : IEquatable<ConnectionStringBuilder>, IConvertible, IEnumerable<KeyValuePair<string, string>>
{
    /// <summary>
    /// An immutable instance of <see cref="ConnectionStringBuilder"/> that is empty.
    /// This is also the default value  of <see cref="ConnectionStringBuilder"/>.
    /// </summary>
    public static readonly ConnectionStringBuilder Empty = new(immutable: true);

    private const char DefaultSegmentsSeparator = ';';
    private const char DefaultValueSeparator = '=';

    private readonly Dictionary<string, string> segments;
    private readonly char segmentsSeparator, valueSeparator;
    private readonly bool immutable;

    /// <summary>
    /// Creates an instance of <see cref="ConnectionStringBuilder"/> from segments.
    /// </summary>
    /// <param name="segments">Segments forming the connection string.</param>
    /// <param name="segmentsSeparator">Character separating segments.</param>
    /// <param name="valueSeparator">Character separating key from value.</param>
    /// <param name="immutable">Whether the instance to be created is immutable.</param>
    public ConnectionStringBuilder(Dictionary<string, string> segments,
                                   char segmentsSeparator = DefaultSegmentsSeparator,
                                   char valueSeparator = DefaultValueSeparator,
                                   bool immutable = false)
    {
        this.segments = segments ?? throw new ArgumentNullException(nameof(segments));
        this.segmentsSeparator = segmentsSeparator;
        this.valueSeparator = valueSeparator;
        this.immutable = immutable;

        // ensure the key and value do not contain the segment and value separators
        foreach (var kvp in segments)
        {
            var key = kvp.Key;
            var value = kvp.Value;
            if (key.Contains(segmentsSeparator)
                || key.Contains(valueSeparator)
                || value.Contains(segmentsSeparator)
                || value.Contains(valueSeparator))
            {
                throw new ArgumentOutOfRangeException(nameof(segments),
                                                      string.Format(Resources.KeyOrValueCannotContainSeparator,
                                                                    segmentsSeparator,
                                                                    valueSeparator));
            }
        }
    }

    /// <summary>
    /// Creates an instance of <see cref="ConnectionStringBuilder"/> that parses the provided connection string into segments
    /// </summary>
    /// <param name="connectionString">Full connection string to be parsed.</param>
    /// <param name="segmentsSeparator">Character that separates segments in a connection string.</param>
    /// <param name="valueSeparator">Character separating key from value.</param>
    /// <param name="immutable">Whether the instance to be created is immutable.</param>
    public ConnectionStringBuilder(string connectionString,
                                   char segmentsSeparator = DefaultSegmentsSeparator,
                                   char valueSeparator = DefaultValueSeparator,
                                   bool immutable = false)
        : this(segments: ExtractSegments(connectionString: connectionString,
                                         segmentsSeparator: segmentsSeparator,
                                         valueSeparator: valueSeparator),
               segmentsSeparator: segmentsSeparator,
               valueSeparator: valueSeparator,
               immutable: immutable)
    {
    }

    /// <summary>
    /// Creates an instance of <see cref="ConnectionStringBuilder"/> that parses the provided connection string into segments
    /// </summary>
    /// <param name="other">The <see cref="ConnectionStringBuilder"/> to copy from.</param>
    /// <param name="segmentsSeparator">Optional character separating segments.</param>
    /// <param name="valueSeparator">Optional character separating key from value.</param>
    /// <param name="immutable">Optional indication whether the new instance to be created is immutable.</param>
    public ConnectionStringBuilder(ConnectionStringBuilder other,
                                   char? segmentsSeparator = null,
                                   char? valueSeparator = null,
                                   bool? immutable = null)
        : this(segments: other.segments,
               segmentsSeparator: segmentsSeparator ?? other.segmentsSeparator,
               valueSeparator: valueSeparator ?? other.valueSeparator,
               immutable: immutable ?? other.immutable)
    {
    }

    /// <summary>
    /// Creates and instance of <see cref="ConnectionStringBuilder"/>.
    /// </summary>
    /// <param name="segmentsSeparator">Character that separates segments in a connection string.</param>
    /// <param name="valueSeparator">Character that separates the key from the value in a segment.</param>
    /// <param name="immutable">Whether the instance to be created is immutable.</param>
    public ConnectionStringBuilder(char segmentsSeparator = DefaultSegmentsSeparator,
                                   char valueSeparator = DefaultValueSeparator,
                                   bool immutable = false)
        : this(segments: new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
               segmentsSeparator: segmentsSeparator,
               valueSeparator: valueSeparator,
               immutable: immutable)
    {
    }

    /// <summary>Gets value indicating if the instance is immutable.</summary>
    public readonly bool Immutable => immutable;

    /// <summary>
    /// Reads a segment's value if it exists and converts to required type
    /// </summary>
    /// <typeparam name="T">the type of item to be returned</typeparam>
    /// <param name="key">the key identifying the segment</param>
    /// <returns></returns>
    public readonly T? GetValue<T>(string key)
    {
        if (TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        return default;
    }

    /// <summary>
    /// Adds the specified key and value.
    /// </summary>
    /// <param name="key">The key of the segment to add.</param>
    /// <param name="value">The value of the segment to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">A segment with the same key already exists.</exception>
    public readonly void Add(string key, string value)
    {
        if (immutable)
        {
            throw new InvalidOperationException(Resources.ImmutableConnectionStringBuilderCannotBeModified);
        }

        segments.Add(key, value);
    }

    /// <summary>
    /// Gets the value associated with the specified segment key.
    /// </summary>
    /// <param name="key">The key of the segment to get.</param>
    /// <param name="value">
    /// When this method returns, contains the value associated with the specified key,
    /// if the key is found; otherwise, <see langword="null"/> is returned.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <returns>
    /// <see langword="true"/> if the <see cref="ConnectionStringBuilder"/> contains a segment with
    /// the specified <paramref name="key"/>; otherwise, false.
    /// </returns>
    public readonly bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => segments.TryGetValue(key, out value);

    /// <summary>
    /// Gets or sets the value associated with the specified key
    /// </summary>
    /// <param name="key">The key of the segment to get or set.</param>
    /// <returns>
    /// The value associated with the specified key. If the specified key is not found,
    /// a get operation throws a <see cref="KeyNotFoundException"/>, and
    /// a set operation creates a new segment with the specified key.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">The key does not exist in the connection string or its segments.</exception>
    public readonly string? this[string key]
    {
        get => segments.TryGetValue(key, out var value) ? value : null;
        set
        {
            if (immutable)
            {
                throw new InvalidOperationException(Resources.ImmutableConnectionStringBuilderCannotBeModified);
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            // ensure the key and value do not contain the segment and value separators
            if (key.Contains(segmentsSeparator)
                || key.Contains(valueSeparator)
                || value.Contains(segmentsSeparator)
                || value.Contains(valueSeparator))
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                                                      string.Format(Resources.KeyOrValueCannotContainSeparator,
                                                                    segmentsSeparator,
                                                                    valueSeparator));
            }

            segments[key] = value.ToString();
        }
    }

    #region Extraction

    private static Dictionary<string, string> ExtractSegments(string connectionString, char segmentsSeparator, char valueSeparator)
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        if (string.IsNullOrWhiteSpace(connectionString)) return new Dictionary<string, string>(comparer);

        return connectionString.Split(new char[] { segmentsSeparator }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => ExtractSegment(segment: s, valueSeparator: valueSeparator))
                               .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, comparer);
    }

    private static KeyValuePair<string, string> ExtractSegment(string segment, char valueSeparator)
    {
        if (string.IsNullOrWhiteSpace(segment))
        {
            throw new ArgumentException($"'{nameof(segment)}' cannot be null or whitespace.", nameof(segment));
        }

        var index = segment.IndexOf(valueSeparator);
        var key = segment[..index];
        var value = segment.Substring(index + 1, segment.Length - index - 1);
        return new KeyValuePair<string, string>(key, value);
    }

    #endregion

    #region Convenience

    private const string SegmentNameKey = "Key";
    private const string SegmentNameScheme = "Scheme";
    private const string SegmentNameHostname = "Hostname";

    /// <summary>Gets the value for <c>Key</c> used in authentication.</summary>
    public readonly string? GetKey() => GetValue<string>(SegmentNameKey);

    /// <summary>Sets value for <c>Key</c> used in authentication.</summary>
    /// <param name="key"></param>
    public ConnectionStringBuilder WithKey(string key)
    {
        this[SegmentNameKey] = key;
        return this;
    }

    /// <summary>Gets the value for <c>Scheme</c> used in making requests.</summary>
    public readonly string? GetScheme() => GetValue<string>(SegmentNameScheme);

    /// <summary>Gets value indicating if the <c>Scheme</c> for making requests is set to <paramref name="scheme"/>.</summary>
    /// <param name="scheme"></param>
    public readonly bool IsScheme(string scheme) => string.Equals(scheme, GetScheme());

    /// <summary>Gets value indicating if the <c>Scheme</c> for making requests is set to <c>https</c>.</summary>
    public readonly bool IsHttpsScheme() => IsScheme(Uri.UriSchemeHttps);

    /// <summary>Gets value indicating if the <c>Scheme</c> for making requests is set to <c>http</c>.</summary>
    public readonly bool IsHttpScheme() => IsScheme(Uri.UriSchemeHttp);

    /// <summary>Sets value for <c>Scheme</c> used in making requests.</summary>
    /// <param name="scheme"></param>
    public ConnectionStringBuilder WithScheme(string scheme)
    {
        this[SegmentNameScheme] = scheme;
        return this;
    }

    /// <summary>Sets value for <c>Scheme</c> used in making requests to <c>https</c>.</summary>
    public ConnectionStringBuilder WithHttpsScheme()
        => WithScheme(Uri.UriSchemeHttps);

    /// <summary>Sets value for <c>Scheme</c> used in making requests to <c>http</c>.</summary>
    public ConnectionStringBuilder WithHttpScheme()
        => WithScheme(Uri.UriSchemeHttp);

    /// <summary>Gets the value for <c>Hostname</c> (usually the FQDN with the port, if needed) used in making the requests.</summary>
    public readonly string? GetHostname() => GetValue<string>(SegmentNameHostname);

    /// <summary>Sets the value for <c>Hostname</c> (usually the FQDN with the port, if needed) used in making the requests.</summary>
    /// <param name="hostname"></param>
    public ConnectionStringBuilder WithHostname(string hostname)
    {
        this[SegmentNameHostname] = hostname;
        return this;
    }

    /// <summary>
    /// Sets the value for <c>Hostname</c> (usually the FQDN with the port, if needed) and <c>Scheme</c> used in making the requests,
    /// using <see cref="Uri"/>.
    /// </summary>
    /// <param name="endpoint"></param>
    public ConnectionStringBuilder WithHostnameAndSchemeFromEndpoint(Uri endpoint)
    {
        // set the scheme
        WithScheme(endpoint.Scheme);

        // set the host name
        var hostname = endpoint.Host;
        if (!endpoint.IsDefaultPort)
        {
            hostname += $":{endpoint.Port}";
        }
        WithHostname(hostname);

        return this;
    }

    /// <summary>
    /// Sets the value for <c>Hostname</c> (usually the FQDN with the port, if needed) and <c>Scheme</c> used in making the requests,
    /// using a <see cref="string"/> endpoint parsed into a <see cref="Uri"/>.
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="kind"></param>
    public ConnectionStringBuilder WithHostnameAndSchemeFromEndpoint(string endpoint, UriKind kind = UriKind.Absolute)
    {
        // parse the endpoint into an Uri
        var uri = new Uri(endpoint, kind);
        return WithHostnameAndSchemeFromEndpoint(uri);
    }

    #endregion

    /// <summary>
    /// Returns the built connection string.
    /// </summary>
    public override readonly string ToString()
    {
        var vs = valueSeparator;
        return string.Join($"{segmentsSeparator}", segments.Select(kvp => $"{kvp.Key}{vs}{kvp.Value}"));
    }

    /// <inheritdoc/>
    public override readonly bool Equals(object? obj) => obj is ConnectionStringBuilder builder && Equals(builder);

    /// <inheritdoc/>
    public readonly bool Equals(ConnectionStringBuilder other) => string.Equals(ToString(), other.ToString());

    /// <inheritdoc/>
    public override readonly int GetHashCode() => HashCode.Combine(segments, segmentsSeparator, valueSeparator);

    /// <summary>
    /// Compares two <see cref="ConnectionStringBuilder"/>s.
    /// </summary>
    /// <param name="left">left operand</param>
    /// <param name="right">right operand</param>
    /// <returns></returns>
    public static bool operator ==(ConnectionStringBuilder left, ConnectionStringBuilder right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="ConnectionStringBuilder"/>s.
    /// </summary>
    /// <param name="left">left operand</param>
    /// <param name="right">right operand</param>
    /// <returns></returns>
    public static bool operator !=(ConnectionStringBuilder left, ConnectionStringBuilder right) => !(left == right);

    /// <summary>
    /// Converts a <see cref="string"/> to a <see cref="ConnectionStringBuilder"/>
    /// </summary>
    /// <param name="connectionString"></param>
    public static implicit operator ConnectionStringBuilder(string connectionString)
        => new(connectionString: connectionString);

    /// <summary>
    /// Converts a <see cref="ConnectionStringBuilder"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="builder"></param>
    public static implicit operator string(ConnectionStringBuilder builder) => builder.ToString();

    #region IEnumerable<T>

    /// <inheritdoc/>
    public readonly IEnumerator<KeyValuePair<string, string>> GetEnumerator() => segments.GetEnumerator();

    readonly IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)segments).GetEnumerator();

    #endregion

    #region IConvertible

    readonly TypeCode IConvertible.GetTypeCode() => TypeCode.Object;
    readonly bool IConvertible.ToBoolean(IFormatProvider? provider) => throw new InvalidCastException();
    readonly byte IConvertible.ToByte(IFormatProvider? provider) => throw new InvalidCastException();
    readonly char IConvertible.ToChar(IFormatProvider? provider) => throw new InvalidCastException();
    readonly DateTime IConvertible.ToDateTime(IFormatProvider? provider) => throw new InvalidCastException();
    readonly decimal IConvertible.ToDecimal(IFormatProvider? provider) => throw new InvalidCastException();
    readonly double IConvertible.ToDouble(IFormatProvider? provider) => throw new InvalidCastException();
    readonly short IConvertible.ToInt16(IFormatProvider? provider) => throw new InvalidCastException();
    readonly int IConvertible.ToInt32(IFormatProvider? provider) => throw new InvalidCastException();
    readonly long IConvertible.ToInt64(IFormatProvider? provider) => throw new InvalidCastException();
    readonly sbyte IConvertible.ToSByte(IFormatProvider? provider) => throw new InvalidCastException();
    readonly float IConvertible.ToSingle(IFormatProvider? provider) => throw new InvalidCastException();
    readonly string IConvertible.ToString(IFormatProvider? provider) => ToString();

    readonly object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
    {
        return Type.GetTypeCode(conversionType) switch
        {
            TypeCode.String => ((IConvertible)this).ToString(provider),
            _ => throw new InvalidCastException(),
        };
    }

    readonly ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new InvalidCastException();
    readonly uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new InvalidCastException();
    readonly ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new InvalidCastException();

    #endregion

    internal class ConnectionStringBuilderTypeConverter : TypeConverter
    {
        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string);

        /// <inheritdoc/>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            return value is string s ? new ConnectionStringBuilder(s) : base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType == typeof(string);

        /// <inheritdoc/>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            return value is ConnectionStringBuilder csb && destinationType == typeof(string)
                ? csb.ToString()
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
