using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tingle.Extensions.Primitives.Converters;

namespace Tingle.Extensions.Primitives.Tests.Converters;

public class JsonIPAddressConverterTests
{
    [Fact]
    public void Serialization_Works()
    {
        var json = JsonSerializer.Serialize(new TestClass { Value = IPAddress.Loopback, });
        Assert.Equal(@"{""Value"":""127.0.0.1""}", json);

        json = JsonSerializer.Serialize(new TestClass { Value = IPAddress.IPv6Loopback, });
        Assert.Equal(@"{""Value"":""::1""}", json);

        json = JsonSerializer.Serialize(new TestClass { Value = null, });
        Assert.Equal(@"{""Value"":null}", json);
    }

    [Fact]
    public void Deserialization_Works()
    {
        var actual = JsonSerializer.Deserialize<TestClass>(@"{""Value"":""127.0.0.1""}");
        Assert.NotNull(actual);
        Assert.Equal(IPAddress.Loopback, actual.Value);

        actual = JsonSerializer.Deserialize<TestClass>(@"{""Value"":""::1""}");
        Assert.NotNull(actual);
        Assert.Equal(IPAddress.IPv6Loopback, actual.Value);

        actual = JsonSerializer.Deserialize<TestClass>(@"{""Value"":null}");
        Assert.NotNull(actual);
        Assert.Null(actual.Value);
    }

    [Fact]
    public void IPAddressInvalidTypeDeserializationTest() => Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TestClass>(@"{""Value"":1}"));

    [Fact]
    public void IPAddressInvalidValueDeserializationTest() => Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TestClass>(@"{""Value"":""invalid_value""}"));

    private class TestClass
    {
        [JsonConverter(typeof(JsonIPAddressConverter))]
        public IPAddress? Value { get; set; }
    }
}
