namespace Tingle.AspNetCore.Tokens;

internal class MessageStrings
{
    //internal const string SerializationUnreferencedCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.";
    //internal const string SerializationRequiresDynamicCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.";

    internal const string TokenProtectorUnreferencedCodeMessage = "JSON serialization/deserialization and generic TypeConverters might require types that cannot be statically analyzed.";
    internal const string TokenProtectorRequiresDynamicCodeMessage = "JSON serialization/deserialization and generic TypeConverters might require types that cannot be statically analyzed.";
}
