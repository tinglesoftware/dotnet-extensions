using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using Tingle.Extensions.Primitives.Converters;

namespace Tingle.Extensions.Primitives.Tests;

public class MoneyTests
{
    #region Formatting

    [Fact, UseCulture("en-US")]
    public void ToString_Respects_CurrentCulture_US()
    {
        var yen = new Money(Currency.FromCode("JPY"), 765);
        var euro = new Money(Currency.FromCode("EUR"), 76543);
        var dollar = new Money(Currency.FromCode("USD"), 76543);
        var dinar = new Money(Currency.FromCode("BHD"), 765432);

        Assert.Equal("en-US", Thread.CurrentThread.CurrentCulture.Name);
        Assert.Equal("¥765", yen.ToString());
        Assert.Equal("€765.43", euro.ToString());
        Assert.Equal("$765.43", dollar.ToString());
        Assert.Equal("BD765.432", dinar.ToString());
    }

    [Fact, UseCulture("nl-NL")]
    public void ToString_Respects_CurrentCulture_NL()
    {
        var yen = new Money(Currency.FromCode("JPY"), 765);
        var euro = new Money(Currency.FromCode("EUR"), 76543);
        var dollar = new Money(Currency.FromCode("USD"), 76543);
        var dinar = new Money(Currency.FromCode("BHD"), 765432);

        Assert.Equal("nl-NL", Thread.CurrentThread.CurrentCulture.Name);
        Assert.Equal("¥ 765", yen.ToString());
        Assert.Equal("€ 765,43", euro.ToString());
        Assert.Equal("$ 765,43", dollar.ToString());
        Assert.Equal("BD 765,432", dinar.ToString());
    }

    [Fact, UseCulture("fr-FR")]
    public void ToString_Respects_CurrentCulture_FR()
    {
        var yen = new Money(Currency.FromCode("JPY"), 765);
        var euro = new Money(Currency.FromCode("EUR"), 76543);
        var dollar = new Money(Currency.FromCode("USD"), 76543);
        var dinar = new Money(Currency.FromCode("BHD"), 765432);

        Assert.Equal("fr-FR", Thread.CurrentThread.CurrentCulture.Name);
        Assert.Equal("765 ¥", yen.ToString());
        Assert.Equal("765,43 €", euro.ToString());
        Assert.Equal("765,43 $", dollar.ToString());
        Assert.Equal("765,432 BD", dinar.ToString());
    }

    [Fact, UseCulture("en-US")]
    public void ToString_Respects_SuppliedCulture()
    {
        var yen = new Money(Currency.FromCode("JPY"), 765);
        var euro = new Money(Currency.FromCode("EUR"), 76543);
        var dollar = new Money(Currency.FromCode("USD"), 76543);
        var dinar = new Money(Currency.FromCode("BHD"), 765432);

        var ci = new CultureInfo("nl-NL");

        Assert.Equal("en-US", Thread.CurrentThread.CurrentCulture.Name);
        Assert.Equal("¥ 765", yen.ToString(ci));
        Assert.Equal("€ 765,43", euro.ToString(ci));
        Assert.Equal("$ 765,43", dollar.ToString(ci));
        Assert.Equal("BD 765,432", dinar.ToString(ci));
    }

    [Fact, UseCulture("en-US")]
    public void ToString_Respects_SuppliedNumberFormat()
    {
        var yen = new Money(Currency.FromCode("JPY"), 765);
        var euro = new Money(Currency.FromCode("EUR"), 76543);
        var dollar = new Money(Currency.FromCode("USD"), 76543);
        var dinar = new Money(Currency.FromCode("BHD"), 765432);

        var nfi = new CultureInfo("nl-NL").NumberFormat;

        Assert.Equal("en-US", Thread.CurrentThread.CurrentCulture.Name);
        Assert.Equal("¥ 765", yen.ToString(nfi));
        Assert.Equal("€ 765,43", euro.ToString(nfi));
        Assert.Equal("$ 765,43", dollar.ToString(nfi));
        Assert.Equal("BD 765,432", dinar.ToString(nfi));
    }

    [Fact]
    public void ToString_GFormat_ReturnsTheSameAsTheDefaultFormat()
    {
        var yen = new Money(Currency.FromCode("JPY"), 765);

        // according to https://learn.microsoft.com/en-us/dotnet/api/system.iformattable.tostring?view=net-8.0#notes-to-implementers
        // the result of using "G" should return the same result as using <null>
        Assert.Equal(yen.ToString(null, null), yen.ToString("G", null));
    }

    [Fact]
    public void ToString_GFormat_Respects_SuppliedCulture()
    {
        var yen = new Money(Currency.FromCode("JPY"), 765);

        Assert.Equal(yen.ToString("G", CultureInfo.InvariantCulture), yen.ToString(null, CultureInfo.InvariantCulture));
        Assert.Equal(yen.ToString("G", CultureInfo.GetCultureInfo("nl-NL")), yen.ToString(null, CultureInfo.GetCultureInfo("nl-NL")));
        Assert.Equal(yen.ToString("G", CultureInfo.GetCultureInfo("fr-FR")), yen.ToString(null, CultureInfo.GetCultureInfo("fr-FR")));
    }

    #endregion

    #region Parsing

    [Fact, UseCulture("nl-BE")]
    public void Parse_Works_WhenInBelgiumDutchSpeaking()
    {
        var euro = Money.Parse("€ 765,43");
        Assert.Equal(new Money("EUR", 76543), euro);
    }

    [Fact, UseCulture("fr-BE")]
    public void Parse_Works_WhenInBelgiumFrenchSpeaking()
    {
        var euro = Money.Parse("765,43 €");
        Assert.Equal(new Money("EUR", 76543), euro);
    }

    [Fact, UseCulture("nl-NL")]
    public void Parse_WithoutCurrency_UsesCurrencyOfCurrentCulture()
    {
        var euro = Money.Parse("765,43");
        Assert.Equal(new Money("EUR", 76543), euro);
    }

    [Fact, UseCulture("ja-JP")]
    public void Parse_Works_For_YenYuanSymbolInJapan()
    {
        var yen = Money.Parse("¥ 765");
        Assert.Equal(new Money("JPY", 765), yen);
    }

    [Fact, UseCulture("en-US")]
    public void Parse_Works_DollarSymbolInUSA()
    {
        var dollar = Money.Parse("$765.43");
        Assert.Equal(new Money("USD", 76543), dollar);
    }

    [Fact, UseCulture("nl-NL")]
    public void Parsing_DollarSymbolInNetherlands_Fails()
    {
        // $ symbol is used for multiple currencies
        var ex = Assert.Throws<FormatException>(() => Money.Parse("$ 765,43"));
        Assert.Contains("multiple known currencies", ex.Message);
    }

    [Fact, UseCulture("en-US")]
    public void Parse_EuroSymbolInUSA_Returns_USDollar()
    {
        var euro = Money.Parse("€765.43");
        Assert.Equal(new Money("EUR", 76543), euro);
    }

    [Fact]
    public void Parse_ThrowsExeception_For_Null()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => Money.Parse(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public void Parse_ThrowsExeception_For_Empty()
    {
        Assert.Throws<ArgumentNullException>(() => Money.Parse(""));
    }

    [Fact, UseCulture("nl-NL")]
    public void Parse_ThrowsExeception_For_UnknownCurrency()
    {
        var ex = Assert.Throws<FormatException>(() => Money.Parse("XYZ 765,43"));
        Assert.Contains("unknown currency", ex.Message);
    }

    #endregion

    [Fact]
    public void TestIConvertibleMethods()
    {
        object value = new Money(Currency.FromCode("KES"), 1250);
        Assert.Equal(TypeCode.Object, ((IConvertible)value).GetTypeCode());
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(object), null)); // not AreSame because of boxing
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(Money), null)); // not AreSame because of boxing
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
        Assert.Equal("Ksh12.50", Convert.ToString(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt32(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt64(value));

        Assert.Equal("Ksh12.50", ((IConvertible)value).ToType(typeof(string), null));
        Assert.Throws<InvalidCastException>(() => ((IConvertible)value).ToType(typeof(ulong), null));
    }

    [Fact]
    public void TypeConverter_ConvertsToString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(Money));
        Assert.NotNull(converter);
        var m = new Money("KES", 1250);
        var actual = converter.ConvertToString(m);
        var expected = "Ksh12.50";
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("KES 12.50", "KES", 1250)]
    [InlineData("Ksh 12.50", "KES", 1250)]
    [InlineData("USD10", "USD", 10_00)]
    public void TypeConverter_ConvertsFromString(string input, string currency, long amount)
    {
        var converter = TypeDescriptor.GetConverter(typeof(Money));
        Assert.NotNull(converter);
        var actual = Assert.IsType<Money>(converter.ConvertFromString(input));
        var expected = new Money(currency, amount);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void JsonConverter_Works()
    {
        var src_json = "{\"money\":\"KES 12.50\"}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(src_json, dst_json);
    }

    [Theory]
    [InlineData("KES 12.50", "I", "KES 12.50")]
    [InlineData("KES 12.50", "C", "Ksh12.50")]
    [InlineData("KES 12.50", "G", "Ksh12.50")]
    [InlineData("KES 12.50", "F", "12.50 Kenyan shillings")]
    [InlineData("KES 12.50", "N", "12.50")]
    public void JsonConverter_RespectsFormat(string val, string format, string expected)
    {
        var src_json = $"{{\"money\":\"{val}\"}}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        options.Converters.Add(new MoneyJsonConverter(format));
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var expected_json = $"{{\"money\":\"{expected}\"}}";
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(expected_json, dst_json);
    }

    [Fact]
    public void JsonSerializerContext_Works()
    {
        var src_json = "{\"money\":\"KES 12.50\"}";
        var model = JsonSerializer.Deserialize(src_json, TestJsonSerializerContext.Default.MoneyTests_TestModel)!;
        var dst_json = JsonSerializer.Serialize(model, TestJsonSerializerContext.Default.MoneyTests_TestModel);
        Assert.Equal(src_json, dst_json);
    }

    internal class TestModel
    {
        public Money? Money { get; set; }
    }
}
