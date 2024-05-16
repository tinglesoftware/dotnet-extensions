using System.ComponentModel;
using System.Text.Json;

namespace Tingle.Extensions.Primitives.Tests;

public class CountryTests
{
    [Fact]
    public void All_Works()
    {
        var all = Country.All;
        Assert.NotNull(all);
        Assert.True(all.Any());

        Assert.Contains(all, c => c.CurrencyCode == "KES");
        Assert.Contains(all, c => c.TwoLetterCode == "KE");
        Assert.Contains(all, c => c.ThreeLetterCode == "KEN");
    }

    [Theory]
    [InlineData("KE", true)]
    [InlineData("UG", true)]
    [InlineData("US", true)]
    [InlineData("KEN", true)]
    [InlineData("UGA", true)]
    [InlineData("USA", true)]
    [InlineData("404", true)]
    [InlineData("800", true)]
    [InlineData("840", true)]
    public void TryGetFromCode_Works(string code, bool expected)
    {
        Assert.Equal(expected, Country.TryGetFromCode(code, out _));
    }

    [Fact(Skip = "ShouldBeRunManually")] // for 249 countries, it takes 50seconds
    public async Task AllFlagsUrls_Exist()
    {
        var all = Country.All;
        using var httpClient = new HttpClient();
        foreach (var c in all)
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, c.FlagUrl);
            using var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Flag URL for {c.ThreeLetterCode}({c.Name}) failed. URL set = {c.FlagUrl}");
            }
        }
    }

    [Fact]
    public void AllContinentsAreKnown()
    {
        string[] contintentNames = [
            "Asia",
            "Europe",
            "Americas",
            "Africa",
            "North America", /*"NorthAmerica",*/
            "South America", /*"SouthAmerica",*/
            "Antarctica",
            "Oceania",
            "Australia",
            "Kenorland",
        ];

        var all = Country.All;
        var distinctContinentsAsStrings = all.Select(c => c.Continent).Distinct().ToList();
        foreach (var dcas in distinctContinentsAsStrings)
        {
            Assert.Contains(dcas, contintentNames);
        }
    }

    [Fact]
    public void TestIConvertibleMethods()
    {
        object value = Country.FromCode("KE");
        Assert.Equal(TypeCode.Object, ((IConvertible)value).GetTypeCode());
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(object), null)); // not AreSame because of boxing
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(Country), null)); // not AreSame because of boxing
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
        Assert.Equal("KEN", Convert.ToString(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt32(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt64(value));

        Assert.Equal("KEN", ((IConvertible)value).ToType(typeof(string), null));
        Assert.Throws<InvalidCastException>(() => ((IConvertible)value).ToType(typeof(ulong), null));
    }

    [Fact]
    public void TypeConverter_ConvertsToString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(Country));
        Assert.NotNull(converter);
        var c = Country.FromCode("KEN");
        var actual = converter.ConvertToString(c);
        var expected = "KEN";
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ConverterTestData))]
    public void TypeConverter_ConvertsFromString(string input, Country expected)
    {
        var converter = TypeDescriptor.GetConverter(typeof(Country));
        Assert.NotNull(converter);
        var actual = Assert.IsType<Country>(converter.ConvertFromString(input));
        Assert.Equal(expected, actual);
    }

    public static readonly IEnumerable<object[]> ConverterTestData =
    [
        ["KEN", Country.FromCode("KEN")],
        ["KE", Country.FromCode("KEN")],
    ];

    [Fact]
    public void JsonConverter_Works()
    {
        var src_json = "{\"country\":\"KEN\"}";
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
        var src_json = "{\"country\":\"KEN\"}";
        var model = JsonSerializer.Deserialize(src_json, TestJsonSerializerContext.Default.CountryTests_TestModel)!;
        var dst_json = JsonSerializer.Serialize(model, TestJsonSerializerContext.Default.CountryTests_TestModel);
        Assert.Equal(src_json, dst_json);
    }

    internal class TestModel
    {
        public Country? Country { get; set; }
    }
}
