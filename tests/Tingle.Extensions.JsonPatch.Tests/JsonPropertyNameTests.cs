using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tingle.Extensions.JsonPatch.Operations;
using Xunit;

namespace Tingle.Extensions.JsonPatch.Tests;

public class JsonPropertyNameTests
{
    [Fact]
    public void HonourJsonPropertyOnSerialization()
    {
        // create patch
        var patchDoc = new JsonPatchDocument<JsonPropertyDTO>();
        patchDoc.Add(p => p.Name, "Kevin");

        var serialized = JsonSerializer.Serialize(patchDoc);
        // serialized value should have "AnotherName" as path
        // deserialize to a JsonPatchDocument<JsonPropertyWithAnotherNameDTO> to check
        var deserialized = JsonSerializer.Deserialize<JsonPatchDocument<JsonPropertyWithAnotherNameDTO>>(serialized)!;

        Assert.Equal("/anothername", deserialized.Operations.First().path);
    }

    [Fact]
    public static void SerializeProducesExpectedJson()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        options.Converters.Add(new JsonStringEnumConverter(options.PropertyNamingPolicy));

        var parts = new[]{
                @"{""value"":""animals"",""path"":""/description"",""op"":""replace""}",
                @"{""path"":""/status"",""op"":""remove""}",
                @"{""value"":""justCrap"",""path"":""/kind"",""op"":""replace""}",
                @"{""value"":""science"",""path"":""/tags/-"",""op"":""add""}",
                @"{""value"":""research"",""path"":""/metadata/purpose"",""op"":""add""}",
                @"{""value"":""nutrition"",""path"":""/metadata/purpose"",""op"":""replace""}",
                @"{""path"":""/metadata/purpose"",""op"":""remove""}",
            };
        var expected = $"[{string.Join(",", parts)}]";

        var document = new JsonPatchDocument<AnotherDTO>();
        document.Replace(a => a.Description, "animals");
        document.Remove(a => a.Status);
        document.Replace(a => a.Kind, DtoKind.JustCrap);
        document.Add(a => a.Tags!, "science");
        document.Add(a => a.Metadata!, "purpose", "research");
        document.Replace(a => a.Metadata!, "purpose", "nutrition");
        document.Remove(a => a.Metadata!, "purpose");
        var actual = JsonSerializer.Serialize(document, options);

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public static void DeSerializeProducesExpectedDocument()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        options.Converters.Add(new JsonStringEnumConverter(options.PropertyNamingPolicy));

        var parts = new[]{
                @"{""value"":""animals"",""path"":""/description"",""op"":""replace""}",
                @"{""path"":""/status"",""op"":""remove""}",
                @"{""value"":""justCrap"",""path"":""/kind"",""op"":""replace""}",
                @"{""value"":""science"",""path"":""/tags/-"",""op"":""add""}",
            };
        var json = $"[{string.Join(",", parts)}]";

        var document = JsonSerializer.Deserialize<JsonPatchDocument<AnotherDTO>>(json, options);
        Assert.NotNull(document);

        var ops = document!.Operations;
        Assert.Equal(4, ops.Count);

        var first = ops.FirstOrDefault();
        Assert.NotNull(first);
        Assert.Equal("replace", first!.op);
        Assert.Equal(OperationType.Replace, first.OperationType);
        Assert.Equal("/description", first.path);
        //Assert.Equal("animals", first.value);

        var second = ops.Skip(1).FirstOrDefault();
        Assert.NotNull(second);
        Assert.Equal("remove", second!.op);
        Assert.Equal(OperationType.Remove, second.OperationType);
        Assert.Equal("/status", second.path);
        Assert.Null(second.value);

        var third = ops.Skip(2).FirstOrDefault();
        Assert.NotNull(third);
        Assert.Equal("replace", third!.op);
        Assert.Equal(OperationType.Replace, third.OperationType);
        Assert.Equal("/kind", third.path);
        //Assert.Equal("justCrap", third.value);

        var fourth = ops.Skip(3).FirstOrDefault();
        Assert.NotNull(fourth);
        Assert.Equal("add", fourth!.op);
        Assert.Equal(OperationType.Add, fourth.OperationType);
        Assert.Equal("/tags/-", fourth.path);
        //Assert.Equal("science", fourth.value);
    }
}
