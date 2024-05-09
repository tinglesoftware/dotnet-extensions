using System.ComponentModel;
using System.Text.Json;

namespace Tingle.Extensions.Primitives.Tests;

public class ConnectionStringBuilderTests
{
    [Fact]
    public void CreateCSB_Works()
    {
        var cs = "HostName=contoso.com;Scheme=https;Key=abcd";
        var csb = new ConnectionStringBuilder(cs);

        // assert the parts
        Assert.Equal("contoso.com", csb.GetHostname());
        Assert.Equal("https", csb.GetScheme());
        Assert.Equal("abcd", csb.GetKey());

        // ensure the built and original are the same
        var built = csb.ToString();
        Assert.Equal(cs, built);

        csb.WithHostname("contoso.co.ke");
        csb.WithScheme("ftp");
        csb.WithKey("1234");

        // assert the changed parts
        Assert.Equal("contoso.co.ke", csb.GetHostname());
        Assert.Equal("ftp", csb.GetScheme());
        Assert.Equal("1234", csb.GetKey());

        csb.WithHttpsScheme();
        Assert.Equal("https", csb.GetScheme());
    }

    [Fact]
    public void Is_Case_Insensitive()
    {
        var cs = "HOStName=contoso.com;SCHEME=https;key=abcd";
        var csb = new ConnectionStringBuilder(cs);

        // assert the parts
        Assert.Equal("contoso.com", csb.GetHostname());
        Assert.Equal("https", csb.GetScheme());
        Assert.Equal("abcd", csb.GetKey());
        Assert.True(csb.IsHttpsScheme());
    }

    [Fact]
    public void CollectionInitializer_Works()
    {
        var csb = new ConnectionStringBuilder("")
        {
            ["Scheme"] = "https",
            ["HostName"] = "contoso.com",
            ["Key"] = "abcd",
        };

        // assert the parts
        Assert.Equal("https", csb.GetScheme());
        Assert.Equal("contoso.com", csb.GetHostname());
        Assert.Equal("abcd", csb.GetKey());
        Assert.True(csb.IsHttpsScheme());
    }

    [Fact]
    public void TypeConverter_ConvertsToString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(ConnectionStringBuilder));
        Assert.NotNull(converter);
        var expected = "HostName=contoso.com;Scheme=https;Key=abcd";
        var csb = new ConnectionStringBuilder(expected);
        var actual = converter.ConvertToString(csb);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ConverterTestData))]
    public void TypeConverter_ConvertsFromString(string input, ConnectionStringBuilder expected)
    {
        var converter = TypeDescriptor.GetConverter(typeof(ConnectionStringBuilder));
        Assert.NotNull(converter);
        var actual = Assert.IsType<ConnectionStringBuilder>(converter.ConvertFromString(input));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ConverterTestData))]
    public void JsonConverter_Serialization_Works(string raw, ConnectionStringBuilder value)
    {
        var src_json = $"{{\"value\":\"{raw}\"}}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(src_json, dst_json);
        Assert.Equal(value, model!.Value);
    }

    [Theory]
    [MemberData(nameof(ConverterTestDataReverse))]
    public void JsonConverter_Deserialization_Works(ConnectionStringBuilder value, string expected)
    {
        var src_json = $"{{\"value\":\"{value}\"}}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
        var expected_json = $"{{\"value\":\"{expected}\"}}";
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(expected_json, dst_json);
    }

    class TestModel
    {
        public ConnectionStringBuilder Value { get; set; }
    }


    public static readonly IEnumerable<object[]> ConverterTestData =
    [
        [
            "Hostname=contoso.com;Scheme=https;Key=abcd",
            new ConnectionStringBuilder("").WithHostname("contoso.com").WithHttpsScheme().WithKey("abcd")
        ],

        [
            "Hostname=contoso.com;Scheme=http;Key=123456",
            new ConnectionStringBuilder("").WithHostname("contoso.com").WithHttpScheme().WithKey("123456")
        ],
    ];

    public static readonly IEnumerable<object[]> ConverterTestDataReverse = ConverterTestData.Select(t => t.Reverse().ToArray()).ToList();
}
