using System.Text.Json;
using System.Text.Json.Nodes;
using SystemTextJsonPatch;
using Tingle.AspNetCore.JsonPatch.Exceptions;
using Tingle.AspNetCore.JsonPatch.Operations;

namespace Tingle.AspNetCore.JsonPatch;

public class JsonPatchDocumentJsonObjectTest
{
    [Fact]
    public void ApplyTo_Array_Add()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { Emails = new[] { "foo@bar.com" } })!, };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("add", "/CustomData/Emails/-", null, "foo@baz.com"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Equal("foo@bar.com", model.CustomData["Emails"]![0]!.GetValue<string>());
        Assert.Equal("foo@baz.com", model.CustomData["Emails"]![1]!.GetValue<string>());
    }

    [Fact]
    public void ApplyTo_Model_Test1()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { Email = "foo@bar.com", Name = "Bar" })!, };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("test", "/CustomData/Email", null, "foo@baz.com"));
        patch.Operations.Add(new Operation<ObjectWithJsonNode>("add", "/CustomData/Name", null, "Bar Baz"));

        // Act & Assert
        Assert.Throws<JsonPatchException>(() => patch.ApplyTo(model));
    }

    [Fact]
    public void ApplyTo_Model_Test2()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { Email = "foo@bar.com", Name = "Bar" })!, };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("test", "/CustomData/Email", null, "foo@bar.com"));
        patch.Operations.Add(new Operation<ObjectWithJsonNode>("add", "/CustomData/Name", null, "Bar Baz"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Equal("Bar Baz", model.CustomData["Name"]!.GetValue<string>());
    }

    [Fact]
    public void ApplyTo_Model_Copy()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { Email = "foo@bar.com" })!, };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("copy", "/CustomData/UserName", "/CustomData/Email"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Equal("foo@bar.com", model.CustomData["UserName"]!.GetValue<string>());
    }

    [Fact]
    public void ApplyTo_Model_Remove()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { FirstName = "Foo", LastName = "Bar" })!, };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("remove", "/CustomData/LastName", null));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Null(model.CustomData["LastName"]);
    }

    [Fact]
    public void ApplyTo_Model_Move()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { FirstName = "Bar" })!, };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("move", "/CustomData/LastName", "/CustomData/FirstName"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Null(model.CustomData["FirstName"]);
        Assert.Equal("Bar", model.CustomData["LastName"]!.GetValue<string>());
    }

    [Fact]
    public void ApplyTo_Model_Add()
    {
        // Arrange
        var model = new ObjectWithJsonNode();
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("add", "/CustomData/Name", null, "Foo"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Equal("Foo", model.CustomData["Name"]!.GetValue<string>());
    }

    [Fact]
    public void ApplyTo_Model_Add_Null()
    {
        // Arrange
        var model = new ObjectWithJsonNode();
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("add", "/CustomData/Name", null, null));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Null(model.CustomData["Name"]);
    }

    [Fact]
    public void ApplyTo_Model_Replace()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { Email = "foo@bar.com", Name = "Bar" })!, };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("replace", "/CustomData/Email", null, "foo@baz.com"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Equal("foo@baz.com", model.CustomData["Email"]!.GetValue<string>());
    }

    [Fact]
    public void ApplyTo_Model_Replace_Null()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { Email = "foo@bar.com", Name = "Bar" })!, };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("replace", "/CustomData/Email", null, null));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Null(model.CustomData["Email"]);
    }


    [Fact]
    public void ReplaceJsonNodeWithNewJson()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { Testing = "JsonNodes" })!, };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("replace", "/CustomData", null, "{\"foo\": \"bar\"}"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Equal("{\"foo\": \"bar\"}", model.CustomData.ToString());
    }

    [Fact]
    public void ReplaceJsonElementWithNewJson()
    {
        // Arrange
        var model = new ObjectWithJsonElement { CustomData = JsonSerializer.SerializeToElement(new { Testing = "JsonNodes" }) };
        var patch = new JsonPatchDocument<ObjectWithJsonElement>();

        patch.Operations.Add(new Operation<ObjectWithJsonElement>("replace", "/CustomData", null, "{\"foo\": \"bar\"}"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Equal("{\"foo\": \"bar\"}", model.CustomData.ToString());
    }

    [Fact]
    public void ReplaceJsonDocumentWithNewJson()
    {
        // Arrange
        var model = new ObjectWithJsonDocument { CustomData = JsonSerializer.SerializeToDocument(new { Testing = "JsonNodes" }) };
        var patch = new JsonPatchDocument<ObjectWithJsonDocument>();

        patch.Operations.Add(new Operation<ObjectWithJsonDocument>("replace", "/CustomData", null, "{\"foo\": \"bar\"}"));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Equal("{\"foo\": \"bar\"}", model.CustomData.RootElement.ToString());
    }

    [Fact]
    public void ReplaceArrayCell()
    {
        var node = JsonSerializer.Deserialize<JsonArray>("[ 123 ]")!;
        var patch = JsonSerializer.Deserialize<JsonPatchDocument>(@"[ { ""op"": ""replace"", ""path"": ""/0"", ""value"": 456 } ]")!;

        patch.ApplyTo(node);

        Assert.Equal(456, node.ElementAt(0)!.GetValue<int>());
        Assert.Single(node);
    }

    [Fact]
    public void ReplaceJsonObjInArray0Idx()
    {
        var node = JsonSerializer.Deserialize<JsonArray>("[ {\"a\": \"12\"}, {\"a\": \"13\"} ]")!;
        var patch = JsonSerializer.Deserialize<JsonPatchDocument>(@"[ { ""op"": ""replace"", ""path"": ""/0/a"", ""value"": ""456"" } ]")!;

        patch.ApplyTo(node);

        var resultJson = node.ToJsonString();

        Assert.Equal("[{\"a\":\"456\"},{\"a\":\"13\"}]", resultJson);
    }

    [Fact]
    public void ReplaceJsonObjInArray1Idx()
    {
        var node = JsonSerializer.Deserialize<JsonArray>("[ {\"a\": \"12\"}, {\"a\": \"12\"}, {\"a\": \"13\"} ]")!;
        var patch = JsonSerializer.Deserialize<JsonPatchDocument>(@"[ { ""op"": ""replace"", ""path"": ""/1/a"", ""value"": ""456"" } ]")!;

        patch.ApplyTo(node);

        var resultJson = node.ToJsonString();

        Assert.Equal("[{\"a\":\"12\"},{\"a\":\"456\"},{\"a\":\"13\"}]", resultJson);
    }

    [Fact]
    public void ReplaceJsonObjInArrayLastIdx()
    {
        var node = JsonSerializer.Deserialize<JsonArray>("[ {\"a\": \"12\"}, {\"a\": \"12\"}, {\"a\": \"13\"} ]")!;
        var patch = JsonSerializer.Deserialize<JsonPatchDocument>(@"[ { ""op"": ""replace"", ""path"": ""/-/a"", ""value"": ""456"" } ]")!;

        patch.ApplyTo(node);

        var resultJson = node.ToJsonString();

        Assert.Equal("[{\"a\":\"12\"},{\"a\":\"12\"},{\"a\":\"456\"}]", resultJson);
    }

    [Fact]
    public void ReplaceJsonObjInArrayMultipleTimes()
    {
        var node = JsonSerializer.Deserialize<JsonArray>("[ {\"a\": \"12\"}, {\"a\": \"12\"}, {\"a\": \"13\"} ]")!;
        var patch = JsonSerializer.Deserialize<JsonPatchDocument>(@"[ { ""op"": ""replace"", ""path"": ""/1/a"", ""value"": ""456"" } ]")!;

        patch.ApplyTo(node);

        patch = JsonSerializer.Deserialize<JsonPatchDocument>(@"[ { ""op"": ""replace"", ""path"": ""/1/a"", ""value"": ""33"" } ]")!;

        patch.ApplyTo(node);

        var resultJson = node.ToJsonString();

        Assert.Equal("[{\"a\":\"12\"},{\"a\":\"33\"},{\"a\":\"13\"}]", resultJson);
    }

    [Fact]
    public void ReplaceJsonProp()
    {
        var node = JsonSerializer.Deserialize<JsonObject>("{\"a\": \"12\"}")!;
        var patch = JsonSerializer.Deserialize<JsonPatchDocument>(@"[ { ""op"": ""replace"", ""path"": ""/a"", ""value"": 456 } ]")!;

        patch.ApplyTo(node);

        node.TryGetPropertyValue("a", out var prop);

        Assert.Equal("456", prop!.ToString());
    }

    [Fact]
    public void ApplyToArrayAddAndRemove()
    {
        // Arrange
        var model = new ObjectWithJsonNode { CustomData = JsonSerializer.SerializeToNode(new { Emails = new[] { "foo@bar.com" } })!, };
        var patch = new JsonPatchDocument<ObjectWithJsonNode>();

        patch.Operations.Add(new Operation<ObjectWithJsonNode>("add", "/CustomData/Emails/-", null, "foo@baz.com"));
        patch.Operations.Add(new Operation<ObjectWithJsonNode>("remove", "/CustomData/Emails/-", null));

        // Act
        patch.ApplyTo(model);

        // Assert
        Assert.Equal("foo@bar.com", model.CustomData["Emails"]![0]!.GetValue<string>());
    }
}
