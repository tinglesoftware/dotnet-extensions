namespace Tingle.Extensions.Primitives.Tests;

public class ImageDimensionsTests
{
    [Fact]
    public void ComparisonWorks()
    {
        var d1 = new ImageDimensions(200, 200);
        var d2 = new ImageDimensions(300, 300);
        var d3 = (ImageDimensions)(System.Drawing.Size)d1;
        var d4 = (ImageDimensions)(System.Drawing.Size)d2;

        Assert.Equal(d1, d3);
        Assert.Equal(d2, d4);
        Assert.NotEqual(d1, d2);
    }

    [Fact]
    public void ConvertsFromSize()
    {
        var size = new System.Drawing.Size(120, 130);
        ImageDimensions dimensions = size;
        Assert.Equal(120U, dimensions.Width);
        Assert.Equal(130U, dimensions.Height);
    }

    [Fact]
    public void ConvertsToSize()
    {
        var dimensions = new ImageDimensions(120, 130);
        System.Drawing.Size size = dimensions;
        Assert.Equal(120, size.Width);
        Assert.Equal(130, size.Height);
    }

    [Fact]
    public void TestIConvertibleMethods()
    {
        object value = new ImageDimensions(50, 45);
        Assert.Equal(TypeCode.Object, ((IConvertible)value).GetTypeCode());
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(object), null)); // not AreSame because of boxing
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(ImageDimensions), null)); // not AreSame because of boxing
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
        Assert.Equal("50px by 45px", Convert.ToString(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt32(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt64(value));

        Assert.Equal("50px by 45px", ((IConvertible)value).ToType(typeof(string), null));
        Assert.Throws<InvalidCastException>(() => ((IConvertible)value).ToType(typeof(ulong), null));
    }
}
