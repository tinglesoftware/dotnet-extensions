namespace Tingle.Extensions.EntityFrameworkCore;

internal class MessageStrings
{
    public const string SerializationUnreferencedCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.";
    public const string SerializationRequiresDynamicCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.";

    public const string MigrationsRequiresDynamicCodeMessage = "Migration operations are not supported with Native AOT. Use a migration bundle or an alternate way of executing migration operations.";
    public const string JsonComparisonRequiresDynamicCodeMessage = "JSON comparison might require types that cannot be statically analyzed";
}
