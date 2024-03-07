using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Tingle.Extensions.MongoDB.Serialization;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.MongoDB.Tests.Serialization.Serializers;

public class SequenceNumberBsonSerializerTests
{
    static SequenceNumberBsonSerializerTests() => BsonSerializer.RegisterSerializationProvider(new TingleSerializationProvider());

    [Theory]
    [InlineData(0UL)]
    [InlineData(123456789UL)]
    public void Deserialize_Works_For_String(long val)
    {
        var json = $"{{ '_id' : 'cake', 'Position' : '{val}' }}".Replace("'", "\"");
        var result = BsonSerializer.Deserialize<Bookshop>(json);
        Assert.Equal("cake", result.Id);
        Assert.Equal<long>(val, result.Position);
    }

    [Theory]
    [InlineData(0UL)]
    [InlineData(123456789UL)]
    public void Deserialize_Works_For_Int64(long val)
    {
        var json = $"{{ '_id' : 'cake', 'Position' : NumberLong({val}) }}".Replace("'", "\"");
        var result = BsonSerializer.Deserialize<Bookshop>(json);
        Assert.Equal("cake", result.Id);
        Assert.Equal<long>(val, result.Position);
    }

    [Fact]
    public void Default_BsonRepresentation_IsUsed()
    {
        var obj = new Bookshop
        {
            Id = "cake",
            Position = new SequenceNumber(123456789)
        };
        var json = obj.ToJson();
        var expected = $"{{ '_id' : 'cake', 'Position' : NumberLong(123456789) }}".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<Bookshop>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));
        Assert.Equal<long>(123456789, rehydrated.Position);
    }

    [Theory]
    [InlineData(123456789UL, typeof(BookshopString), "'123456789'")]
    [InlineData(123456789UL, typeof(BookshopInt64), "NumberLong(123456789)")]
    public void BsonRepresentation_Is_Respected(long val, Type t, string bsonRaw)
    {
        var obj = (IBookshop)Activator.CreateInstance(t)!;
        obj.Position = new SequenceNumber(val);
        var json = obj.ToJson(t);
        var expected = $"{{ 'Position' : {bsonRaw} }}".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson(t);
        var rehydrated = (IBookshop)BsonSerializer.Deserialize(bson, t);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson(t)));
        Assert.Equal<long>(val, rehydrated.Position);
    }

    class Bookshop
    {
        public string? Id { get; set; }
        public SequenceNumber Position { get; set; }
    }

    interface IBookshop { SequenceNumber Position { get; set; } }
    class BookshopString : IBookshop { [BsonRepresentation(BsonType.String)] public SequenceNumber Position { get; set; } }
    class BookshopInt64 : IBookshop { [BsonRepresentation(BsonType.Int64)] public SequenceNumber Position { get; set; } }
}
