using System.ComponentModel;
using System.Text.Json;

namespace Tingle.Extensions.Primitives.Tests;

public class DurationTests
{
    private readonly DateTime dt = new(2022, 08, 31, 12, 19, 10);
    private readonly DateTimeOffset dto = new(2022, 08, 31, 12, 19, 10, TimeZoneInfo.FindSystemTimeZoneById("Africa/Nairobi").BaseUtcOffset);

    [Fact]
    public void CreatesFromTimeSpan()
    {
        var ts = TimeSpan.FromDays(1.556);
        var duration = new Duration(ts);
        Assert.Equal(0U, duration.Years);
        Assert.Equal(0U, duration.Months);
        Assert.Equal(0U, duration.Weeks);
        Assert.Equal(1U, duration.Days);
        Assert.Equal(13U, duration.Hours);
        Assert.Equal(20U, duration.Minutes);
        Assert.Equal(38U, duration.Seconds);
    }

    #region Add & Subtract on DateTime and DateTimeOffset

    [Fact]
    public void Add_Works_ForDateTime()
    {
        var duration = Duration.FromMonths(1);
        var expected = dt.AddMonths(1);
        var actual = dt + duration;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Add_Works_ForDateTimeOffset()
    {
        var duration = Duration.FromWeeks(1);
        var expected = dto.AddDays(7);
        var actual = dto + duration;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Subtract_Works_ForDateTime()
    {
        var duration = Duration.FromMonths(1);
        var expected = dt.AddMonths(-1);
        var actual = dt - duration;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Subtract_Works_ForDateTimeOffset()
    {
        var duration = Duration.FromWeeks(1);
        var expected = dto.AddDays(-7);
        var actual = dto - duration;
        Assert.Equal(expected, actual);
    }

    #endregion

    #region Equality

    [Theory]
    [InlineData("P1Y", "P1Y", true)]
    [InlineData("P2M", "P2M", true)]
    [InlineData("P3W", "P3W", true)]
    [InlineData("P4D", "P4D", true)]
    [InlineData("PT5H", "PT5H", true)]
    [InlineData("PT6M", "PT6M", true)]
    [InlineData("PT7S", "PT7S", true)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P1Y2M3W4DT5H6M7S", true)]

    [InlineData("P1Y", "P2Y", false)]
    [InlineData("P1Y", "P1M", false)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P1Y2M3W4DT5H6M9S", false)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P1Y2M3W4DT5H9M7S", false)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P1Y2M3W4DT9H6M7S", false)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P1Y2M3W9DT5H6M7S", false)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P1Y2M9W4DT5H6M7S", false)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P1Y9M3W4DT5H6M7S", false)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P9Y2M3W4DT5H6M7S", false)]
    public void Equal_Works_ForMethod(string input1, string input2, bool equal)
    {
        var duration1 = Duration.Parse(input1);
        var duration2 = Duration.Parse(input2);
        if (equal) Assert.Equal(duration1, duration2);
        else Assert.NotEqual(duration1, duration2);
    }

    [Theory]

    [InlineData("P1Y", "P1Y", true)]
    [InlineData("P2M", "P2M", true)]
    [InlineData("P3W", "P3W", true)]
    [InlineData("P4D", "P4D", true)]
    [InlineData("PT5H", "PT5H", true)]
    [InlineData("PT6M", "PT6M", true)]
    [InlineData("PT7S", "PT7S", true)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P1Y2M3W4DT5H6M7S", true)]

    [InlineData("P1Y", "P2Y", false)]
    [InlineData("P1Y", "P1M", false)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P1Y2M3W4DT5H6M9S", false)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P1Y2M3W4DT5H9M7S", false)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P1Y2M3W4DT9H6M7S", false)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P1Y2M3W9DT5H6M7S", false)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P1Y2M9W4DT5H6M7S", false)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P1Y9M3W4DT5H6M7S", false)]
    [InlineData("P1Y2M3W4DT5H6M7S", "P9Y2M3W4DT5H6M7S", false)]
    public void Equal_Works_ForOperators(string input1, string input2, bool equal)
    {
        var duration1 = Duration.Parse(input1);
        var duration2 = Duration.Parse(input2);
        Assert.Equal(equal, duration1 == duration2);
        Assert.Equal(!equal, duration1 != duration2);
    }

    #endregion

    #region Parsing

    [Theory]
    [InlineData("P1Y", 1, 0, 0, 0, 0, 0, 0)]
    [InlineData("P2M", 0, 2, 0, 0, 0, 0, 0)]
    [InlineData("P3W", 0, 0, 3, 0, 0, 0, 0)]
    [InlineData("P4D", 0, 0, 0, 4, 0, 0, 0)]
    [InlineData("PT5H", 0, 0, 0, 0, 5, 0, 0)]
    [InlineData("PT6M", 0, 0, 0, 0, 0, 6, 0)]
    [InlineData("PT7S", 0, 0, 0, 0, 0, 0, 7)]
    [InlineData("P1Y2M3W4DT5H6M7S", 1, 2, 3, 4, 5, 6, 7)]
    public void Parse_Works(string input, uint years, uint months, uint weeks, uint days, uint hours, uint minutes, uint seconds)
    {
        var duration = Duration.Parse(input);
        Assert.Equal(years, duration.Years);
        Assert.Equal(months, duration.Months);
        Assert.Equal(weeks, duration.Weeks);
        Assert.Equal(days, duration.Days);
        Assert.Equal(hours, duration.Hours);
        Assert.Equal(minutes, duration.Minutes);
        Assert.Equal(seconds, duration.Seconds);
    }

    [Theory]
    [InlineData("P")]
    [InlineData("P1H")] // Time in date part
    [InlineData("PT1D")] // Date in time part
    [InlineData("P1M2Y")] // Components in wrong order
    [InlineData("PT1M2H")] // Components in wrong order
    [InlineData("P1Z")] // Invalid component
    [InlineData("P1Y---2M")] // Invalid characters after component
    [InlineData("P1Y2M+++")] // Trailing invalid characters
    public void Parse_Throws_FormatException_BadValue(string input)
    {
        Assert.Throws<FormatException>(() => Duration.Parse(input));
    }

    [Fact]
    public void Parse_Throws_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Duration.Parse(null!));
    }

    #endregion

    [Fact]
    public void TestIConvertibleMethods()
    {
        object value = Duration.FromMonths(3);
        Assert.Equal(TypeCode.Object, ((IConvertible)value).GetTypeCode());
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(object), null)); // not AreSame because of boxing
        Assert.Equal(value, ((IConvertible)value).ToType(typeof(Duration), null)); // not AreSame because of boxing
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
        Assert.Equal("P3M", Convert.ToString(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt16(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt32(value));
        Assert.Throws<InvalidCastException>(() => Convert.ToUInt64(value));

        Assert.Equal("P3M", ((IConvertible)value).ToType(typeof(string), null));
        Assert.Throws<InvalidCastException>(() => ((IConvertible)value).ToType(typeof(ulong), null));
    }

    [Fact]
    public void TypeConverter_ConvertsToString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(Duration));
        Assert.NotNull(converter);
        var d = Duration.FromMonths(3);
        var actual = converter.ConvertToString(d);
        var expected = "P3M";
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ConverterTestData))]
    public void TypeConverter_ConvertsFromString(string input, Duration expected)
    {
        var converter = TypeDescriptor.GetConverter(typeof(Duration));
        Assert.NotNull(converter);
        var actual = Assert.IsType<Duration>(converter.ConvertFromString(input));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ConverterTestData))]
    public void JsonConverter_Serialization_Works(string raw, Duration duration)
    {
        var src_json = $"{{\"duration\":\"{raw}\"}}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(src_json, dst_json);
        Assert.Equal(duration, model!.Duration);
    }

    [Theory]
    [MemberData(nameof(ConverterTestDataReverse))]
    public void JsonConverter_Deserialization_Works(Duration duration, string expected)
    {
        var src_json = $"{{\"duration\":\"{duration}\"}}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var expected_json = $"{{\"duration\":\"{expected}\"}}";
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(expected_json, dst_json);
    }

    [Fact]
    public void JsonSerializerContext_Works()
    {
        var src_json = "{\"duration\":\"P3M\"}";
        var model = JsonSerializer.Deserialize(src_json, TestJsonSerializerContext.Default.DurationTests_TestModel)!;
        var dst_json = JsonSerializer.Serialize(model, TestJsonSerializerContext.Default.DurationTests_TestModel);
        Assert.Equal(src_json, dst_json);
    }

    internal class TestModel
    {
        public Duration Duration { get; set; }
    }


    public static readonly IEnumerable<object[]> ConverterTestData =
    [
        ["P3M", Duration.FromMonths(3)],
        ["P5D", Duration.FromDays(5)],
        ["P3M5D", new Duration(0, months: 3, 0, days: 5)],
        ["PT10M", Duration.FromMinutes(10)],
    ];

    public static readonly IEnumerable<object[]> ConverterTestDataReverse = ConverterTestData.Select(t => t.Reverse().ToArray()).ToList();
}
