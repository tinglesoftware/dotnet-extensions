using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Tingle.Extensions.EntityFrameworkCore;
using Tingle.Extensions.EntityFrameworkCore.Converters;
using Tingle.Extensions.Primitives;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
/// Extensions for <see cref="PropertyBuilder{TProperty}"/>.
/// </summary>
public static class PropertyBuilderExtensions
{
    /// <summary>
    /// Attach conversion of property to/from <see cref="Etag"/> stored in the database as a <see cref="T:byte[]"/>.
    /// </summary>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/> to extend.</param>
    /// <returns></returns>
    public static PropertyBuilder<Etag> HasEtagToBytesConversion(this PropertyBuilder<Etag> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new EtagToBytesConverter());
        propertyBuilder.Metadata.SetValueComparer(new EtagComparer());

        return propertyBuilder;
    }

    /// <summary>
    /// Attach conversion of property to/from <see cref="Etag"/> stored in the database as a <see cref="int"/>.
    /// </summary>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/> to extend.</param>
    /// <returns></returns>
    public static PropertyBuilder<Etag> HasEtagToInt32Conversion(this PropertyBuilder<Etag> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new EtagToInt32Converter());
        propertyBuilder.Metadata.SetValueComparer(new EtagComparer());

        return propertyBuilder;
    }

    /// <summary>
    /// Attach conversion of property to/from <see cref="Etag"/> stored in the database as a <see cref="uint"/>.
    /// </summary>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/> to extend.</param>
    /// <returns></returns>
    public static PropertyBuilder<Etag> HasEtagToUInt32Conversion(this PropertyBuilder<Etag> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new EtagToUInt32Converter());
        propertyBuilder.Metadata.SetValueComparer(new EtagComparer());

        return propertyBuilder;
    }

    /// <summary>
    /// Attach conversion of property to/from <see cref="Etag"/> stored in the database as a <see cref="long"/>.
    /// </summary>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/> to extend.</param>
    /// <returns></returns>
    public static PropertyBuilder<Etag> HasEtagToInt64Conversion(this PropertyBuilder<Etag> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new EtagToInt64Converter());
        propertyBuilder.Metadata.SetValueComparer(new EtagComparer());

        return propertyBuilder;
    }

    /// <summary>
    /// Attach conversion of property to/from <see cref="Etag"/> stored in the database as a <see cref="ulong"/>.
    /// </summary>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/> to extend.</param>
    /// <returns></returns>
    public static PropertyBuilder<Etag> HasEtagToUInt64Conversion(this PropertyBuilder<Etag> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new EtagToUInt64Converter());
        propertyBuilder.Metadata.SetValueComparer(new EtagComparer());

        return propertyBuilder;
    }

    /// <summary>
    /// Attach conversion of property to/from <see cref="SequenceNumber"/> stored in the database as a <see cref="long"/>.
    /// </summary>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/> to extend.</param>
    /// <returns></returns>
    public static PropertyBuilder<SequenceNumber> HasSequenceNumberConversion(this PropertyBuilder<SequenceNumber> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new SequenceNumberConverter());
        propertyBuilder.Metadata.SetValueComparer(new SequenceNumberComparer());

        return propertyBuilder;
    }

    /// <summary>
    /// Attach conversion of property to/from <see cref="ByteSize"/> stored in the database as a <see cref="long"/>.
    /// </summary>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/> to extend.</param>
    /// <returns></returns>
    public static PropertyBuilder<ByteSize> HasByteSizeConversion(this PropertyBuilder<ByteSize> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new ByteSizeConverter());
        propertyBuilder.Metadata.SetValueComparer(new ByteSizeComparer());

        return propertyBuilder;
    }

    /// <summary>
    /// Attach conversion of property to/from <see cref="Duration"/> stored in the database as a <see cref="string"/>.
    /// </summary>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/> to extend.</param>
    /// <returns></returns>
    public static PropertyBuilder<Duration> HasDurationConversion(this PropertyBuilder<Duration> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new DurationConverter());
        propertyBuilder.Metadata.SetValueComparer(new DurationComparer());

        return propertyBuilder;
    }

    /// <summary>
    /// Attach conversion of property to/from <see cref="IPNetwork"/> stored in the database as a <see cref="string"/>.
    /// </summary>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/> to extend.</param>
    /// <returns></returns>
    public static PropertyBuilder<IPNetwork> HasIPNetworkConversion(this PropertyBuilder<IPNetwork> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new IPNetworkConverter());
        propertyBuilder.Metadata.SetValueComparer(new IPNetworkComparer());

        return propertyBuilder;
    }

    /// <summary>
    /// Attach conversion of property to/from <see cref="JsonElement"/> stored in the database as a <see cref="string"/>.
    /// </summary>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/> to extend.</param>
    /// <returns></returns>
    public static PropertyBuilder<JsonElement> HasJsonElementConversion(this PropertyBuilder<JsonElement> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new JsonElementConverter());
        propertyBuilder.Metadata.SetValueComparer(new JsonElementComparer());

        return propertyBuilder;
    }

    /// <summary>
    /// Attach conversion of property to/from <see cref="JsonObject"/> stored in the database as a <see cref="string"/>.
    /// </summary>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/> to extend.</param>
    /// <returns></returns>
    [RequiresDynamicCode(MessageStrings.JsonComparisonRequiresDynamicCodeMessage)]
    [RequiresUnreferencedCode(MessageStrings.JsonComparisonRequiresDynamicCodeMessage)]
    public static PropertyBuilder<JsonObject> HasJsonObjectConversion(this PropertyBuilder<JsonObject> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new JsonObjectConverter());
        propertyBuilder.Metadata.SetValueComparer(new JsonObjectComparer());

        return propertyBuilder;
    }

    /// <summary>
    /// Attach conversion of property to/from <see cref="JsonNode"/> stored in the database as a <see cref="string"/>.
    /// </summary>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/> to extend.</param>
    /// <returns></returns>
    [RequiresDynamicCode(MessageStrings.JsonComparisonRequiresDynamicCodeMessage)]
    [RequiresUnreferencedCode(MessageStrings.JsonComparisonRequiresDynamicCodeMessage)]
    public static PropertyBuilder<JsonNode> HasJsonNodeConversion(this PropertyBuilder<JsonNode> propertyBuilder)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

        propertyBuilder.HasConversion(new JsonNodeConverter());
        propertyBuilder.Metadata.SetValueComparer(new JsonNodeComparer());

        return propertyBuilder;
    }

    /// <summary>
    /// Attach conversion of property to/from JSON stored in the database as a string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/> to extend.</param>
    /// <param name="serializerOptions">The <see cref="JsonSerializerOptions"/> to use.</param>
    /// <returns></returns>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder, JsonSerializerOptions? serializerOptions = null)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

#pragma warning disable CS8603 // Possible null reference return.
        var converter = new ValueConverter<T, string?>(
            convertToProviderExpression: v => ConvertToJson(v, serializerOptions),
            convertFromProviderExpression: v => ConvertFromJson<T>(v, serializerOptions));

        var comparer = new ValueComparer<T>(
            equalsExpression: (l, r) => ConvertToJson(l, serializerOptions) == ConvertToJson(r, serializerOptions),
            hashCodeExpression: v => v == null ? 0 : ConvertToJson(v, serializerOptions).GetHashCode(),
            snapshotExpression: v => ConvertFromJson<T>(ConvertToJson(v, serializerOptions), serializerOptions));
#pragma warning restore CS8603 // Possible null reference return.

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);

        return propertyBuilder;
    }

    /// <summary>
    /// Attach conversion of property to/from JSON stored in the database as a string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyBuilder">The <see cref="PropertyBuilder{TProperty}"/> to extend.</param>
    /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
    /// <returns></returns>
    public static PropertyBuilder<T> HasJsonConversion<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods
        | DynamicallyAccessedMemberTypes.NonPublicMethods
        | DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        this PropertyBuilder<T> propertyBuilder, JsonTypeInfo<T> jsonTypeInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyBuilder);

#pragma warning disable CS8603 // Possible null reference return.
        var converter = new ValueConverter<T, string?>(
            convertToProviderExpression: v => ConvertToJson(v, jsonTypeInfo),
            convertFromProviderExpression: v => ConvertFromJson(v, jsonTypeInfo));

        var comparer = new ValueComparer<T>(
            equalsExpression: (l, r) => ConvertToJsonNullable(l, jsonTypeInfo) == ConvertToJsonNullable(r, jsonTypeInfo),
            hashCodeExpression: v => v == null ? 0 : ConvertToJson(v, jsonTypeInfo).GetHashCode(),
            snapshotExpression: v => ConvertFromJson(ConvertToJson(v, jsonTypeInfo), jsonTypeInfo));
#pragma warning restore CS8603 // Possible null reference return.

        propertyBuilder.HasConversion(converter);
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);

        return propertyBuilder;
    }

    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    private static string ConvertToJson<T>(T value, JsonSerializerOptions? serializerOptions) => JsonSerializer.Serialize(value, serializerOptions);

    private static string ConvertToJson<T>(T value, JsonTypeInfo<T> jsonTypeInfo) => JsonSerializer.Serialize(value, jsonTypeInfo);

    private static string? ConvertToJsonNullable<T>(T? value, JsonTypeInfo<T> jsonTypeInfo) => value is null ? null : ConvertToJson(value, jsonTypeInfo);

    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    private static T? ConvertFromJson<T>(string? value, JsonSerializerOptions? serializerOptions) => string.IsNullOrEmpty(value) ? default : JsonSerializer.Deserialize<T>(value, serializerOptions);

    private static T? ConvertFromJson<T>(string? value, JsonTypeInfo<T> jsonTypeInfo) => string.IsNullOrEmpty(value) ? default : JsonSerializer.Deserialize(value, jsonTypeInfo);
}
