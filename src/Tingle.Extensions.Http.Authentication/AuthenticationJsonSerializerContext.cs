using System.Text.Json.Serialization;

namespace Tingle.Extensions.Http.Authentication;

[JsonSerializable(typeof(OAuthClientCredentialHandler.OAuthTokenResponse))]
internal partial class AuthenticationJsonSerializerContext : JsonSerializerContext { }
