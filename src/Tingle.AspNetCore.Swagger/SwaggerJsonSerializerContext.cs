using System.Text.Json.Serialization;
using Tingle.AspNetCore.Swagger.ReDoc;

namespace Tingle.AspNetCore.Swagger;

[JsonSerializable(typeof(ReDocConfig))]
internal partial class SwaggerJsonSerializerContext : JsonSerializerContext { }
