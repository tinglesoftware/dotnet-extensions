#if NET8_0_OR_GREATER
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tingle.Extensions.Primitives.Converters;

namespace Tingle.Extensions.Primitives.Tests.Converters;

public class JsonIPNetworkConverterTests
{
    [Fact]
    public void Serialization_Works()
    {
        var json = JsonSerializer.Serialize(new TestClass { Value = IPNetwork.Parse("192.168.0.4/32"), });
        Assert.Equal(@"{""Value"":""192.168.0.4/32""}", json);

        json = JsonSerializer.Serialize(new TestClass { Value = IPNetwork.Parse("30.0.0.0/27"), });
        Assert.Equal(@"{""Value"":""30.0.0.0/27""}", json);

        json = JsonSerializer.Serialize(new TestClass { Value = null, });
        Assert.Equal(@"{""Value"":null}", json);
    }

    [Fact]
    public void Deserialization_Works()
    {
        TestClass? actual = JsonSerializer.Deserialize<TestClass>(@"{""Value"":""192.168.0.4/32""}");
        Assert.NotNull(actual);
        Assert.Equal(IPNetwork.Parse("192.168.0.4/32"), actual.Value);

        actual = JsonSerializer.Deserialize<TestClass>(@"{""Value"":""30.0.0.0/27""}");
        Assert.NotNull(actual);
        Assert.Equal(IPNetwork.Parse("30.0.0.0/27"), actual.Value);

        actual = JsonSerializer.Deserialize<TestClass>(@"{""Value"":null}");
        Assert.NotNull(actual);
        Assert.Null(actual.Value);
    }

    [Fact]
    public void IPNetworkInvalidTypeDeserializationTest() => Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TestClass>(@"{""Value"":1}"));

    [Fact]
    public void IPNetworkInvalidValueDeserializationTest() => Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TestClass>(@"{""Value"":""invalid_value""}"));

    private class TestClass
    {
        [JsonConverter(typeof(JsonIPNetworkConverter))]
        public IPNetwork? Value { get; set; }
    }
}
#endif
