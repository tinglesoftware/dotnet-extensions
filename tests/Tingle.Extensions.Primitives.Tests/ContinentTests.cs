﻿using System.ComponentModel;
using System.Text.Json;

namespace Tingle.Extensions.Primitives.Tests;

public class ContinentTests
{
    [Fact]
    public void TypeConverter_ConvertsToString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(Continent));
        Assert.NotNull(converter);
        var c = new Continent("Africa");
        var actual = converter.ConvertToString(c);
        var expected = "Africa";
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ConverterTestData))]
    public void TypeConverter_ConvertsFromString(string input, Continent expected)
    {
        var converter = TypeDescriptor.GetConverter(typeof(Continent));
        Assert.NotNull(converter);
        var actual = Assert.IsType<Continent>(converter.ConvertFromString(input));
        Assert.Equal(expected, actual);
    }

    public static readonly IEnumerable<object[]> ConverterTestData =
    [
        ["Africa", new Continent("Africa")],
        ["Europe", new Continent("Europe")],
        ["Australia", new Continent("Australia")],
        ["Americas", new Continent("Americas")],
    ];

    [Fact]
    public void JsonConverter_Works()
    {
        var src_json = "{\"continent\":\"Africa\"}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options)!;
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(src_json, dst_json);
        Assert.True(model.Continent!.IsKnown());
    }

    [Fact]
    public void JsonSerializerContext_Works()
    {
        var src_json = "{\"continent\":\"Africa\"}";
        var model = JsonSerializer.Deserialize(src_json, TestJsonSerializerContext.Default.ContinentTests_TestModel)!;
        var dst_json = JsonSerializer.Serialize(model, TestJsonSerializerContext.Default.ContinentTests_TestModel);
        Assert.Equal(src_json, dst_json);
    }

    internal class TestModel
    {
        public Continent? Continent { get; set; }
    }
}
