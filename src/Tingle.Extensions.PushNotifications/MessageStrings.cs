namespace Tingle.Extensions.PushNotifications;

internal class MessageStrings
{
    internal const string SerializationUnreferencedCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.";
    internal const string SerializationRequiresDynamicCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.";

    internal const string FirebaseLegacyObsoleteMessage = "FCM legacy APIs for HTTP and XMPP protocols are deprecated and will be removed in the future. Migrate to the HTTP v1 API (Tingle.Extensions.PushNotifications.Firebase) before June 2024 to ensure uninterrupted service. See https://firebase.google.com/docs/cloud-messaging/migrate-v1 for more details.";
}
