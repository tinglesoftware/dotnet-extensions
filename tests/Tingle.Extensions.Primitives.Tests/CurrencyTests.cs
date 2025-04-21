using System.ComponentModel;
using System.Globalization;
using System.Text.Json;

namespace Tingle.Extensions.Primitives.Tests;

public class CurrencyTests
{
    [Fact]
    public void All_Works()
    {
        var all = Currency.All;
        Assert.NotNull(all);
        Assert.True(all.Any());

        Assert.Contains(all, c => c.Code == "KES");
        Assert.Contains(all, c => c.DecimalDigits == 2);
        Assert.Contains(all, c => c.Name == "Kenyan Shilling");
    }

    [Theory]
    [InlineData("KES", true)]
    [InlineData("840", false)]
    public void TryGetFromCode_Works(string code, bool expected)
    {
        Assert.Equal(expected, Currency.TryGetFromCode(code, out _));
    }

    [Fact]
    public void FromCulture_Works()
    {
        var currency = Currency.FromCulture(CultureInfo.CreateSpecificCulture("nl-NL"));

        Assert.NotNull(currency);
        Assert.Equal("€", currency.Symbol);
        Assert.Equal("EUR", currency.Code);
        Assert.Equal("Euro", currency.Name);
    }

    [Fact]
    public void FromCultureWithNullCultureInfo_Throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Currency.FromCulture(null!));
    }

    [Fact]
    public void FromCultureWithNeutralCultureInfo_Throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentException>(() => Currency.FromCulture(new CultureInfo("en")));
    }

    [Fact, UseCulture("nl-NL")]
    public void CurrentCurrency_IsCorrect()
    {
        var currency = Currency.CurrentCurrency;
        Assert.Equal(Currency.FromCulture(CultureInfo.CurrentCulture), currency);
    }

    [Fact]
    public void TestIConvertibleMethods()
    {
        object value = Currency.FromCode("KES");
        Assert.Equal(TypeCode.Object, ((IConvertible)value).GetTypeCode());
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(object), null)); // not AreSame because of boxing
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(Currency), null)); // not AreSame because of boxing
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
        Assert.Equal("KES", Convert.ToString(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt32(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt64(value));

        Assert.Equal("KES", ((IConvertible)value).ToType(typeof(string), null));
        Assert.Throws<InvalidCastException>(() => ((IConvertible)value).ToType(typeof(ulong), null));
    }

    [Fact]
    public void TypeConverter_ConvertsToString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(Currency));
        Assert.NotNull(converter);
        var c = Currency.FromCode("KES");
        var actual = converter.ConvertToString(c);
        var expected = "KES";
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ConverterTestData))]
    public void TypeConverter_ConvertsFromString(string input, Currency expected)
    {
        var converter = TypeDescriptor.GetConverter(typeof(Currency));
        Assert.NotNull(converter);
        var actual = Assert.IsType<Currency>(converter.ConvertFromString(input));
        Assert.Equal(expected, actual);
    }

    public static readonly IEnumerable<object[]> ConverterTestData =
    [
        ["KES", Currency.FromCode("KES")],
        ["USD", Currency.FromCode("USD")],
    ];

    [Fact]
    public void JsonConverter_Works()
    {
        var src_json = "{\"currency\":\"KES\"}";
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
        var src_json = "{\"currency\":\"KES\"}";
        var model = JsonSerializer.Deserialize(src_json, TestJsonSerializerContext.Default.CurrencyTests_TestModel)!;
        var dst_json = JsonSerializer.Serialize(model, TestJsonSerializerContext.Default.CurrencyTests_TestModel);
        Assert.Equal(src_json, dst_json);
    }

    internal class TestModel
    {
        public Currency? Currency { get; set; }
    }
}
