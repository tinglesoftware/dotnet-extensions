using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.AspNetCore.JsonPatch;

public class JsonPatchValidationTest
{
    [Fact]
    public void Validation_Fails_With_PatchModel()
    {
        // ideally retrieved from the database
        var target = new TestModel
        {
            Id = "test1",
            Name = "John",
            Age = 20,
        };

        var options = new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
#if NET8_0_OR_GREATER
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
#else
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
#endif
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,

        };
        var doc = JsonSerializer.Deserialize<JsonPatchDocument<TestPatchModel>>(
                        JsonSerializer.Serialize(
                            new JsonPatchDocument<TestModel>(options).Replace(x => x.Id, "test2"), options), options)!;
        var modelState = new ModelStateDictionary();
        doc.ApplyToSafely(target, modelState);
        Assert.False(modelState.IsValid);
        var error = Assert.Single(Assert.Single(modelState).Value!.Errors);
        Assert.Equal("The property at path '/id' is immutable or does not exist.", error.ErrorMessage);
    }

    [Fact]
    public void Validation_Passes()
    {
        // ideally retrieved from the database
        var target = new TestModel
        {
            Id = "test1",
            Name = "John",
            Age = 20,
        };

        // test with JsonPatchDocument<TestModel>
        var options = new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
#if NET8_0_OR_GREATER
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
#else
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
#endif
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,

        };
        var doc = JsonSerializer.Deserialize<JsonPatchDocument<TestPatchModel>>(
                        JsonSerializer.Serialize(
                            new JsonPatchDocument<TestModel>(options).Replace(x => x.Name, "Alice"), options), options)!;
        var modelState = new ModelStateDictionary();
        doc.ApplyToSafely(target, modelState);
        Assert.True(modelState.IsValid);
        Assert.Empty(modelState);
        Assert.Equal("Alice", target.Name);

        // test with JsonPatchDocument<TestPatchModel>
        doc = new JsonPatchDocument<TestPatchModel>().Replace(x => x.Name, "David");
        modelState = new ModelStateDictionary();
        doc.ApplyToSafely(target, modelState);
        Assert.True(modelState.IsValid);
        Assert.Empty(modelState);
        Assert.Equal("David", target.Name);
    }

    [Fact]
    public void Validation_Passes_CompoundPropertyName()
    {
        // ideally retrieved from the database
        var target = new TestModel
        {
            Id = "test1",
            Name = "John",
            Age = 20,
            Inner = new TestInnerModel
            {
                Batch = "001"
            },
        };

        // test with compound property names
#if NET8_0_OR_GREATER
        var json = "[{\"op\":\"replace\",\"path\":\"/middle_name\",\"value\":\"Kamau\"},{\"op\":\"add\",\"path\":\"/extra_metadata/strength\",\"value\":\"average\"}]";
#else
        var json = "[{\"op\":\"replace\",\"path\":\"/middleName\",\"value\":\"Kamau\"},{\"op\":\"add\",\"path\":\"/extraMetadata/strength\",\"value\":\"average\"}]";
#endif
        var options = new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
#if NET8_0_OR_GREATER
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
#else
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
#endif
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,

        };
        var doc = JsonSerializer.Deserialize<JsonPatchDocument<TestPatchModel>>(json, options)!;
        var modelState = new ModelStateDictionary();
        doc.ApplyToSafely(target, modelState);
        Assert.True(modelState.IsValid);
        Assert.Empty(modelState);
        Assert.Equal("Kamau", target.MiddleName);
    }

    [Fact]
    public void Validation_Passes_Inner()
    {
        // ideally retrieved from the database
        var target = new TestModel
        {
            Id = "test1",
            Name = "John",
            Age = 20,
            Inner = new TestInnerModel
            {
                Batch = "001"
            },
        };

        // test with inner
        var doc = new JsonPatchDocument<TestPatchModel>().Replace(x => x.Inner!.Batch, "002");
        var modelState = new ModelStateDictionary();
        doc.ApplyToSafely(target, modelState);
        Assert.True(modelState.IsValid);
        Assert.Empty(modelState);
        Assert.Equal("002", target.Inner.Batch);

        // test with list
        doc = new JsonPatchDocument<TestPatchModel>().Add(x => x.Tags, "promo");
        modelState = new ModelStateDictionary();
        doc.ApplyToSafely(target, modelState);
        Assert.True(modelState.IsValid);
        Assert.Empty(modelState);
        Assert.Equal("promo", Assert.Single(target.Tags));

        // test with metadata (Add)
        doc = new JsonPatchDocument<TestPatchModel>().Add(x => x.Metadata, "kind", "tests");
        modelState = new ModelStateDictionary();
        doc.ApplyToSafely(target, modelState);
        Assert.True(modelState.IsValid);
        Assert.Empty(modelState);
        Assert.Equal("tests", Assert.Contains("kind", target.Metadata));

        // test with metadata (Replace)
        doc = new JsonPatchDocument<TestPatchModel>().Replace(x => x.Metadata, "kind", "warning");
        modelState = new ModelStateDictionary();
        doc.ApplyToSafely(target, modelState);
        Assert.True(modelState.IsValid);
        Assert.Empty(modelState);
        Assert.Equal("warning", Assert.Contains("kind", target.Metadata));

        // test with metadata (Remove)
        doc = new JsonPatchDocument<TestPatchModel>().Remove(x => x.Metadata, "kind");
        modelState = new ModelStateDictionary();
        doc.ApplyToSafely(target, modelState);
        Assert.True(modelState.IsValid);
        Assert.Empty(modelState);
        Assert.Empty(target.Metadata);
    }

    class TestInnerModel
    {
        public string? Batch { get; set; }
    }

    class TestPatchModel
    {
        public string? Name { get; set; }

        public string? MiddleName { get; set; }

        public TestInnerModel? Inner { get; set; }

        public List<string> Tags { get; set; } = [];

        public Dictionary<string, string> Metadata { get; set; } = [];
        public Dictionary<string, string> ExtraMetadata { get; set; } = [];
    }

    class TestModel : TestPatchModel
    {
        public string? Id { get; set; }

        public int Age { get; set; }
    }
}
