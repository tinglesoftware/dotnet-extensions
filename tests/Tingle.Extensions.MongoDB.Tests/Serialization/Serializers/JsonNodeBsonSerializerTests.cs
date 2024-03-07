using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Text.Json.Nodes;
using Tingle.Extensions.MongoDB.Serialization;

namespace Tingle.Extensions.MongoDB.Tests.Serialization.Serializers;

public class JsonNodeBsonSerializerTests
{
    static JsonNodeBsonSerializerTests() => BsonSerializer.RegisterSerializationProvider(new JsonSerializationProvider());

    [Fact]
    public void TestEmpty()
    {
        var obj = JsonNode.Parse("{}");
        var json = obj.ToJson();
        var expected = "{ }";
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<JsonNode>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));
    }

    [Fact]
    public void TestSimple()
    {
        var obj = JsonNode.Parse("{'a':2,'b':null}".Replace("'", "\""));
        var json = obj.ToJson();
        var expected = "{ 'a' : 2, 'b' : null }".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<JsonNode>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));
    }

    [Fact]
    public void TestNested()
    {
        var obj = JsonNode.Parse("{'a':2,'b':null,'c':{'c1':'cake'}}".Replace("'", "\""));
        var json = obj.ToJson();
        var expected = "{ 'a' : 2, 'b' : null, 'c' : { 'c1' : 'cake' } }".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<JsonNode>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));
    }

    [Fact]
    public void TestArray()
    {
        var obj = new Animal { Details = JsonNode.Parse("['a','b','c']".Replace("'", "\"")), };
        var json = obj.ToJson();
        var expected = "{ 'Details' : ['a', 'b', 'c'] }".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<Animal>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));

    }

    class Animal
    {
        public JsonNode? Details { get; set; }
    }
}
