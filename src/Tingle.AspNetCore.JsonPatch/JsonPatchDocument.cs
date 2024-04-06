using System.Text.Json;
using System.Text.Json.Serialization;
using Tingle.AspNetCore.JsonPatch.Adapters;
using Tingle.AspNetCore.JsonPatch.Converters;
using Tingle.AspNetCore.JsonPatch.Exceptions;
using Tingle.AspNetCore.JsonPatch.Helpers;
using Tingle.AspNetCore.JsonPatch.Internal;
using Tingle.AspNetCore.JsonPatch.Operations;

namespace Tingle.AspNetCore.JsonPatch;

[JsonConverter(typeof(JsonPatchDocumentConverter))]
public class JsonPatchDocument(List<Operation> operations, JsonSerializerOptions serializerOptions) : IJsonPatchDocument
{
    public List<Operation> Operations { get; private set; } = operations ?? throw new ArgumentNullException(nameof(operations));

    [JsonIgnore]
    public JsonSerializerOptions SerializerOptions { get; set; } = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));

    public JsonPatchDocument() : this([]) { }

    public JsonPatchDocument(List<Operation> operations) : this(operations, new()) { }

    public JsonPatchDocument(JsonSerializerOptions serializerOptions) : this([], serializerOptions) { }

    /// <summary>
    /// Add operation.  Will result in, for example,
    /// { "op": "add", "path": "/a/b/c", "value": [ "foo", "bar" ] }
    /// </summary>
    /// <param name="path">target location</param>
    /// <param name="value">value</param>
    /// <returns></returns>
    public JsonPatchDocument Add(string path, object value)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation("add", PathHelpers.ValidateAndNormalizePath(path), null, value));
        return this;
    }

    /// <summary>
    /// Remove value at target location.  Will result in, for example,
    /// { "op": "remove", "path": "/a/b/c" }
    /// </summary>
    /// <param name="path">target location</param>
    /// <returns></returns>
    public JsonPatchDocument Remove(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation("remove", PathHelpers.ValidateAndNormalizePath(path), null, null));
        return this;
    }

    /// <summary>
    /// Replace value.  Will result in, for example,
    /// { "op": "replace", "path": "/a/b/c", "value": 42 }
    /// </summary>
    /// <param name="path">target location</param>
    /// <param name="value">value</param>
    /// <returns></returns>
    public JsonPatchDocument Replace(string path, object value)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation("replace", PathHelpers.ValidateAndNormalizePath(path), null, value));
        return this;
    }

    /// <summary>
    /// Test value.  Will result in, for example,
    /// { "op": "test", "path": "/a/b/c", "value": 42 }
    /// </summary>
    /// <param name="path">target location</param>
    /// <param name="value">value</param>
    /// <returns></returns>
    public JsonPatchDocument Test(string path, object value)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation("test", PathHelpers.ValidateAndNormalizePath(path), null, value));
        return this;
    }

    /// <summary>
    /// Removes value at specified location and add it to the target location.  Will result in, for example:
    /// { "op": "move", "from": "/a/b/c", "path": "/a/b/d" }
    /// </summary>
    /// <param name="from">source location</param>
    /// <param name="path">target location</param>
    /// <returns></returns>
    public JsonPatchDocument Move(string from, string path)
    {
        ArgumentNullException.ThrowIfNull(from);

        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation("move", PathHelpers.ValidateAndNormalizePath(path), PathHelpers.ValidateAndNormalizePath(from)));
        return this;
    }

    /// <summary>
    /// Copy the value at specified location to the target location.  Will result in, for example:
    /// { "op": "copy", "from": "/a/b/c", "path": "/a/b/e" }
    /// </summary>
    /// <param name="from">source location</param>
    /// <param name="path">target location</param>
    /// <returns></returns>
    public JsonPatchDocument Copy(string from, string path)
    {
        ArgumentNullException.ThrowIfNull(from);

        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation("copy", PathHelpers.ValidateAndNormalizePath(path), PathHelpers.ValidateAndNormalizePath(from)));
        return this;
    }

    /// <summary>
    /// Apply this JsonPatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
    public void ApplyTo(object objectToApplyTo)
    {
        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        ApplyTo(objectToApplyTo, new ObjectAdapter(SerializerOptions, null, AdapterFactory.Default, create: false));
    }

    /// <summary>
    /// Apply this JsonPatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
    /// <param name="logErrorAction">Action to log errors</param>
    public void ApplyTo(object objectToApplyTo, Action<JsonPatchError> logErrorAction)
    {
        ApplyTo(objectToApplyTo, new ObjectAdapter(SerializerOptions, logErrorAction, AdapterFactory.Default, create: false), logErrorAction);
    }

    /// <summary>
    /// Apply this JsonPatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
    /// <param name="adapter">IObjectAdapter instance to use when applying</param>
    /// <param name="logErrorAction">Action to log errors</param>
    public void ApplyTo(object objectToApplyTo, IObjectAdapter adapter, Action<JsonPatchError> logErrorAction)
    {
        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        ArgumentNullException.ThrowIfNull(adapter);

        foreach (var op in Operations)
        {
            try
            {
                op.Apply(objectToApplyTo, adapter);
            }
            catch (JsonPatchException jsonPatchException)
            {
                var errorReporter = logErrorAction ?? ErrorReporter.Default;
                errorReporter(new JsonPatchError(objectToApplyTo, op, jsonPatchException.Message));

                // As per JSON Patch spec if an operation results in error, further operations should not be executed.
                break;
            }
        }
    }

    /// <summary>
    /// Apply this JsonPatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
    /// <param name="adapter">IObjectAdapter instance to use when applying</param>
    public void ApplyTo(object objectToApplyTo, IObjectAdapter adapter)
    {
        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        ArgumentNullException.ThrowIfNull(adapter);

        // apply each operation in order
        foreach (var op in Operations)
        {
            op.Apply(objectToApplyTo, adapter);
        }
    }

    IList<Operation> IJsonPatchDocument.GetOperations()
    {
        var allOps = new List<Operation>();

        if (Operations != null)
        {
            foreach (var op in Operations)
            {
                var untypedOp = new Operation
                {
                    op = op.op,
                    value = op.value,
                    path = op.path,
                    from = op.from
                };

                allOps.Add(untypedOp);
            }
        }

        return allOps;
    }
}
