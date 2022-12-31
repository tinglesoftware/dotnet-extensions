using System.Text.Json.Serialization;
using Tingle.AspNetCore.Tokens;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// Represents a timed continuation token's data.
/// </summary>
/// <typeparam name="T">The type of data contained</typeparam>
[JsonConverter(typeof(ContinuationTokenJsonConverter))]
public class TimedContinuationToken<T> : ContinuationToken<T>, IEquatable<TimedContinuationToken<T>>
{
    private readonly DateTimeOffset expiration;

    /// <summary>Initializes a new instance of <see cref="TimedContinuationToken{T}"/>.</summary>
    /// <param name="value">The value.</param>
    /// <param name="expiration">The time when this token should expire.</param>
    public TimedContinuationToken(T value, DateTimeOffset expiration) : base(value)
    {
        this.expiration = expiration;
    }

    /// <summary>Initializes a new instance of <see cref="TimedContinuationToken{T}"/>.</summary>
    /// <param name="value">The value.</param>
    /// <param name="protected">The original, opaque value.</param>
    /// <param name="expiration">The time when this token should expire.</param>
    internal TimedContinuationToken(T value, string @protected, DateTimeOffset expiration) : base(value, @protected)
    {
        this.expiration = expiration;
    }

    /// <summary>
    /// Gets the time when this token should expire
    /// </summary>
    public DateTimeOffset GetExpiration() => expiration;

    /// <summary>
    /// Compares two instances of the <see cref="TimedContinuationToken{T}"/> for equality.
    /// </summary>
    /// <param name="left">The left value to compare.</param>
    /// <param name="right">The right value to compare.</param>
    /// <returns></returns>
    public static bool operator ==(TimedContinuationToken<T> left, TimedContinuationToken<T> right)
    {
        return EqualityComparer<TimedContinuationToken<T>>.Default.Equals(left, right);
    }

    /// <summary>
    /// Compares two instances of the <see cref="TimedContinuationToken{T}"/> for inequality.
    /// </summary>
    /// <param name="left">The left value to compare.</param>
    /// <param name="right">The right value to compare.</param>
    /// <returns></returns>
    public static bool operator !=(TimedContinuationToken<T> left, TimedContinuationToken<T> right) => !(left == right);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as TimedContinuationToken<T>);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), expiration);

    /// <inheritdoc/>
    public bool Equals(TimedContinuationToken<T>? other)
    {
        return other is not null
            && base.Equals(other)
            && expiration.Equals(other.expiration);
    }
}
