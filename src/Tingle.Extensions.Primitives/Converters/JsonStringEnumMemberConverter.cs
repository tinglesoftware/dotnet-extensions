using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Converters;

/// <summary>
/// <see cref="JsonConverterFactory"/> to convert enums to and from strings, respecting <see cref="EnumMemberAttribute"/> decorations. Supports nullable enums.
/// </summary>
/// <typeparam name="TEnum">The enum type that this converter targets.</typeparam>
/// <param name="namingPolicy">Optional naming policy for writing enum values.</param>
/// <param name="allowIntegerValues">
/// True to allow undefined enum values. When true, if an enum value isn't
/// defined it will output as a number rather than a string.
/// </param>
public class JsonStringEnumMemberConverter<[DynamicallyAccessedMembers(EnumConverterHelper.RequiredMembersTypes)] TEnum>(JsonNamingPolicy? namingPolicy = null, bool allowIntegerValues = true)
    : JsonConverterFactory where TEnum : struct, Enum
{
    private readonly JsonNamingPolicy? namingPolicy = namingPolicy;
    private readonly bool allowIntegerValues = allowIntegerValues;

    /// <summary>Initializes a new instance of the <see cref="JsonStringEnumMemberConverter{TEnum}"/> class.</summary>
    public JsonStringEnumMemberConverter() : this(null, true) { }

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(TEnum);

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => new EnumMemberConverter<TEnum>(namingPolicy, allowIntegerValues);
}

/// <summary>
/// <see cref="JsonConverterFactory"/> to convert enums to and from strings, respecting <see cref="EnumMemberAttribute"/> decorations. Supports nullable enums.
/// </summary>
/// <param name="namingPolicy">Optional naming policy for writing enum values.</param>
/// <param name="allowIntegerValues">
/// True to allow undefined enum values. When true, if an enum value isn't
/// defined it will output as a number rather than a string.
/// </param>
[RequiresDynamicCode(
    "JsonStringEnumMemberConverter cannot be statically analyzed and requires runtime code generation. " +
    "Applications should use the generic JsonStringEnumMemberConverter<TEnum> instead")]
public class JsonStringEnumMemberConverter(JsonNamingPolicy? namingPolicy = null, bool allowIntegerValues = true) : JsonConverterFactory
{
    private readonly JsonNamingPolicy? namingPolicy = namingPolicy;
    private readonly bool allowIntegerValues = allowIntegerValues;

    /// <summary>Initializes a new instance of the <see cref="JsonStringEnumMemberConverter"/> class.</summary>
    public JsonStringEnumMemberConverter() : this(null, true) { }

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
#pragma warning disable IL2070 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.
        return (JsonConverter?)Activator.CreateInstance(
                typeof(EnumMemberConverter<>).MakeGenericType(typeToConvert),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: [namingPolicy, allowIntegerValues],
                culture: null);
#pragma warning restore IL2070 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.
    }
}

internal class EnumMemberConverter<[DynamicallyAccessedMembers(EnumConverterHelper.RequiredMembersTypes)] TEnum>(JsonNamingPolicy? namingPolicy, bool allowIntegerValues)
    : JsonConverter<TEnum> where TEnum : struct, Enum
{
    private readonly EnumConverterHelper<TEnum> helper = new(namingPolicy, allowIntegerValues);

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => helper.Read(ref reader);

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        => helper.Write(writer, value);

    public override TEnum ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => helper.Read(ref reader);

    public override void WriteAsPropertyName(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        => helper.WritePropertyName(writer, value);
}
