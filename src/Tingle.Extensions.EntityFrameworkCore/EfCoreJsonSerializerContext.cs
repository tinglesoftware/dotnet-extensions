using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.EntityFrameworkCore;

[JsonSerializable(typeof(JsonElement))]
internal partial class EfCoreJsonSerializerContext : JsonSerializerContext { }
