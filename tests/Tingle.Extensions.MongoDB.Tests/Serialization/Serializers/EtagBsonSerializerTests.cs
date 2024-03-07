using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Tingle.Extensions.MongoDB.Serialization;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.MongoDB.Tests.Serialization.Serializers;

public class EtagBsonSerializerTests
{
    static EtagBsonSerializerTests() => BsonSerializer.RegisterSerializationProvider(new TingleSerializationProvider());

    [Theory]
    [InlineData(0UL)]
    [InlineData(123456789UL)]
    public void Deserialize_Works_For_String(ulong val)
    {
        var json = $"{{ '_id' : 'cake', 'Etag' : '{Convert.ToBase64String(BitConverter.GetBytes(val))}' }}".Replace("'", "\"");
        var result = BsonSerializer.Deserialize<Bookshop>(json);
        Assert.Equal("cake", result.Id);
        Assert.Equal<ulong>(val, result.Etag);
    }

    [Theory]
    [InlineData(0UL)]
    [InlineData(123456789UL)]
    public void Deserialize_Works_For_Int64(ulong val)
    {
        var json = $"{{ '_id' : 'cake', 'Etag' : NumberLong({val}) }}".Replace("'", "\"");
        var result = BsonSerializer.Deserialize<Bookshop>(json);
        Assert.Equal("cake", result.Id);
        Assert.Equal<ulong>(val, result.Etag);
    }

    [Theory]
    [InlineData(0UL)]
    [InlineData(123456789UL)]
    public void Deserialize_Works_For_Binary(ulong val)
    {
        var json = $"{{ '_id' : 'cake', 'Etag' : BinData(0, '{Convert.ToBase64String(BitConverter.GetBytes(val))}') }}".Replace("'", "\"");
        var result = BsonSerializer.Deserialize<Bookshop>(json);
        Assert.Equal("cake", result.Id);
        Assert.Equal<ulong>(val, result.Etag);
    }

    [Fact]
    public void Default_BsonRepresentation_IsUsed()
    {
        var obj = new Bookshop
        {
            Id = "cake",
            Etag = new Etag(123456789UL)
        };
        var json = obj.ToJson();
        var expected = $"{{ '_id' : 'cake', 'Etag' : NumberLong(123456789) }}".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson();
        var rehydrated = BsonSerializer.Deserialize<Bookshop>(bson);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson()));
        Assert.Equal<ulong>(123456789UL, rehydrated.Etag);
    }

    [Theory]
    [InlineData(123456789UL, typeof(BookshopString), "'Fc1bBwAAAAA='")]
    [InlineData(123456789UL, typeof(BookshopInt64), "NumberLong(123456789)")]
    [InlineData(123456789UL, typeof(BookshopBinary), "new BinData(0, 'Fc1bBwAAAAA=')")]
    public void BsonRepresentation_Is_Respected(ulong val, Type t, string bsonRaw)
    {
        var obj = (IBookshop)Activator.CreateInstance(t)!;
        obj.Etag = new Etag(val);
        var json = obj.ToJson(t);
        var expected = $"{{ 'Etag' : {bsonRaw} }}".Replace("'", "\"");
        Assert.Equal(expected, json);

        var bson = obj.ToBson(t);
        var rehydrated = (IBookshop)BsonSerializer.Deserialize(bson, t);
        Assert.True(bson.SequenceEqual(rehydrated.ToBson(t)));
        Assert.Equal<ulong>(val, rehydrated.Etag);
    }

    class Bookshop
    {
        public string? Id { get; set; }
        public Etag Etag { get; set; }
    }

    interface IBookshop { Etag Etag { get; set; } }
    class BookshopString : IBookshop { [BsonRepresentation(BsonType.String)] public Etag Etag { get; set; } }
    class BookshopInt64 : IBookshop { [BsonRepresentation(BsonType.Int64)] public Etag Etag { get; set; } }
    class BookshopBinary : IBookshop { [BsonRepresentation(BsonType.Binary)] public Etag Etag { get; set; } }
}
