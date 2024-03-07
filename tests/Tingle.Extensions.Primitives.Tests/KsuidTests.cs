using System.ComponentModel;
using System.Text.Json;
using Tingle.Extensions.Primitives.Converters;

namespace Tingle.Extensions.Primitives.Tests;

public class KsuidTests
{
    private static readonly DateTimeOffset origin = new(2014, 05, 13, 16, 53, 20, TimeSpan.Zero);

    [Theory]
    [InlineData("0o5Fs0EELR0fUjHjbCnEtdUwQe3", "2017-05-17T01:49:21+00:00", true)]
    [InlineData("05A95E21D7B6FE8CD7CFF211704D8E7B9421210B", "2017-05-17T01:49:21+00:00", true)]
    [InlineData("1234567890", "2017-05-17T01:49:21+00:00", false)]
    public void TryParse_Works(string s, string created, bool expected)
    {
        Assert.Equal(expected, Ksuid.TryParse(s, out var id));
        if (expected) Assert.NotEqual(Ksuid.Empty, id);
        else Assert.Equal(Ksuid.Empty, id);

        // if it was expected to pass, validate the parsed fields
        if (expected)
        {
            Assert.Equal(DateTimeOffset.Parse(created), id.Created);
        }
    }

    [Fact]
    public void Parse_Works()
    {
        var id = Ksuid.Parse("0o5Fs0EELR0fUjHjbCnEtdUwQe3");
        Assert.Equal("0o5Fs0EELR0fUjHjbCnEtdUwQe3", id.ToString());
        Assert.Equal("05A95E21D7B6FE8CD7CFF211704D8E7B9421210B", id.ToString("H"));
        Assert.Equal(DateTimeOffset.Parse("2017-05-17T01:49:21+00:00"), id.Created);
    }

    [Fact]
    public void ToString_Works()
    {
        var bytes = new byte[] { 5, 169, 94, 33, 215, 182, 254, 140, 215, 207, 242, 17, 112, 77, 142, 123, 148, 33, 33, 11 };
        var id = new Ksuid(bytes);
        Assert.Equal("0o5Fs0EELR0fUjHjbCnEtdUwQe3", id.ToString());
        Assert.Equal("05A95E21D7B6FE8CD7CFF211704D8E7B9421210B", id.ToString("H"));
        Assert.Equal(DateTimeOffset.Parse("2017-05-17T01:49:21+00:00"), id.Created);
        Assert.Equal<byte>(bytes, id.ToByteArray());
    }

    [Fact]
    public void TestGenerateNewId()
    {
        // compare against two timestamps in case seconds since epoch changes in middle of test
        var timestamp1 = (long)Math.Floor((DateTimeOffset.UtcNow - origin).TotalSeconds);
        var id = Ksuid.Generate();
        var timestamp2 = (long)Math.Floor((DateTimeOffset.UtcNow - origin).TotalSeconds);
        Assert.InRange(id.Timestamp, timestamp1, timestamp2 + 1);
        Assert.False(id.ToString().StartsWith("0000"));
    }

    [Fact]
    public void TestGenerateNewIdWithDateTime()
    {
        var timestamp = new DateTimeOffset(2021, 1, 2, 3, 4, 5, 0, TimeSpan.Zero);
        var id = Ksuid.Generate(timestamp);
        Assert.Equal(timestamp, id.Created);
        Assert.False(id.ToString().StartsWith("0000"));
    }

    [Fact]
    public void TestGenerateNewIdWithTimestamp()
    {
        var timestamp = Convert.ToUInt32(TimeSpan.FromSeconds(0x01020304).TotalSeconds);
        var id = Ksuid.Generate(timestamp);
        Assert.Equal(timestamp, id.Timestamp);
        Assert.False(id.ToString().StartsWith("0000"));
    }

    [Fact]
    public void TestCompareEqualGeneratedIds()
    {
        var id1 = Ksuid.Generate();
        var id2 = id1;
        Assert.False(id1 != id2);
        Assert.True(id1 == id2);

        id2 = Ksuid.Parse(id1);
        Assert.Equal(id1, id2);
    }

    [Fact]
    public void ReproduceString()
    {
        var id = Ksuid.Parse("2FaKo92Ftv0Xnus63qqt1qNGL11");
        Assert.Equal("2FaKo92Ftv0Xnus63qqt1qNGL11", id.ToString());
        Assert.Equal(new DateTime(2022, 10, 2, 15, 35, 26, DateTimeKind.Utc), id.Created);
    }

    //[Fact]
    //public void TestCompareSmallerGeneratedId()
    //{
    //    var id1 = Ksuid.Generate().ToString();
    //    var id2 = Ksuid.Generate().ToString();
    //    Assert.True(id1.CompareTo(id2) < 0);
    //    Assert.True(id1.CompareTo(id2) <= 0);
    //    Assert.True(id1 != id2);
    //    Assert.False(id1 == id2);
    //    Assert.False(id1.CompareTo(id2) > 0);
    //    Assert.False(id1.CompareTo(id2) >= 0);
    //}

    //[Fact]
    //public void TestCompareLargerGeneratedId()
    //{
    //    var id2 = Ksuid.Generate().ToString(); // generate before id1
    //    var id1 = Ksuid.Generate().ToString();
    //    Assert.False(id1.CompareTo(id2) < 0);
    //    Assert.False(id1.CompareTo(id2) <= 0);
    //    Assert.True(id1 != id2);
    //    Assert.False(id1 == id2);
    //    Assert.True(id1.CompareTo(id2) > 0);
    //    Assert.True(id1.CompareTo(id2) >= 0);
    //}

    [Fact]
    public void TestIConvertibleMethods()
    {
        object value = Ksuid.Empty;
        Assert.Equal(TypeCode.Object, ((IConvertible)value).GetTypeCode());
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(object), null)); // not AreSame because of boxing
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(Ksuid), null)); // not AreSame because of boxing
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
        Assert.Equal("000000000000000000000000000", Convert.ToString(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt32(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt64(value));

        Assert.Equal("000000000000000000000000000", ((IConvertible)value).ToType(typeof(string), null));
        Assert.Throws<InvalidCastException>(() => ((IConvertible)value).ToType(typeof(ulong), null));
    }

    [Fact]
    public void TypeConverter_ConvertsToString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(Ksuid));
        Assert.NotNull(converter);
        var kid = Ksuid.Parse("0o5Fs0EELR0fUjHjbCnEtdUwQe3");
        var actual = converter.ConvertToString(kid);
        Assert.Equal("0o5Fs0EELR0fUjHjbCnEtdUwQe3", actual);
    }

    [Theory]
    [MemberData(nameof(ConverterTestData))]
    public void TypeConverter_ConvertsFromString(string input, Ksuid expected)
    {
        var converter = TypeDescriptor.GetConverter(typeof(Ksuid));
        Assert.NotNull(converter);
        var actual = Assert.IsType<Ksuid>(converter.ConvertFromString(input));
        Assert.Equal(expected, actual);
    }

    public static readonly IEnumerable<object[]> ConverterTestData = new List<object[]>
    {
        new object []{ "0o5Fs0EELR0fUjHjbCnEtdUwQe3", Ksuid.Parse("0o5Fs0EELR0fUjHjbCnEtdUwQe3"), },
        new object []{ "05A95E21D7B6FE8CD7CFF211704D8E7B9421210B", Ksuid.Parse("0o5Fs0EELR0fUjHjbCnEtdUwQe3"), },
    };

    [Fact]
    public void JsonConverter_Works()
    {
        var src_json = "{\"id\":\"0o5Fs0EELR0fUjHjbCnEtdUwQe3\"}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(src_json, dst_json);
        Assert.Equal(Ksuid.Parse("0o5Fs0EELR0fUjHjbCnEtdUwQe3"), model!.Id);
    }

    [Theory]
    [InlineData("0o5Fs0EELR0fUjHjbCnEtdUwQe3", Ksuid.HexFormat, "05A95E21D7B6FE8CD7CFF211704D8E7B9421210B")]
    [InlineData("0o5Fs0EELR0fUjHjbCnEtdUwQe3", Ksuid.Base62Format, "0o5Fs0EELR0fUjHjbCnEtdUwQe3")]
    public void JsonConverter_RespectsFormat(string val, string format, string expected)
    {
        var src_json = $"{{\"id\":\"{Ksuid.Parse(val)}\"}}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        options.Converters.Add(new KsuidJsonConverter(format));
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var expected_json = $"{{\"id\":\"{expected}\"}}";
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(expected_json, dst_json);
    }

    class TestModel
    {
        public Ksuid? Id { get; set; }
    }
}
