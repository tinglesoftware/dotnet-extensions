using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.AspNetCore.ApplicationInsights;

[JsonSerializable(typeof(IDictionary<string, string[]>))]
[JsonSerializable(typeof(IDictionary<string, object?>))]
// These primitive types are declared for common types that may be used with ProblemDetails.Extensions.
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(byte))]
[JsonSerializable(typeof(sbyte))]
[JsonSerializable(typeof(short))]
[JsonSerializable(typeof(ushort))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(uint))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(ulong))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(char))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(DateTimeOffset))]
[JsonSerializable(typeof(TimeSpan))]
[JsonSerializable(typeof(JsonArray))]
[JsonSerializable(typeof(JsonObject))]
[JsonSerializable(typeof(JsonDocument))]
[JsonSerializable(typeof(DateOnly))]
[JsonSerializable(typeof(TimeOnly))]
#if NET8_0_OR_GREATER
[JsonSerializable(typeof(Half))]
[JsonSerializable(typeof(Int128))]
[JsonSerializable(typeof(UInt128))]
#endif
internal partial class InsightsJsonSerializerContext : JsonSerializerContext { }
