namespace Tingle.AspNetCore.Tokens;

internal class MessageStrings
{
    public const string SerializationUnreferencedCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.";
    public const string SerializationRequiresDynamicCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.";

    public const string ModelBindingGenericsRequiresDynamicCodeMessage = "Model binding for generic types requires dynamic code generation which is not support for native AOT applications.";
}
