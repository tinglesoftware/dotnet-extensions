using System.Text.Json.Serialization;
using Tingle.AspNetCore.Tokens;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// Represents a continuation token's data
/// </summary>
/// <typeparam name="T">The type of data contained</typeparam>
/// <param name="value">The value.</param>
[JsonConverter(typeof(ContinuationTokenJsonConverter))]
public class ContinuationToken<T>(T value) : IToken, IEquatable<ContinuationToken<T>>
{
    private readonly T value = value;
    private readonly string? @protected;

    /// <summary>Initializes a new instance of <see cref="ContinuationToken{T}"/>.</summary>
    /// <param name="value">The value.</param>
    /// <param name="protected">The original, opaque value.</param>
    internal ContinuationToken(T value, string @protected) : this(value)
    {
        this.@protected = @protected ?? throw new ArgumentNullException(nameof(@protected));
    }

    /// <summary>
    /// Gets the actual (unprotected/decrypted) value
    /// </summary>
    public T GetValue() => value;

    /// <inheritdoc/>
    public string? GetProtected() => @protected;

    /// <summary>
    /// Compares two instances of the <see cref="ContinuationToken{T}"/> for equality.
    /// </summary>
    /// <param name="left">The left value to compare.</param>
    /// <param name="right">The right value to compare.</param>
    /// <returns></returns>
    public static bool operator ==(ContinuationToken<T> left, ContinuationToken<T> right)
    {
        return EqualityComparer<ContinuationToken<T>>.Default.Equals(left, right);
    }

    /// <summary>
    /// Compares two instances of the <see cref="ContinuationToken{T}"/> for inequality.
    /// </summary>
    /// <param name="left">The left value to compare.</param>
    /// <param name="right">The right value to compare.</param>
    /// <returns></returns>
    public static bool operator !=(in ContinuationToken<T> left, in ContinuationToken<T> right) => !(left == right);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as ContinuationToken<T>);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(value, @protected);

    /// <inheritdoc/>
    public override string? ToString() => value?.ToString();

    /// <inheritdoc/>
    public bool Equals(ContinuationToken<T>? other)
    {
        return other is not null
            && EqualityComparer<T>.Default.Equals(value, other.value)
            && @protected == other.@protected;
    }
}
