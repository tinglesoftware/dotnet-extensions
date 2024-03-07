using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Tingle.Extensions.MongoDB.Serialization.Conventions;

namespace Tingle.Extensions.MongoDB.Tests.Serialization.Conventions;

public class DateTimeOffsetRepresentationConventionTests
{
    [Theory]
    [InlineData(BsonType.Array)]
    [InlineData(BsonType.Document)]
    [InlineData(BsonType.DateTime)]
    [InlineData(BsonType.String)]
    public void Convention_Is_Used_When_Memeber_Is_A_DateTimeOffset(BsonType representation)
    {
        var subject = new DateTimeOffsetRepresentationConvention(representation);
        var classMap = new BsonClassMap<Bookshop>();
        var memberMap = classMap.MapMember(b => b.Created);

        subject.Apply(memberMap);

        var serializer = (DateTimeOffsetSerializer)memberMap.GetSerializer();
        Assert.Equal(representation, serializer.Representation);
    }

    [Theory]
    [InlineData(BsonType.Array)]
    [InlineData(BsonType.Document)]
    [InlineData(BsonType.DateTime)]
    [InlineData(BsonType.String)]
    public void Convention_Is_Used_When_Memeber_Is_A_Nullable_DateTimeOffset(BsonType representation)
    {
        var subject = new DateTimeOffsetRepresentationConvention(representation);
        var classMap = new BsonClassMap<Bookshop>();
        var memberMap = classMap.MapMember(b => b.Closed);

        subject.Apply(memberMap);

        var serializer = (IChildSerializerConfigurable)memberMap.GetSerializer();
        var childSerializer = (DateTimeOffsetSerializer)serializer.ChildSerializer;
        Assert.Equal(representation, childSerializer.Representation);
    }

    [Fact]
    public void Convention_Is_Not_Used_When_Memeber_Is_Not_A_DateTimeOffset()
    {
        var subject = new DateTimeOffsetRepresentationConvention(BsonType.Array);
        var classMap = new BsonClassMap<Bookshop>();
        var memberMap = classMap.MapMember(b => b.Created);

        var serializer = memberMap.GetSerializer();

        subject.Apply(memberMap);

        Assert.Equal(serializer, memberMap.GetSerializer());
    }

    [Theory]
    [InlineData(BsonType.Array)]
    [InlineData(BsonType.Document)]
    [InlineData(BsonType.DateTime)]
    [InlineData(BsonType.String)]
    public void Constructor_Should_Initialize_Instance_When_Representation_Is_Valid(BsonType representation)
    {
        var subject = new DateTimeOffsetRepresentationConvention(representation);

        Assert.Equal(representation, subject.Representation);
    }

    [Theory]
    [InlineData(BsonType.Decimal128)]
    [InlineData(BsonType.Double)]
    public void Constructor_Should_Throw_When_Representation_Is_Not_Valid(BsonType representation)
    {
        var exception = Assert.Throws<ArgumentException>(() => new DateTimeOffsetRepresentationConvention(representation));

        Assert.Equal("representation", exception.ParamName);
    }


    class Bookshop
    {
        public string? Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Closed { get; set; }
        public int Count { get; set; }
    }
}
