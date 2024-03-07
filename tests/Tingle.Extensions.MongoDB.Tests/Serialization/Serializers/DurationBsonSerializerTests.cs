using MongoDB.Bson.Serialization;
using Tingle.Extensions.MongoDB.Serialization;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.MongoDB.Tests.Serialization.Serializers;

public class DurationBsonSerializerTests
{
    static DurationBsonSerializerTests() => BsonSerializer.RegisterSerializationProvider(new TingleSerializationProvider());

    [Theory]
    [InlineData("P0D")]
    [InlineData("P3M")]
    [InlineData("P1Y2M3W4DT5H6M7S")]
    public void Deserialize_Works_For_String(string val)
    {
        var json = $"{{ '_id' : 'cake', 'Duration' : '{val}' }}".Replace("'", "\"");
        var result = BsonSerializer.Deserialize<Bookshop>(json);
        Assert.Equal("cake", result.Id);
        Assert.Equal(Duration.Parse(val), result.Duration);
    }

    class Bookshop
    {
        public string? Id { get; set; }
        public Duration Duration { get; set; }
    }
}
