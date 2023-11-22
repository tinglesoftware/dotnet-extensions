using System.Text.Json.Serialization;
using Tingle.Extensions.JsonPatch.Converters;
using Tingle.Extensions.JsonPatch.Helpers;
using Tingle.Extensions.JsonPatch.Operations;

namespace Tingle.Extensions.JsonPatch;

[JsonConverter(typeof(JsonPatchDocumentConverter))]
public class JsonPatchDocument : IJsonPatchDocument
{
    public List<Operation> Operations { get; private set; } = [];

    public JsonPatchDocument() { }

    public JsonPatchDocument(List<Operation> operations)
    {
        Operations = operations ?? throw new ArgumentNullException(nameof(operations));
    }

    /// <summary>
    /// Add operation.  Will result in, for example,
    /// { "op": "add", "path": "/a/b/c", "value": [ "foo", "bar" ] }
    /// </summary>
    /// <param name="path">target location</param>
    /// <param name="value">value</param>
    /// <returns></returns>
    public JsonPatchDocument Add(string path, object value)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        Operations.Add(new Operation("add", PathHelpers.NormalizePath(path), null, value));
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
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        Operations.Add(new Operation("remove", PathHelpers.NormalizePath(path), null, null));
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
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        Operations.Add(new Operation("replace", PathHelpers.NormalizePath(path), null, value));
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
        if (from == null)
        {
            throw new ArgumentNullException(nameof(from));
        }

        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        Operations.Add(new Operation("move", PathHelpers.NormalizePath(path), PathHelpers.NormalizePath(from)));
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
        if (from == null)
        {
            throw new ArgumentNullException(nameof(from));
        }

        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        Operations.Add(new Operation("copy", PathHelpers.NormalizePath(path), PathHelpers.NormalizePath(from)));
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
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        Operations.Add(new Operation("test", PathHelpers.NormalizePath(path), null, value));
        return this;
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
