using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications;

[JsonSerializable(typeof(Apple.Models.ApnsMessageData))]
[JsonSerializable(typeof(Apple.Models.ApnsMessageResponse))]
[JsonSerializable(typeof(Apple.Models.ApnsResponseError))]
[JsonSerializable(typeof(Apple.ApnsAuthenticationHandler.ApnsAuthHeader))]
[JsonSerializable(typeof(Apple.ApnsAuthenticationHandler.ApnsAuthPayload))]

[JsonSerializable(typeof(FcmLegacy.Models.FcmLegacyRequest))]
[JsonSerializable(typeof(FcmLegacy.Models.FcmLegacyRequestAndroid))]
[JsonSerializable(typeof(FcmLegacy.Models.FcmLegacyRequestIos))]
[JsonSerializable(typeof(FcmLegacy.Models.FcmLegacyRequestWeb))]
[JsonSerializable(typeof(FcmLegacy.Models.FcmLegacyResponse))]

[JsonSerializable(typeof(Firebase.Models.FirebaseRequest))]
[JsonSerializable(typeof(Firebase.Models.FirebaseResponse))]
[JsonSerializable(typeof(Firebase.Models.FirebaseResponseProblem))]
[JsonSerializable(typeof(Firebase.FirebaseAuthenticationHandler.FirebaseAuthHeader))]
[JsonSerializable(typeof(Firebase.FirebaseAuthenticationHandler.FirebaseAuthPayload))]

[JsonSerializable(typeof(Microsoft.Extensions.DependencyInjection.FirebaseSettings))]
internal partial class PushNotificationsJsonSerializerContext : JsonSerializerContext { }
