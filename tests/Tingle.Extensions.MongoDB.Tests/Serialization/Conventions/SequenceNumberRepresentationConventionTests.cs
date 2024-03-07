using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Tingle.Extensions.MongoDB.Serialization;
using Tingle.Extensions.MongoDB.Serialization.Conventions;
using Tingle.Extensions.MongoDB.Serialization.Serializers;
using Tingle.Extensions.Primitives;

namespace Tingle.Extensions.MongoDB.Tests.Serialization.Conventions;

public class SequenceNumberRepresentationConventionTests
{
    static SequenceNumberRepresentationConventionTests() => BsonSerializer.RegisterSerializationProvider(new TingleSerializationProvider());

    [Theory]
    [InlineData(BsonType.Int64)]
    [InlineData(BsonType.String)]
    public void Convention_Is_Used_When_Memeber_Is_A_SequenceNumber(BsonType representation)
    {
        var subject = new SequenceNumberRepresentationConvention(representation);
        var classMap = new BsonClassMap<Bookshop>();
        var memberMap = classMap.MapMember(b => b.First);

        subject.Apply(memberMap);

        var serializer = (SequenceNumberBsonSerializer)memberMap.GetSerializer();
        Assert.Equal(representation, serializer.Representation);
    }

    [Theory]
    [InlineData(BsonType.Int64)]
    [InlineData(BsonType.String)]
    public void Convention_Is_Used_When_Memeber_Is_A_Nullable_SequenceNumber(BsonType representation)
    {
        var subject = new SequenceNumberRepresentationConvention(representation);
        var classMap = new BsonClassMap<Bookshop>();
        var memberMap = classMap.MapMember(b => b.Second);

        subject.Apply(memberMap);

        var serializer = (IChildSerializerConfigurable)memberMap.GetSerializer();
        var childSerializer = (SequenceNumberBsonSerializer)serializer.ChildSerializer;
        Assert.Equal(representation, childSerializer.Representation);
    }

    [Fact]
    public void Convention_Is_Not_Used_When_Memeber_Is_Not_A_SequenceNumber()
    {
        var subject = new SequenceNumberRepresentationConvention(BsonType.Int64);
        var classMap = new BsonClassMap<Bookshop>();
        var memberMap = classMap.MapMember(b => b.First);

        var serializer = memberMap.GetSerializer();

        subject.Apply(memberMap);

        Assert.Equal(serializer, memberMap.GetSerializer());
    }

    [Theory]
    [InlineData(BsonType.Int64)]
    [InlineData(BsonType.String)]
    public void Constructor_Should_Initialize_Instance_When_Representation_Is_Valid(BsonType representation)
    {
        var subject = new SequenceNumberRepresentationConvention(representation);

        Assert.Equal(representation, subject.Representation);
    }

    [Theory]
    [InlineData(BsonType.Decimal128)]
    [InlineData(BsonType.Double)]
    public void Constructor_Should_Throw_When_Representation_Is_Not_Valid(BsonType representation)
    {
        var exception = Assert.Throws<ArgumentException>(() => new SequenceNumberRepresentationConvention(representation));

        Assert.Equal("representation", exception.ParamName);
    }


    class Bookshop
    {
        public string? Id { get; set; }
        public SequenceNumber First { get; set; }
        public SequenceNumber? Second { get; set; }
        public int Count { get; set; }
    }
}
