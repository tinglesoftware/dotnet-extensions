using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Tingle.AspNetCore.JsonPatch.NewtonsoftJson.Tests;

public class ValidationTest
{
    [Fact]
    public void Validation_Fails_With_ImmutableList()
    {
        // ideally retrieved from the database
        var target = new TestModel
        {
            Id = "test1",
            Name = "John",
            Age = 20,
        };

        var doc = new JsonPatchDocument<TestModel>().Replace(x => x.Id, "test2");
        var modelState = new ModelStateDictionary();
        doc.ApplyToSafely(target, modelState, new List<string> { nameof(TestModel.Id), });
        Assert.False(modelState.IsValid);
        var error = Assert.Single(Assert.Single(modelState).Value!.Errors);
        Assert.Equal("The property at path '/Id' is immutable.", error.ErrorMessage);
    }

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

        var doc = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonPatchDocument<TestPatchModel>>(
                        Newtonsoft.Json.JsonConvert.SerializeObject(
                            new JsonPatchDocument<TestModel>().Replace(x => x.Id, "test2")))!;
        var modelState = new ModelStateDictionary();
        doc.ApplyToSafely(target, modelState);
        Assert.False(modelState.IsValid);
        var error = Assert.Single(Assert.Single(modelState).Value!.Errors);
        Assert.Equal("The property at path '/Id' is immutable or does not exist.", error.ErrorMessage);
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
        var doc = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonPatchDocument<TestPatchModel>>(
                        Newtonsoft.Json.JsonConvert.SerializeObject(
                            new JsonPatchDocument<TestModel>().Replace(x => x.Name, "Alice")))!;
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
        var json = "[{\"op\":\"replace\",\"path\":\"/middle_name\",\"value\":\"Kamau\"}]";
        var doc = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonPatchDocument<TestPatchModel>>(json)!;
        doc.ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy(), };
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
        Assert.Equal("tests", Assert.Contains("kind", (IDictionary<string, string>)target.Metadata));

        // test with metadata (Replace)
        doc = new JsonPatchDocument<TestPatchModel>().Replace(x => x.Metadata, "kind", "warning");
        modelState = new ModelStateDictionary();
        doc.ApplyToSafely(target, modelState);
        Assert.True(modelState.IsValid);
        Assert.Empty(modelState);
        Assert.Equal("warning", Assert.Contains("kind", (IDictionary<string, string>)target.Metadata));

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

        public List<string> Tags { get; set; } = new List<string>();

        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }

    class TestModel : TestPatchModel
    {
        [Key]
        public string? Id { get; set; }

        [Range(18, 65)]
        public int Age { get; set; }
    }
}
