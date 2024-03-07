using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tingle.Extensions.MongoDB.Serialization.Conventions;

namespace Tingle.Extensions.MongoDB.Tests.Serialization.Conventions;

public class SystemComponentModelAttributesConventionTests
{
    [Fact]
    public void TestApply()
    {
        var subject = new SystemComponentModelAttributesConvention();
        var classMap = new BsonClassMap<TestClass>();

        classMap.AutoMap();

        Assert.Null(classMap.IdMemberMap);
        subject.Apply(classMap);
        Assert.NotNull(classMap.IdMemberMap);
        Assert.Equal(typeof(string), classMap.IdMemberMap.MemberType);
        Assert.Equal("SomeKey", classMap.IdMemberMap.ElementName);

        Assert.Equal(["SomeKey", "NormalValue"],
                     classMap.DeclaredMemberMaps.Select(m => m.MemberName));
    }

    //[Fact]
    //public void Serialization_Works()
    //{
    //    ConventionRegistry.Register("Test", TestConventionPack.Instance, _ => true);

    //    var json = $"{{ '_id' : 'cake', 'NormalValue' : NumberLong(2) }}".Replace("'", "\"");
    //    var result = BsonSerializer.Deserialize<TestClass>(json);
    //    Assert.Equal("cake", result.SomeKey);
    //    var bson = result.ToBson();
    //    Assert.Equal(json, result.ToJson());
    //}

    private class TestClass
    {
        [Key]
        public string? SomeKey { get; set; }

        [NotMapped]
        public int SomeValue { get; set; }

        public long NormalValue { get; set; }
    }

    private class TestConventionPack : IConventionPack
    {
        private static readonly IConventionPack __defaultConventionPack = new TestConventionPack();
        private readonly IEnumerable<IConvention> _conventions = new List<IConvention> { new SystemComponentModelAttributesConvention(), };

        public static IConventionPack Instance => __defaultConventionPack;
        public IEnumerable<IConvention> Conventions => _conventions;
    }
}
