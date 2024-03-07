using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Text.Json;
using Tingle.Extensions.MongoDB.Serialization;

namespace Tingle.Extensions.MongoDB.Tests.Serialization.Serializers;

public class JsonElementBsonSerializerTests
{
    static JsonElementBsonSerializerTests() => BsonSerializer.RegisterSerializationProvider(new JsonSerializationProvider());

    [Fact]
    public void TestEmpty()
    {
        var obj = JsonSerializer.Deserialize<JsonElement>("{}");
        var json = obj.ToJson();
        var expected = "{ }";
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<JsonElement>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));
    }

    [Fact]
    public void TestSimple()
    {
        var obj = JsonSerializer.Deserialize<JsonElement>("{'a':2,'b':null}".Replace("'", "\""));
        var json = obj.ToJson();
        var expected = "{ 'a' : 2, 'b' : null }".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<JsonElement>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));
    }

    [Fact]
    public void TestNested()
    {
        var obj = JsonSerializer.Deserialize<JsonElement>("{'a':2,'b':null,'c':{'c1':'cake'}}".Replace("'", "\""));
        var json = obj.ToJson();
        var expected = "{ 'a' : 2, 'b' : null, 'c' : { 'c1' : 'cake' } }".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<JsonElement>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));
    }

    [Fact]
    public void TestArray()
    {
        var obj = new Animal { Details = JsonSerializer.Deserialize<JsonElement>("['a','b','c']".Replace("'", "\"")), };
        var json = obj.ToJson();
        var expected = "{ 'Details' : ['a', 'b', 'c'] }".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<Animal>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));

    }

    class Animal
    {
        public JsonElement Details { get; set; }
    }
}
