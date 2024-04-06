using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Tingle.AspNetCore.JsonPatch.Converters;

public class JsonPatchMergeDocumentConverter : JsonConverter<JsonPatchMergeDocument>
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(JsonPatchMergeDocument);

    /// <inheritdoc/>
    public override JsonPatchMergeDocument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;
        if (reader.TokenType is not JsonTokenType.StartObject)
        {
            throw new InvalidOperationException("Only objects are supported");
        }

        var no = new JsonNodeOptions { PropertyNameCaseInsensitive = options.PropertyNameCaseInsensitive, };
        var node = JsonNode.Parse(ref reader, no)!.AsObject();
        var operations = new List<Operations.Operation>();
        JsonPatchMergeDocumentConverterHelper.PopulateOperations(operations, node);

        return new JsonPatchMergeDocument(operations, options);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, JsonPatchMergeDocument value, JsonSerializerOptions options)
    {
        // convert the operations to a JSON object
        var operations = value.Operations ?? [];
        var node = new JsonObject();

        foreach (var operation in operations)
        {
            var type = operation.OperationType;
            if (type is Operations.OperationType.Add or Operations.OperationType.Replace)
            {
                var segments = operation.path.Trim('/').Split('/');
                JsonPatchMergeDocumentConverterHelper.PopulateJsonObject(node, segments, operation.value, options);
            }
        }

        // write the object
        node.WriteTo(writer, options);
    }
}

public class JsonPatchMergeDocumentConverter<TModel> : JsonConverter<JsonPatchMergeDocument<TModel>> where TModel : class
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(JsonPatchMergeDocument<TModel>);

    /// <inheritdoc/>
    public override JsonPatchMergeDocument<TModel>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;
        if (reader.TokenType is not JsonTokenType.StartObject)
        {
            throw new InvalidOperationException("Only objects are supported");
        }

        var no = new JsonNodeOptions { PropertyNameCaseInsensitive = options.PropertyNameCaseInsensitive, };
        var node = JsonNode.Parse(ref reader, no)!.AsObject();
        var operations = new List<Operations.Operation<TModel>>();
        JsonPatchMergeDocumentConverterHelper.PopulateOperations(operations, node);

        return new JsonPatchMergeDocument<TModel>(operations, options);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, JsonPatchMergeDocument<TModel> value, JsonSerializerOptions options)
    {
        // convert the operations to a JSON object
        var operations = value.Operations ?? [];
        var node = new JsonObject();

        foreach (var operation in operations)
        {
            var type = operation.OperationType;
            var segments = operation.path.Trim('/').Split('/');
            var opvalue = type is Operations.OperationType.Remove ? null : operation.value;
            JsonPatchMergeDocumentConverterHelper.PopulateJsonObject(node, segments, opvalue, options);
        }

        // write the object
        node.WriteTo(writer, options);
    }
}

internal static class JsonPatchMergeDocumentConverterHelper
{
    internal static void PopulateJsonObject(JsonObject node, IReadOnlyList<string> segments, object? value, JsonSerializerOptions options)
    {
        var currentNode = node;
        for (var i = 0; i < segments.Count; i++)
        {
            var segment = segments[i];
            if (i + 1 == segments.Count)
            {
                currentNode[segment] = JsonSerializer.SerializeToNode(value, options);
            }
            else
            {
                // TODO: consider arrays here!!!
                var obj = new JsonObject();
                if (currentNode.TryGetPropertyValue(segment, out var existing)) obj = existing!.AsObject();
                else currentNode[segment] = obj;

                currentNode = obj;
            }
        }
    }

    internal static void PopulateOperations<TOperation>(List<TOperation> operations, JsonNode? node, string key = "") where TOperation : Operations.Operation, new()
    {
        if (node is null && key == "") return;

        if (node is JsonObject jo)
        {
            foreach (var pair in jo)
            {
                var value = pair.Value;
                PopulateOperations(operations, value, key + "/" + pair.Key);
            }
        }
        else if (node is JsonArray ja)
        {
            var index = 0;
            foreach (var element in ja)
            {
                PopulateOperations(operations, element, key + "/" + index);
                index++;
            }
        }
        else if (node is JsonValue jv)
        {
            // convert to JsonElement to avoid type conversions for JsonValueKind.Number
            var value = JsonSerializer.SerializeToElement(jv);
            if (value.ValueKind == JsonValueKind.Null)
            {
                operations.Add(new TOperation { op = "remove", path = key });
            }
            else
            {
                //operations.Add(new TOperation { op = "replace", path = key, value = value });
                operations.Add(new TOperation { op = "add", path = key, value = value });
            }
        }
        else if (node is null)
        {
            operations.Add(new TOperation { op = "remove", path = key });
        }
        else
        {
            throw new InvalidOperationException($"'{node?.GetType()}' types are not allowed here!");
        }
    }
}
