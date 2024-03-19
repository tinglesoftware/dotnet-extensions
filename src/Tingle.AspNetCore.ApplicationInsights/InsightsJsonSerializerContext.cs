using System.Text.Json.Serialization;

namespace Tingle.AspNetCore.ApplicationInsights;

[JsonSerializable(typeof(IDictionary<string, string[]>))]
[JsonSerializable(typeof(IDictionary<string, object?>))]
internal partial class InsightsJsonSerializerContext : JsonSerializerContext { }
