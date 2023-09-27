using System.Text.Json.Serialization;

namespace Tingle.AspNetCore.Authentication;

[JsonSerializable(typeof(AddressClaim))]
internal partial class AuthenticationJsonSerializerContext : JsonSerializerContext { }
