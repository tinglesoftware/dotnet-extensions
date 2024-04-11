using System.Text.Json;
using System.Text.Json.Nodes;
using Tingle.AspNetCore.JsonPatch.Converters;

namespace Tingle.AspNetCore.JsonPatch;

public class JsonMergePatchDocumentConverterHelperTests
{
    [Fact]
    public void PopulateOperations_Works()
    {
        var node = new JsonObject
        {
            ["translations"] = new JsonObject
            {
                ["swa"] = new JsonObject
                {
                    ["body"] = "rudi shule",
                    ["provider"] = "google",
                },
            },
            ["metadata"] = new JsonObject
            {
                ["primary"] = "hapa tu",
                ["secondary"] = "pale tu",
            },
            ["tags"] = new JsonArray { "prod", "ken", },
            ["description"] = "immigration",
            ["name"] = null,
        };

        var operations = new List<Operations.Operation<Video>>();
        JsonMergePatchDocumentConverterHelper.PopulateOperations(operations, node);

        Assert.Equal([
            "add",
            "add",

            "add",
            "add",

            "add",
            "add",

            "add",
            "remove",
        ], operations.Select(o => o.op));
        Assert.Equal([
            "/translations/swa/body",
            "/translations/swa/provider",

            "/metadata/primary",
            "/metadata/secondary",

            "/tags/0",
            "/tags/1",

            "/description",
            "/name",
        ], operations.Select(o => o.path));
        Assert.Equal([
            "rudi shule",
            "google",

            "hapa tu",
            "pale tu",

            "prod",
            "ken",

            "immigration",
            null,
        ], operations.Select(o => ((JsonElement?)o.value)?.ToString()));
    }

    [Fact]
    public void PopulateJsonObject_Works()
    {
        var operations = new List<Operations.Operation<Video>>
        {
            new() { op = "add", path = "/translations/swa/body", value = "rudi shule" },
            new() { op = "add", path = "/translations/swa/provider", value = "google" },
            new() { op = "add", path = "/metadata/primary", value = "hapa tu" },
            new() { op = "add", path = "/metadata/secondary", value = "pale tu" },
            //new() { op = "add", path = "/tags/0", value = "prod" },
            //new() { op = "add", path = "/tags/1", value = "ken" },
            new() { op = "add", path = "/description", value = "immigration" },
            new() { op = "remove", path = "/name" },
        };

        var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        var node = new JsonObject();
        foreach (var operation in operations)
        {
            var type = operation.OperationType;
            var segments = operation.path.Trim('/').Split('/'); ;
            var opvalue = type is Operations.OperationType.Remove ? null : operation.value;
            JsonMergePatchDocumentConverterHelper.PopulateJsonObject(node, segments, opvalue, serializerOptions);
        }

        var expected = new JsonObject
        {
            ["translations"] = new JsonObject
            {
                ["swa"] = new JsonObject
                {
                    ["body"] = "rudi shule",
                    ["provider"] = "google",
                },
            },
            ["metadata"] = new JsonObject
            {
                ["primary"] = "hapa tu",
                ["secondary"] = "pale tu",
            },
            //["tags"] = new JsonArray { "prod", "ken", },
            ["description"] = "immigration",
            ["name"] = null,
        };

        Assert.Equal(expected.ToJsonString(serializerOptions), node.ToJsonString(serializerOptions));
    }

    [Fact]
    public void Apply_Works()
    {
        var node = new JsonObject
        {
            ["translations"] = new JsonObject
            {
                ["swa"] = new JsonObject
                {
                    ["body"] = "rudi shule",
                    ["provider"] = "google",
                },
            },
            ["metadata"] = new JsonObject
            {
                ["primary"] = "hapa tu",
                ["secondary"] = "pale tu",
            },
            ["tags"] = new JsonArray { "prod", "ken", },
            ["description"] = "immigration",
            ["name"] = null,
        };

        var video = new Video { Metadata = new() { ["primary"] = "cake", } };
        var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var doc = JsonSerializer.Deserialize<JsonMergePatchDocument<Video>>(node.ToJsonString(), serializerOptions)!;
        doc.ApplyTo(video);

        Assert.Equal("rudi shule", Assert.Contains("swa", video.Translations).Body);
        Assert.Equal("google", Assert.Contains("swa", video.Translations).Provider);
        Assert.Equal("hapa tu", Assert.Contains("primary", video.Metadata));
        Assert.Equal("pale tu", Assert.Contains("secondary", video.Metadata));
        Assert.Equal(["prod", "ken"], video.Tags);
        Assert.Equal("immigration", video.Description);
        Assert.Null(video.Name);
        Assert.Equal("123", video.Id);
    }

    class Video
    {
        public Dictionary<string, VideoTranslation> Translations { get; set; } = [];
        public Dictionary<string, string> Metadata { get; set; } = [];
        public List<string> Tags { get; set; } = [];
        public string? Description { get; set; }
        public string Name { get; set; } = "nat-geo";
        public string Id { get; set; } = "123";
    }

    class VideoTranslation
    {
        public string? Body { get; set; }
        public string? Provider { get; set; }
    }
}
