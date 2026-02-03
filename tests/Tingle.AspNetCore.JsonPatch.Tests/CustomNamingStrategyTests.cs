using System.Dynamic;
using System.Text.Json;

namespace Tingle.AspNetCore.JsonPatch;

public class CustomNamingPolicyTests
{
    [Fact]
    public void OperationsRespectPropertyNamingPolicy()
    {
        // Arrange
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };

        var patchDocument = new JsonPatchDocument<SimpleObject>(serializerOptions);
        patchDocument.Replace(x => x.StringProperty, "Test");

        // Act
        var operation = Assert.Single(patchDocument.Operations);
        Assert.Equal("/string_property", operation.path);
    }

    [Fact]
    public void OperationsRespectDictionaryKeyPolicy()
    {
        // Arrange
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };

        var patchDocument = new JsonPatchDocument<ObjectWithDictionary>(serializerOptions);
        patchDocument.Replace(x => x.CustomData, "NamedKey", "Test");

        // Act
        var operation = Assert.Single(patchDocument.Operations);
        Assert.Equal("/custom_data/NamedKey", operation.path);
    }

    [Fact]
    public void AddProperty_ToDynamicTestObject_WithCustomNamingStrategy()
    {
        // Arrange
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new TestNamingPolicy()
        };

        dynamic targetObject = new DynamicTestObject();
        targetObject.Test = 1;

        var patchDocument = new JsonPatchDocument();
        patchDocument.Add("NewInt", 1);
        patchDocument.SerializerOptions = serializerOptions;

        // Act
        patchDocument.ApplyTo(targetObject);

        // Assert
        Assert.Equal(1, targetObject.customNewInt);
        Assert.Equal(1, targetObject.Test);
    }

    [Fact]
    public void CopyPropertyValue_ToDynamicTestObject_WithCustomNamingStrategy()
    {
        // Arrange
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new TestNamingPolicy()
        };

        dynamic targetObject = new DynamicTestObject();
        targetObject.customStringProperty = "A";
        targetObject.customAnotherStringProperty = "B";

        var patchDocument = new JsonPatchDocument();
        patchDocument.Copy("StringProperty", "AnotherStringProperty");
        patchDocument.SerializerOptions = serializerOptions;

        // Act
        patchDocument.ApplyTo(targetObject);

        // Assert
        Assert.Equal("A", targetObject.customAnotherStringProperty);
    }

    [Fact]
    public void MovePropertyValue_ForExpandoObject_WithCustomNamingStrategy()
    {
        // Arrange
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new TestNamingPolicy()
        };

        dynamic targetObject = new ExpandoObject();
        targetObject.customStringProperty = "A";
        targetObject.customAnotherStringProperty = "B";

        var patchDocument = new JsonPatchDocument();
        patchDocument.Move("StringProperty", "AnotherStringProperty");
        patchDocument.SerializerOptions = serializerOptions;

        // Act
        patchDocument.ApplyTo(targetObject);
        var cont = (IDictionary<string, object>)targetObject;
        cont.TryGetValue("customStringProperty", out var valueFromDictionary);

        // Assert
        Assert.Equal("A", targetObject.customAnotherStringProperty);
        Assert.Null(valueFromDictionary);
    }

    [Fact]
    public void RemoveProperty_FromDictionaryObject_WithCustomNamingStrategy()
    {
        // Arrange
        var serializerOptions = new JsonSerializerOptions
        {
            DictionaryKeyPolicy = new TestNamingPolicy(),
        };

        var targetObject = new Dictionary<string, int>()
        {
            { "customTest", 1},
        };

        var patchDocument = new JsonPatchDocument();
        patchDocument.Remove("Test");
        patchDocument.SerializerOptions = serializerOptions;

        // Act
        patchDocument.ApplyTo(targetObject);
        var cont = targetObject as IDictionary<string, int>;
        cont.TryGetValue("customTest", out var valueFromDictionary);

        // Assert
        Assert.Equal(0, valueFromDictionary);
    }

    [Fact]
    public void ReplacePropertyValue_ForExpandoObject_WithCustomNamingStrategy()
    {
        // Arrange
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new TestNamingPolicy()
        };

        dynamic targetObject = new ExpandoObject();
        targetObject.customTest = 1;

        var patchDocument = new JsonPatchDocument();
        patchDocument.Replace("Test", 2);
        patchDocument.SerializerOptions = serializerOptions;

        // Act
        patchDocument.ApplyTo(targetObject);

        // Assert
        Assert.Equal(2, targetObject.customTest);
    }

    private class TestNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => "custom" + name;
    }
}