using System.Text.Json.Nodes;

namespace Tingle.AspNetCore.JsonPatch;

public class ObjectWithJsonNode
{
    public JsonNode CustomData { get; set; } = JsonNode.Parse("{}")!;
}
