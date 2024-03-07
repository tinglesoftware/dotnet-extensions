namespace Tingle.Extensions.Primitives.Tests;

public class ImageDimensionsRangeTests
{
    [Fact]
    public void Constructor_Throws_InvalidOperationException_NotDefault()
    {
        var min = new ImageDimensions(0);
        var max = new ImageDimensions(0);
        var ex = Assert.Throws<InvalidOperationException>(() => new ImageDimensionsRange(min, max));
        Assert.Equal("Either 'min' or 'max' or both must have non zero values.", ex.Message);
    }

    [Theory]
    [InlineData(100, 100, 50, 50)]      // less
    [InlineData(100, 100, 100, 100)]    // equal
    [InlineData(100, 50, 100, 50)]      // equal
    [InlineData(100, 150, 100, 50)]     // mixed
    public void Constructor_Throws_InvalidOperationException_MustBeLess(uint minWidth, uint minHeight, uint maxWidth, uint maxHeight)
    {
        var min = new ImageDimensions(minWidth, minHeight);
        var max = new ImageDimensions(maxWidth, maxHeight);
        var ex = Assert.Throws<InvalidOperationException>(() => new ImageDimensionsRange(min, max));
        Assert.Equal("'min' must be less than 'max'", ex.Message);
    }

    [Theory]
    [InlineData(45, 45, 100, 100, 40, 50, false)]   // false: lower limit (1)
    [InlineData(45, 45, 100, 100, 50, 40, false)]   // false: lower limit (2)
    [InlineData(45, 45, 100, 100, 110, 50, false)]  // false: upper limit (1)
    [InlineData(45, 45, 100, 100, 50, 110, false)]  // false: upper limit (2)
    [InlineData(45, 45, 100, 100, 50, 50, true)]    // true: within
    [InlineData(45, 45, 100, 100, 45, 45, true)]    // true: lower limit
    [InlineData(45, 45, 100, 100, 100, 100, true)]  // true: upper limit
    [InlineData(45, 45, 100, 100, 45, 100, true)]   // true: mixed (1)
    [InlineData(45, 45, 100, 100, 50, 80, true)]    // true: mixed (2)
    [InlineData(0, 0, 100, 100, 45, 100, true)]     // true: defaults (1)
    [InlineData(45, 45, 0, 0, 50, 80, true)]        // true: defaults (2)
    public void IsWithin_Works(uint minWidth, uint minHeight, uint maxWidth, uint maxHeight, uint width, uint height, bool expected)
    {
        var min = new ImageDimensions(minWidth, minHeight);
        var max = new ImageDimensions(maxWidth, maxHeight);
        var range = new ImageDimensionsRange(min, max);
        var dimensions = new ImageDimensions(width, height);
        Assert.Equal(expected, range.IsWithin(dimensions));
    }

    [Theory]
    [InlineData(0, 400, "up to 400px by 400px")]
    [InlineData(400, 0, "at least 400px by 400px")]
    [InlineData(400, 4_000, "within (400px by 400px) and (4,000px by 4,000px)")]
    public void ToString_Works(uint min, uint max, string expected)
    {
        var range = new ImageDimensionsRange(min, max);
        Assert.Equal(expected, range.ToString());
    }

    [Fact]
    public void TestIConvertibleMethods()
    {
        object value = new ImageDimensionsRange(45, 50);
        Assert.Equal(TypeCode.Object, ((IConvertible)value).GetTypeCode());
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(object), null)); // not AreSame because of boxing
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(ImageDimensionsRange), null)); // not AreSame because of boxing
        Assert.Throws<InvalidCastException>(() => Convert.ToBoolean(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToByte(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToChar(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToDateTime(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToDecimal(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToDouble(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToInt32(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToInt64(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToSByte(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToSingle(value));
        Assert.Equal("within (45px by 45px) and (50px by 50px)", Convert.ToString(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt32(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt64(value));

        Assert.Equal("within (45px by 45px) and (50px by 50px)", ((IConvertible)value).ToType(typeof(string), null));
        Assert.Throws<InvalidCastException>(() => ((IConvertible)value).ToType(typeof(ulong), null));
    }
}
