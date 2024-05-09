using System.ComponentModel;
using System.Text.Json;

namespace Tingle.Extensions.Primitives.Tests;

public class SwiftCodeTests
{
    [Theory]
    [InlineData("KCBLKENXXXX", "KCBL", "KE", "NX", "XXX", true)]
    [InlineData("IMBLKENAXXX", "IMBL", "KE", "NA", "XXX", true)]
    [InlineData("KCBLUGAXXXX", "KCBL", "UG", "AX", "XXX", true)]
    [InlineData("PMFAUS66HKG", "PMFA", "US", "66", "HKG", true)]
    [InlineData("KCBLKENX", "KCBL", "KE", "NX", null, true)]
    [InlineData("PMFAUS66", "PMFA", "US", "66", null, true)]
    [InlineData("IM2LKENAXXX", null, null, null, null, false)]
    [InlineData("KCBL2ENXXXX", null, null, null, null, false)]
    [InlineData("KCB2UGAX", null, null, null, null, false)]
    public void TryParse_Works(string code, string? institution, string? country, string? location, string? branch, bool expected)
    {
        Assert.Equal(expected, SwiftCode.TryParse(code, out var sw));
        if (expected) Assert.NotNull(sw);
        else Assert.Null(sw);

        // if it was expected to pass, validate the parsed fields
        if (expected)
        {
            Assert.Equal(institution, sw!.Institution);
            Assert.Equal(country, sw.Country);
            Assert.Equal(location, sw.Location);
            Assert.Equal(branch, sw.Branch);
        }
    }

    [Theory]
    [InlineData("KCBLKENXXXX", "KCBL", "KE", "NX", "XXX")]
    [InlineData("IMBLKENAXXX", "IMBL", "KE", "NA", "XXX")]
    [InlineData("KCBLUGAXXXX", "KCBL", "UG", "AX", "XXX")]
    [InlineData("PMFAUS66HKG", "PMFA", "US", "66", "HKG")]
    [InlineData("KCBLKENX", "KCBL", "KE", "NX", null)]
    [InlineData("PMFAUS66", "PMFA", "US", "66", null)]
    public void Parse_Works(string code, string institution, string country, string location, string? branch)
    {
        var sw = SwiftCode.Parse(code);
        Assert.Equal(institution, sw!.Institution);
        Assert.Equal(country, sw.Country);
        Assert.Equal(location, sw.Location);
        Assert.Equal(branch, sw.Branch);
    }

    [Theory]
    [InlineData("KCBLKENXXXX", false, false, false, true)]
    [InlineData("KCBLKEN0XXX", true, false, false, true)]
    [InlineData("KCBLKEN1XXX", false, true, false, true)]
    [InlineData("KCBLKEN2XXX", false, false, true, true)]
    [InlineData("PMFAUS66HKG", false, false, false, false)]
    [InlineData("PMFAUS62HKG", false, false, true, false)]
    [InlineData("PMFAUS62", false, false, true, true)]
    [InlineData("KCBLKENX", false, false, false, true)]
    public void SpecialChars_Works(string code, bool isTestCode, bool isPassive, bool isReverse, bool isPrimaryOffice)
    {
        var sw = SwiftCode.Parse(code);
        Assert.NotNull(sw);

        Assert.Equal(isTestCode, sw!.Test);
        Assert.Equal(isPassive, sw.Passive);
        Assert.Equal(isReverse, sw.Reverse);
        Assert.Equal(isPrimaryOffice, sw.Primary);
    }

    [Fact]
    public void TestIConvertibleMethods()
    {
        object value = SwiftCode.Parse("KCBLKENXXXX");
        Assert.Equal(TypeCode.Object, ((IConvertible)value).GetTypeCode());
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(object), null)); // not AreSame because of boxing
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(SwiftCode), null)); // not AreSame because of boxing
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
        Assert.Equal("KCBLKENXXXX", Convert.ToString(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt32(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt64(value));

        Assert.Equal("KCBLKENXXXX", ((IConvertible)value).ToType(typeof(string), null));
        Assert.Throws<InvalidCastException>(() => ((IConvertible)value).ToType(typeof(ulong), null));
    }

    [Fact]
    public void TypeConverter_ConvertsToString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(SwiftCode));
        Assert.NotNull(converter);
        var c = SwiftCode.Parse("KCBLKENXXXX");
        var actual = converter.ConvertToString(c);
        var expected = "KCBLKENXXXX";
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ConverterTestData))]
    public void TypeConverter_ConvertsFromString(string input, SwiftCode expected)
    {
        var converter = TypeDescriptor.GetConverter(typeof(SwiftCode));
        Assert.NotNull(converter);
        var actual = Assert.IsType<SwiftCode>(converter.ConvertFromString(input));
        Assert.Equal(expected, actual);
    }

    public static readonly IEnumerable<object[]> ConverterTestData =
    [
        ["KCBLKENXXXX", SwiftCode.Parse("KCBLKENXXXX")],
        ["IMBLKENAXXX", SwiftCode.Parse("IMBLKENAXXX")],
        ["PMFAUS66HKG", SwiftCode.Parse("PMFAUS66HKG")],
    ];

    [Fact]
    public void JsonConverter_Works()
    {
        var src_json = "{\"swiftCode\":\"KCBLKENXXXX\"}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(src_json, dst_json);
    }

    class TestModel
    {
        public SwiftCode? SwiftCode { get; set; }
    }
}
