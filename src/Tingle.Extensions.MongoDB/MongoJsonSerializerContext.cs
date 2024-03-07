using System.Text.Json.Serialization;
using System.Text.Json;

namespace Tingle.Extensions.MongoDB;

[JsonSerializable(typeof(JsonElement))]
internal partial class MongoJsonSerializerContext : JsonSerializerContext { }
