using System.Text.Json;
using System.Text.Json.Serialization;
using Tingle.AspNetCore.JsonPatch.Adapters;
using Tingle.AspNetCore.JsonPatch.Converters;
using Tingle.AspNetCore.JsonPatch.Operations;

namespace Tingle.AspNetCore.JsonPatch;

[JsonConverter(typeof(JsonMergePatchDocumentConverter))]
public class JsonMergePatchDocument(JsonPatchDocument inner) : IJsonMergePatchDocument
{
    private readonly JsonPatchDocument inner = inner ?? throw new ArgumentNullException(nameof(inner));

    [JsonIgnore]
    public JsonSerializerOptions SerializerOptions { get { return inner.SerializerOptions; } set { inner.SerializerOptions = value; } }

    public JsonMergePatchDocument() : this([]) { }

    public JsonMergePatchDocument(List<Operation> operations) : this(operations, new()) { }

    public JsonMergePatchDocument(JsonSerializerOptions serializerOptions) : this([], serializerOptions) { }

    public JsonMergePatchDocument(List<Operation> operations, JsonSerializerOptions serializerOptions)
        : this(new JsonPatchDocument(operations, serializerOptions)) { }

    internal List<Operation> Operations => inner.Operations;
    IList<Operation> IJsonMergePatchDocument.GetOperations() => ((IJsonPatchDocument)inner).GetOperations();

    /// <summary>
    /// Apply this JsonMergePatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonMergePatchDocument to</param>
    public void ApplyTo(object objectToApplyTo)
    {
        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        ApplyTo(objectToApplyTo, new ObjectAdapter(SerializerOptions, null, AdapterFactory.Default, create: true));
    }

    /// <summary>
    /// Apply this JsonMergePatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonMergePatchDocument to</param>
    /// <param name="logErrorAction">Action to log errors</param>
    public void ApplyTo(object objectToApplyTo, Action<JsonPatchError> logErrorAction)
    {
        ApplyTo(objectToApplyTo, new ObjectAdapter(SerializerOptions, logErrorAction, AdapterFactory.Default, create: true), logErrorAction);
    }

    /// <summary>
    /// Apply this JsonMergePatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonMergePatchDocument to</param>
    /// <param name="adapter">IObjectAdapter instance to use when applying</param>
    /// <param name="logErrorAction">Action to log errors</param>
    public void ApplyTo(object objectToApplyTo, IObjectAdapter adapter, Action<JsonPatchError> logErrorAction) => inner.ApplyTo(objectToApplyTo, adapter, logErrorAction);

    /// <summary>
    /// Apply this JsonMergePatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonMergePatchDocument to</param>
    /// <param name="adapter">IObjectAdapter instance to use when applying</param>
    public void ApplyTo(object objectToApplyTo, IObjectAdapter adapter) => inner.ApplyTo(objectToApplyTo, adapter);
}
