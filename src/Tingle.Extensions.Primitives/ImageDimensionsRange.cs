namespace Tingle.Extensions.Primitives;

/// <summary>
/// Stores an ordered pair of dimensions, which specify a <see cref="Min"/> and <see cref="Max"/>.
/// </summary>
public readonly struct ImageDimensionsRange : IEquatable<ImageDimensionsRange>, IConvertible
{
    /// <summary>
    /// Initializes a new instance of <see cref="ImageDimensionsRange"/>.
    /// </summary>
    /// <param name="min">The minimum width and height.</param>
    /// <param name="max">The maximum width and height.</param>
    public ImageDimensionsRange(uint min, uint max) : this(new ImageDimensions(min), new ImageDimensions(max)) { }

    /// <summary>
    /// Initializes a new instance of <see cref="ImageDimensionsRange"/>.
    /// </summary>
    /// <param name="min">The minimum width and height.</param>
    /// <param name="max">The maximum width and height.</param>
    public ImageDimensionsRange(int min, int max) : this(new ImageDimensions(min), new ImageDimensions(max)) { }

    /// <summary>
    /// Initializes a new instance of <see cref="ImageDimensionsRange"/>.
    /// </summary>
    /// <param name="min">The minimum dimensions.</param>
    /// <param name="max">The maximum dimensions.</param>
    public ImageDimensionsRange(ImageDimensions min, ImageDimensions max)
    {
        Min = min;
        Max = max;

        // if both are default, there is no need to use this type
        if (min == default && max == default)
        {
            throw new InvalidOperationException($"Either '{nameof(min)}' or '{nameof(max)}' or both must have non zero values.");
        }

        // if both are not default, we have to ensure min is less than or equal max
        if (min != default && max != default && GreaterThanOrEqualTo(min, max))
        {
            throw new InvalidOperationException($"'{nameof(min)}' must be less than '{nameof(max)}'");
        }
    }

    /// <summary>
    /// Gets or sets the minimum dimensions.
    /// </summary>
    public ImageDimensions Min { get; init; }

    /// <summary>
    /// Gets or sets the maximum dimensions.
    /// </summary>
    public ImageDimensions Max { get; init; }

    /// <summary>
    /// Whether a given <see cref="ImageDimensions"/> fits within the dimensions.
    /// </summary>
    /// <param name="dimensions">The <see cref="ImageDimensions"/> to check.</param>
    /// <returns></returns>
    public readonly bool IsWithin(ImageDimensions dimensions)
    {
        // only do comparison if the bounds are not default
        if (Min != default && !LessThanOrEqualTo(Min, dimensions)) return false;
        if (Max != default && !LessThanOrEqualTo(dimensions, Max)) return false;
        return true;
    }

    // these comparisons are here because adding them to ImageDimensions complicates things. No wonder System.Drawing.Size does not do have them
    internal static bool LessThanOrEqualTo(ImageDimensions s1, ImageDimensions s2) => s1.Width <= s2.Width && s1.Height <= s2.Height;
    internal static bool GreaterThanOrEqualTo(ImageDimensions s1, ImageDimensions s2) => s1.Width >= s2.Width && s1.Height >= s2.Height;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ImageDimensionsRange range && Equals(range);

    /// <inheritdoc/>
    public readonly bool Equals(ImageDimensionsRange other) => Min.Equals(other.Min) && Max.Equals(other.Max);

    /// <inheritdoc/>
    public override readonly int GetHashCode() => HashCode.Combine(Min, Max);

    /// <inheritdoc/>
    public override readonly string ToString() => Min == default ? $"up to {Max}" : Max == default ? $"at least {Min}" : $"within ({Min}) and ({Max})";

    /// <inheritdoc/>
    public static bool operator ==(ImageDimensionsRange left, ImageDimensionsRange right) => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(ImageDimensionsRange left, ImageDimensionsRange right) => !(left == right);

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
    string IConvertible.ToString(IFormatProvider? provider) => ToString();

    readonly object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
    {
        return Type.GetTypeCode(conversionType) switch
        {
            TypeCode.Object when conversionType == typeof(object) => this,
            TypeCode.Object when conversionType == typeof(ImageDimensionsRange) => this,
            TypeCode.String => ((IConvertible)this).ToString(provider),
            _ => throw new InvalidCastException(),
        };
    }

    readonly ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new InvalidCastException();
    readonly uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new InvalidCastException();
    readonly ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new InvalidCastException();

    #endregion
}
