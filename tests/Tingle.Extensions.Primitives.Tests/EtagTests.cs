using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using Tingle.Extensions.Primitives.Converters;

namespace Tingle.Extensions.Primitives.Tests;

public class EtagTests
{
    [Theory]
    [InlineData("AAAAAAAAAAA=", "AQAAAAAAAAA=")]
    [InlineData("CgAAAAAAAAA=", "CwAAAAAAAAA=")]
    public void Next_Works(string current, string expected)
    {
        var etag = Etag.Parse(current);
        var actual = etag.Next().ToString();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(new ulong[] { 1, 1, }, 2)]
    [InlineData(new ulong[] { 3, 62, }, 65)]
    [InlineData(new ulong[] { 1, 1, 3, 62, }, 67)]
    [InlineData(new ulong[] { 12, 8, 1, 21, }, 42)]
    public void Combine_Works(ulong[] values, ulong expected)
    {
        var tags = values.Select(u => (Etag)u).ToArray();
        var actual = Etag.Combine(tags);
        Assert.Equal((Etag)expected, actual);
    }

    [Theory]
    [InlineData(new string[] { "AQAAAAAAAAA=", "AQAAAAAAAAA=", }, "AgAAAAAAAAA=")]
    [InlineData(new string[] { "AwAAAAAAAAA=", "PgAAAAAAAAA=", }, "QQAAAAAAAAA=")]
    [InlineData(new string[] { "AQAAAAAAAAA=", "AQAAAAAAAAA=", "AwAAAAAAAAA=", "PgAAAAAAAAA=", }, "QwAAAAAAAAA=")]
    [InlineData(new string[] { "DAAAAAAAAAA=", "CAAAAAAAAAA=", "AQAAAAAAAAA=", "FQAAAAAAAAA=", }, "KgAAAAAAAAA=")]
    public void Combine_Works_ForStrings(string[] values, string expected)
    {
        var tags = values.Select(v => Etag.Parse(v)).ToArray();
        var actual = Etag.Combine(tags);
        Assert.Equal(Etag.Parse(expected), actual);
    }

    [Theory]
    [InlineData(0, "AAAAAAAAAAA=")]
    [InlineData(1, "AQAAAAAAAAA=")]
    [InlineData(2, "AgAAAAAAAAA=")]
    public void ToString_Works(ulong value, string expected)
    {
        var etag = new Etag(value);
        var actual = etag.ToString();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, "AAAAAAAAAAA=")]
    [InlineData(1, "AQAAAAAAAAA=")]
    [InlineData(2, "AgAAAAAAAAA=")]
    public void ToString_B_Works(ulong value, string expected)
    {
        var etag = new Etag(value);
        var actual = etag.ToString("B");
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1, "0x1")]
    [InlineData(2, "0x2")]
    [InlineData(123456789, "0x75BCD15")]
    public void ToString_D_Works(ulong value, string expected)
    {
        var etag = new Etag(value);
        var actual = etag.ToString("D");
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1, "\"0x1\"")]
    [InlineData(2, "\"0x2\"")]
    [InlineData(123456789, "\"0x75BCD15\"")]
    public void ToString_H_Works(ulong value, string expected)
    {
        var etag = new Etag(value);
        var actual = etag.ToString("H");
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("\"0x75BCD15\"", "Fc1bBwAAAAA=")]
    [InlineData("0x2", "AgAAAAAAAAA=")]
    [InlineData("\"0X75BCD15\"", "Fc1bBwAAAAA=")]
    [InlineData("0X2", "AgAAAAAAAAA=")]
    [InlineData("AQAAAAAAAAA=", "AQAAAAAAAAA=")]
    public void CreateFromString_Works(string value, string expected)
    {
        var etag = Etag.Parse(value);
        var actual = etag.ToString();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestIConvertibleMethods()
    {
        object value = Etag.Empty;
        Assert.Equal(TypeCode.Object, ((IConvertible)value).GetTypeCode());
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(object), null)); // not AreSame because of boxing
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(Etag), null)); // not AreSame because of boxing
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
        Assert.Equal("AAAAAAAAAAA=", Convert.ToString(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt32(value));
        Assert.Equal(0UL, Convert.ToUInt64(value));

        Assert.Equal("AAAAAAAAAAA=", ((IConvertible)value).ToType(typeof(string), null));
        Assert.Equal(0UL, ((IConvertible)value).ToType(typeof(ulong), null));
    }

    [Theory]
    [InlineData(0UL, "AAAAAAAAAAA=")]
    [InlineData(123456789UL, "Fc1bBwAAAAA=")]
    public void TypeConverter_ConvertsToString(ulong val, string expected)
    {
        var converter = TypeDescriptor.GetConverter(typeof(Etag));
        Assert.NotNull(converter);
        var actual = converter.ConvertToString(new Etag(val));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ConverterTestData))]
    public void TypeConverter_ConvertsFromString(string input, Etag expected)
    {
        var converter = TypeDescriptor.GetConverter(typeof(Etag));
        Assert.NotNull(converter);
        var actual = Assert.IsType<Etag>(converter.ConvertFromString(input));
        Assert.Equal(expected, actual);
    }

    public static readonly TheoryData<string, Etag> ConverterTestData = new()
    {
        { "AAAAAAAAAAA=", new Etag(0UL) },
        { "Fc1bBwAAAAA=", new Etag(123456789UL) },
        { "0x0", new Etag(0UL) },
        { "0x75BCD15", new Etag(123456789UL) },
    };

    [Theory]
    [InlineData("AAAAAAAAAAA=", 0UL)]
    [InlineData("Fc1bBwAAAAA=", 123456789UL)]
    public void JsonConverter_Works(string raw, ulong val)
    {
        var src_json = $"{{\"etag\":\"{raw}\"}}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(src_json, dst_json);
        Assert.Equal<ulong>(val, model!.Etag);
    }

    [Theory]
    [InlineData("0x75BCD15", "Fc1bBwAAAAA=")]
    [InlineData("0x75bcd15", "Fc1bBwAAAAA=")]
    public void JsonConverter_Works_Hex(string raw, string expected)
    {
        var src_json = $"{{\"etag\":\"{raw}\"}}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var expected_json = $"{{\"etag\":\"{expected}\"}}";
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(expected_json, dst_json);
    }

    [Theory]
    [InlineData(123456789UL, Etag.DefaultFormat, "0x75BCD15")]
    [InlineData(123456789UL, Etag.Base64Format, "Fc1bBwAAAAA=")]
    [InlineData(123456789UL, Etag.HeaderFormat, "\\\"0x75BCD15\\\"")]
    public void JsonConverter_RespectsFormat(ulong val, string format, string expected)
    {
        var src_json = $"{{\"etag\":\"{new Etag(val)}\"}}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        options.Converters.Add(new EtagJsonConverter(format));
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var expected_json = $"{{\"etag\":\"{expected}\"}}";
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(expected_json, dst_json);
    }

    [Fact]
    public void JsonSerializerContext_Works()
    {
        var src_json = "{\"etag\":\"Fc1bBwAAAAA=\"}";
        var model = JsonSerializer.Deserialize(src_json, TestJsonSerializerContext.Default.EtagTests_TestModel)!;
        var dst_json = JsonSerializer.Serialize(model, TestJsonSerializerContext.Default.EtagTests_TestModel);
        Assert.Equal(src_json, dst_json);
    }

    internal class TestModel
    {
        public Etag Etag { get; set; }
    }
}
