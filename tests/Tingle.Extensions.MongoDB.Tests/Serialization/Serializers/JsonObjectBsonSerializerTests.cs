using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Text.Json.Nodes;
using Tingle.Extensions.MongoDB.Serialization;

namespace Tingle.Extensions.MongoDB.Tests.Serialization.Serializers;

public class JsonObjectBsonSerializerTests
{
    static JsonObjectBsonSerializerTests() => BsonSerializer.RegisterSerializationProvider(new JsonSerializationProvider());

    [Fact]
    public void TestEmpty()
    {
        var obj = new JsonObject();
        var json = obj.ToJson();
        var expected = "{ }";
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<JsonObject>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));
    }

    [Fact]
    public void TestSimple()
    {
        var obj = (JsonObject)JsonNode.Parse("{'a':2,'b':null}".Replace("'", "\""))!;
        var json = obj.ToJson();
        var expected = "{ 'a' : 2, 'b' : null }".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<JsonObject>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));
    }

    [Fact]
    public void TestNested()
    {
        var obj = (JsonObject)JsonNode.Parse("{'a':2,'b':null,'c':{'c1':'cake'}}".Replace("'", "\""))!;
        var json = obj.ToJson();
        var expected = "{ 'a' : 2, 'b' : null, 'c' : { 'c1' : 'cake' } }".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<JsonObject>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));
    }
}
