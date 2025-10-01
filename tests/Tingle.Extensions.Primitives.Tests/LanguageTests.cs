using System.ComponentModel;
using System.Text.Json;

namespace Tingle.Extensions.Primitives.Tests;

public class LanguageTests
{
    [Fact]
    public void All_Works()
    {
        var all = Language.All;
        Assert.NotNull(all);
        Assert.True(all.Any());

        Assert.Contains(all, l => l.Name == "English");
        Assert.Contains(all, l => l.TwoLetterCode == "en");
        Assert.Contains(all, l => l.ThreeLetterCode == "eng");
    }

    [Theory]
    [InlineData("en", true)]
    [InlineData("sw", true)]
    [InlineData("FR", true)]
    [InlineData("eng", true)]
    [InlineData("swa", true)]
    [InlineData("FRA", true)]
    public void TryGetFromCode_Works(string code, bool expected)
    {
        Assert.Equal(expected, Language.TryGetFromCode(code, out _));
    }

    [Fact]
    public void TestIConvertibleMethods()
    {
        object value = Language.FromCode("en");
        Assert.Equal(TypeCode.Object, ((IConvertible)value).GetTypeCode());
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(object), null)); // not AreSame because of boxing
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(Language), null)); // not AreSame because of boxing
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
        Assert.Equal("eng", Convert.ToString(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt32(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt64(value));

        Assert.Equal("eng", ((IConvertible)value).ToType(typeof(string), null));
        Assert.Throws<InvalidCastException>(() => ((IConvertible)value).ToType(typeof(ulong), null));
    }

    [Fact]
    public void TypeConverter_ConvertsToString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(Language));
        Assert.NotNull(converter);
        var c = Language.FromCode("eng");
        var actual = converter.ConvertToString(c);
        var expected = "eng";
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("eng")]
    [InlineData("swa")]
    public void TypeConverter_ConvertsFromString(string input)
    {
        var expected = Language.FromCode(input);
        var converter = TypeDescriptor.GetConverter(typeof(Language));
        Assert.NotNull(converter);
        var actual = Assert.IsType<Language>(converter.ConvertFromString(input));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JsonConverter_Works()
    {
        var src_json = "{\"language\":\"eng\"}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(src_json, dst_json);
    }

    [Fact]
    public void JsonSerializerContext_Works()
    {
        var src_json = "{\"language\":\"eng\"}";
        var model = JsonSerializer.Deserialize(src_json, TestJsonSerializerContext.Default.LanguageTests_TestModel)!;
        var dst_json = JsonSerializer.Serialize(model, TestJsonSerializerContext.Default.LanguageTests_TestModel);
        Assert.Equal(src_json, dst_json);
    }

    internal class TestModel
    {
        public Language? Language { get; set; }
    }
}
