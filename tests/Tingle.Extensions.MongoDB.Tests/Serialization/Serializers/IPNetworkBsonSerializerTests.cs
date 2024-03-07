#if NET8_0_OR_GREATER
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Net;
using Tingle.Extensions.MongoDB.Serialization;

namespace Tingle.Extensions.MongoDB.Tests.Serialization.Serializers;

public class IPNetworkBsonSerializerTests
{
    static IPNetworkBsonSerializerTests() => BsonSerializer.RegisterSerializationProvider(new TingleSerializationProvider());

    [Fact]
    public void Works_For_Null()
    {
        var obj = new TestClass
        {
            Network = null
        };
        var json = obj.ToJson();
        var expected = "{ 'Network' : null }".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<TestClass>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));
    }

    [Fact]
    public void Works_For_Parse()
    {
        var obj = new TestClass
        {
            Network = IPNetwork.Parse("192.168.0.4/32")
        };
        var json = obj.ToJson();
        var expected = "{ 'Network' : '192.168.0.4/32' }".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<TestClass>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));
    }

    [Fact]
    public void Works_For_Create()
    {
        var obj = new TestClass
        {
            Network = new IPNetwork(new IPAddress(new byte[] { 30, 0, 0, 0 }), 27)
        };
        var json = obj.ToJson();
        var expected = "{ 'Network' : '30.0.0.0/27' }".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<TestClass>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));
    }

    public class TestClass
    {
        public IPNetwork? Network { get; set; }
    }
}
#endif
