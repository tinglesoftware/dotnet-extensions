using System.Drawing;

namespace Tingle.Extensions.Primitives;

/// <summary>
/// Stores an ordered pair of integers, which specify a <see cref="Width"/> and <see cref="Height"/>.
/// </summary>
/// <param name="width">The horizontal component of <see cref="ImageDimensions"/>, typically measured in pixels.</param>
/// <param name="height">The vertical component of <see cref="ImageDimensions"/>, typically measured in pixels.</param>
public readonly struct ImageDimensions(uint width, uint height) : IEquatable<ImageDimensions>, IConvertible
{
    /// <summary>The default value <c>0px by 0px</c>.</summary>
    public static readonly ImageDimensions Empty = new();

    /// <summary>
    /// Initializes a new instance of <see cref="ImageDimensions"/> with the same dimensions.
    /// </summary>
    /// <param name="value">The horizontal and vertical component of <see cref="ImageDimensions"/>, typically measured in pixels.</param>
    public ImageDimensions(uint value) : this(value, value) { }

    /// <summary>
    /// Initializes a new instance of <see cref="ImageDimensions"/> with the same dimensions.
    /// </summary>
    /// <param name="value">The horizontal and vertical component of <see cref="ImageDimensions"/>, typically measured in pixels.</param>
    public ImageDimensions(int value) : this(value, value) { }

    /// <summary>
    /// Initializes a new instance of <see cref="ImageDimensions"/> with the given dimensions.
    /// </summary>
    /// <param name="width">The horizontal component of <see cref="ImageDimensions"/>, typically measured in pixels.</param>
    /// <param name="height">The vertical component of <see cref="ImageDimensions"/>, typically measured in pixels.</param>
    public ImageDimensions(int width, int height) : this((uint)width, (uint)height) { }

    /// <summary>
    /// Gets or sets the horizontal component of <see cref="ImageDimensions"/>, typically measured in pixels.
    /// </summary>
    public uint Width { get; init; } = width;

    /// <summary>
    /// Gets or sets the vertical component of <see cref="ImageDimensions"/>, typically measured in pixels.
    /// </summary>
    public uint Height { get; init; } = height;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ImageDimensions dimensions && Equals(dimensions);

    /// <inheritdoc/>
    public readonly bool Equals(ImageDimensions other) => Width == other.Width && Height == other.Height;

    /// <inheritdoc/>
    public override readonly int GetHashCode() => HashCode.Combine(Width, Height);

    /// <inheritdoc/>
    public override readonly string ToString() => $"{Width:n0}px by {Height:n0}px";

    /// <inheritdoc/>
    public static bool operator ==(ImageDimensions left, ImageDimensions right) => left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(ImageDimensions left, ImageDimensions right) => !(left == right);

    /// <summary>Converts an <see cref="ImageDimensions"/> to a <see cref="Size"/>.</summary>
    /// <param name="value"></param>
    public static implicit operator Size(ImageDimensions value) => new(width: (int)value.Width, height: (int)value.Height);

    /// <summary>Converts a <see cref="Size"/> to an <see cref="ImageDimensions"/>.</summary>
    /// <param name="value"></param>
    public static implicit operator ImageDimensions(Size value) => new(width: value.Width, height: value.Height);

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
            TypeCode.Object when conversionType == typeof(ImageDimensions) => this,
            TypeCode.String => ((IConvertible)this).ToString(provider),
            _ => throw new InvalidCastException(),
        };
    }

    readonly ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new InvalidCastException();
    readonly uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new InvalidCastException();
    readonly ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new InvalidCastException();

    #endregion
}
