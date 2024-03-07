using System.ComponentModel;
using System.Text.Json;

namespace Tingle.Extensions.Primitives.Tests;

public class SequenceNumberTests
{
    private static readonly DateTimeOffset origin = new(2015, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(0)]
    [InlineData(2_199_023_255_551)]
    public void TestDateTimeConstructorAtEdgeOfRange(ulong milliSecondsSinceOrigin)
    {
        var timestamp = origin.AddMilliseconds(milliSecondsSinceOrigin);
        var sequenceNumber = new SequenceNumber(timestamp, 0, 0);
        Assert.Equal(timestamp, sequenceNumber.Created);
    }

    [Theory]
    [InlineData(-1L)]
    [InlineData((ulong)2_199_023_255_552 + 1)]
    public void TestDateTimeConstructorArgumentOutOfRangeException(long secondsSinceOrigin)
    {
        var timestamp = origin.AddMilliseconds(secondsSinceOrigin);
        Assert.Throws<ArgumentOutOfRangeException>(() => new SequenceNumber(timestamp, 0, 0));
    }

    [Fact]
    public void TestGenerateNewId()
    {
        // compare against two timestamps in case seconds since epoch changes in middle of test
        var timestamp1 = (long)Math.Floor((DateTimeOffset.UtcNow - origin).TotalMilliseconds);
        var sequenceNumber = SequenceNumber.Generate();
        var timestamp2 = (long)Math.Floor((DateTimeOffset.UtcNow - origin).TotalMilliseconds);
        Assert.InRange(sequenceNumber.Timestamp, timestamp1, timestamp2 + 1);
        Assert.NotEqual(0, sequenceNumber.Value);
    }

    [Fact]
    public void TestGenerateNewIdWithDateTime()
    {
        var timestamp = new DateTimeOffset(2021, 1, 2, 3, 4, 5, 6, TimeSpan.Zero);
        var sequenceNumber = SequenceNumber.Generate(timestamp);
        Assert.Equal(timestamp, sequenceNumber.Created);
        Assert.NotEqual(0, sequenceNumber.Value);
    }

    [Fact]
    public void TestGenerateNewIdWithTimestamp()
    {
        var timestamp = Convert.ToInt64(TimeSpan.FromSeconds(0x01020304).TotalMilliseconds);
        var sequenceNumber = SequenceNumber.Generate(timestamp);
        Assert.Equal(timestamp, sequenceNumber.Timestamp);
        Assert.NotEqual(0, sequenceNumber.Value);
    }

    [Fact]
    public void TestIComparable()
    {
        var sequenceNumber1 = SequenceNumber.Generate();
        var sequenceNumber2 = SequenceNumber.Generate();
        Assert.Equal(0, sequenceNumber1.CompareTo(sequenceNumber1));
        Assert.Equal(-1, sequenceNumber1.CompareTo(sequenceNumber2));
        Assert.Equal(1, sequenceNumber2.CompareTo(sequenceNumber1));
        Assert.Equal(0, sequenceNumber2.CompareTo(sequenceNumber2));
    }

    [Fact]
    public void TestCompareEqualGeneratedIds()
    {
        var sequenceNumber1 = SequenceNumber.Generate();
        var sequenceNumber2 = sequenceNumber1;
        Assert.False(sequenceNumber1 < sequenceNumber2);
        Assert.True(sequenceNumber1 <= sequenceNumber2);
        Assert.False(sequenceNumber1 != sequenceNumber2);
        Assert.True(sequenceNumber1 == sequenceNumber2);
        Assert.False(sequenceNumber1 > sequenceNumber2);
        Assert.True(sequenceNumber1 >= sequenceNumber2);
    }

    [Fact]
    public void TestCompareSmallerGeneratedId()
    {
        var sequenceNumber1 = SequenceNumber.Generate();
        var sequenceNumber2 = SequenceNumber.Generate();
        Assert.True(sequenceNumber1 < sequenceNumber2);
        Assert.True(sequenceNumber1 <= sequenceNumber2);
        Assert.True(sequenceNumber1 != sequenceNumber2);
        Assert.False(sequenceNumber1 == sequenceNumber2);
        Assert.False(sequenceNumber1 > sequenceNumber2);
        Assert.False(sequenceNumber1 >= sequenceNumber2);
    }

    [Fact]
    public void TestCompareLargerGeneratedId()
    {
        var sequenceNumber2 = SequenceNumber.Generate(); // generate before sequenceNumber1
        var sequenceNumber1 = SequenceNumber.Generate();
        Assert.False(sequenceNumber1 < sequenceNumber2);
        Assert.False(sequenceNumber1 <= sequenceNumber2);
        Assert.True(sequenceNumber1 != sequenceNumber2);
        Assert.False(sequenceNumber1 == sequenceNumber2);
        Assert.True(sequenceNumber1 > sequenceNumber2);
        Assert.True(sequenceNumber1 >= sequenceNumber2);
    }

    [Fact]
    public void TestIConvertibleMethods()
    {
        object value = SequenceNumber.Empty;
        Assert.Equal(TypeCode.Object, ((IConvertible)value).GetTypeCode());
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(object), null)); // not AreSame because of boxing
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(SequenceNumber), null)); // not AreSame because of boxing
        Assert.Throws<InvalidCastException>(() => Convert.ToBoolean(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToByte(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToChar(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToDateTime(value));
        Assert.Equal(0, Convert.ToDecimal(value));
        Assert.Equal(0, Convert.ToDouble(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToInt32(value));
        Assert.Equal(0L, Convert.ToInt64(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToSByte(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToSingle(value));
        Assert.Equal("0", Convert.ToString(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt32(value));
        Assert.Equal(0UL, Convert.ToUInt64(value));

        Assert.Equal("0", ((IConvertible)value).ToType(typeof(string), null));
        Assert.Equal(0UL, ((IConvertible)value).ToType(typeof(ulong), null));
    }

    [Theory]
    [InlineData(0UL, "0")]
    [InlineData(123456789UL, "123456789")]
    public void TypeConverter_ConvertsToString(long val, string expected)
    {
        var converter = TypeDescriptor.GetConverter(typeof(SequenceNumber));
        Assert.NotNull(converter);
        var actual = converter.ConvertToString(new SequenceNumber(val));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ConverterTestData))]
    public void TypeConverter_ConvertsFromString(string input, SequenceNumber expected)
    {
        var converter = TypeDescriptor.GetConverter(typeof(SequenceNumber));
        Assert.NotNull(converter);
        var actual = Assert.IsType<SequenceNumber>(converter.ConvertFromString(input));
        Assert.Equal(expected, actual);
    }

    public static readonly IEnumerable<object[]> ConverterTestData = new List<object[]>
    {
        new object []{ "0", new SequenceNumber(0L), },
        new object []{ "123456789", new SequenceNumber(123456789L), },
    };

    [Theory]
    [InlineData("0", 0UL)]
    [InlineData("123456789", 123456789UL)]
    public void JsonConverter_Works(string raw, long val)
    {
        var src_json = $"{{\"position\":{raw}}}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(src_json, dst_json);
        Assert.Equal<long>(val, model!.Position);
    }

    class TestModel
    {
        public SequenceNumber Position { get; set; }
    }
}
