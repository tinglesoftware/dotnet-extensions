using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Adapters;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using Tingle.AspNetCore.JsonPatch.Converters;

namespace Tingle.AspNetCore.JsonPatch;

[JsonConverter(typeof(TypedJsonMergePatchDocumentConverter))]
public class JsonMergePatchDocument<TModel>(JsonPatchDocument<TModel> inner) : IJsonMergePatchDocument where TModel : class
{
    private readonly JsonPatchDocument<TModel> inner = inner ?? throw new ArgumentNullException(nameof(inner));

    [JsonIgnore]
    public JsonSerializerOptions SerializerOptions { get { return inner.SerializerOptions; } set { inner.SerializerOptions = value; } }

    public JsonMergePatchDocument() : this([]) { }

    public JsonMergePatchDocument(List<Operation<TModel>> operations) : this(operations, new()) { }

    public JsonMergePatchDocument(JsonSerializerOptions serializerOptions) : this([], serializerOptions) { }

    public JsonMergePatchDocument(List<Operation<TModel>> operations, JsonSerializerOptions serializerOptions)
        : this(new JsonPatchDocument<TModel>(operations, serializerOptions)) { }

    internal List<Operation<TModel>> Operations => inner.Operations;
    IList<Operation> IJsonMergePatchDocument.GetOperations() => ((IJsonPatchDocument)inner).GetOperations();

    /// <summary>
    /// Apply this JsonMergePatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonMergePatchDocument to</param>
    public void ApplyTo(TModel objectToApplyTo)
    {
        JsonMergePatchApplier.Apply(this, objectToApplyTo, SerializerOptions);
    }

    /// <summary>
    /// Apply this JsonMergePatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonMergePatchDocument to</param>
    /// <param name="logErrorAction">Action to log errors</param>
    public void ApplyTo(TModel objectToApplyTo, Action<JsonPatchError> logErrorAction)
    {
        JsonMergePatchApplier.Apply(this, objectToApplyTo, SerializerOptions, logErrorAction);
    }

    /// <summary>
    /// Apply this JsonMergePatchDocument.
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonMergePatchDocument to</param>
    /// <param name="create">Ignored. The merge patch implementation always creates missing object members when needed.</param>
    [Obsolete("The create parameter is ignored. Use ApplyTo(objectToApplyTo).", false)]
    public void ApplyTo(TModel objectToApplyTo, bool create) => JsonMergePatchApplier.Apply(this, objectToApplyTo, SerializerOptions);

    /// <summary>
    /// Apply this JsonMergePatchDocument.
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonMergePatchDocument to</param>
    /// <param name="logErrorAction">Action to log errors</param>
    /// <param name="create">Ignored. The merge patch implementation always creates missing object members when needed.</param>
    [Obsolete("The create parameter is ignored. Use ApplyTo(objectToApplyTo, logErrorAction).", false)]
    public void ApplyTo(TModel objectToApplyTo, Action<JsonPatchError> logErrorAction, bool create)
        => JsonMergePatchApplier.Apply(this, objectToApplyTo, SerializerOptions, logErrorAction);

    /// <summary>
    /// Apply this JsonMergePatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonMergePatchDocument to</param>
    /// <param name="adapter">IObjectAdapter instance to use when applying</param>
    /// <param name="logErrorAction">Action to log errors</param>
    public void ApplyTo(TModel objectToApplyTo, IObjectAdapter adapter, Action<JsonPatchError> logErrorAction) => inner.ApplyTo(objectToApplyTo, adapter, logErrorAction);

    /// <summary>
    /// Apply this JsonMergePatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonMergePatchDocument to</param>
    /// <param name="adapter">IObjectAdapter instance to use when applying</param>
    public void ApplyTo(TModel objectToApplyTo, IObjectAdapter adapter) => inner.ApplyTo(objectToApplyTo, adapter);
}
