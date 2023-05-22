using System.Text.Json.Serialization;

namespace Tingle.Extensions.JsonPatch.Converters;

[JsonSerializable(typeof(List<Operations.Operation>))]
public partial class JsonPatchSerializerContext : JsonSerializerContext { }
