using System.Text.Json.Serialization;

namespace Tingle.Extensions.Http;

[JsonSerializable(typeof(ResourceResponseHeaders))]
[JsonSerializable(typeof(HttpApiResponseProblem))]
internal partial class HttpJsonSerializerContext : JsonSerializerContext { }
