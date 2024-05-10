using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;

namespace System;

/// <summary>Extensions for Enums.</summary>
public static class EnumExtensions
{
    private const DynamicallyAccessedMemberTypes MembersTypesForEnums =
        DynamicallyAccessedMemberTypes.PublicFields |
        DynamicallyAccessedMemberTypes.PublicMethods |
        DynamicallyAccessedMemberTypes.PublicEvents |
        DynamicallyAccessedMemberTypes.PublicProperties |
        DynamicallyAccessedMemberTypes.PublicConstructors |
        DynamicallyAccessedMemberTypes.PublicNestedTypes;

    /// <summary>Gets the value declared on the member using <see cref="EnumMemberAttribute"/>.</summary>
    /// <param name="type">The <see cref="Type"/> of the enum.</param>
    /// <param name="value">The value of the enum member/field.</param>
    /// <returns></returns>
    public static string? GetEnumMemberAttrValue([DynamicallyAccessedMembers(MembersTypesForEnums)] this Type type, object value)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (!type.IsEnum) throw new ArgumentException("Only enum types are allowed.", nameof(type));

        var mi = type.GetMember(value.ToString()!);
        var attr = mi.FirstOrDefault()?.GetCustomAttribute<EnumMemberAttribute>(inherit: false);

        return attr?.Value;
    }

    /// <summary>Gets the value declared on the member using <see cref="EnumMemberAttribute"/> or the default.</summary>
    /// <param name="type">The <see cref="Type"/> of the enum.</param>
    /// <param name="value">The value of the enum member/field.</param>
    /// <returns></returns>
    public static string GetEnumMemberAttrValueOrDefault([DynamicallyAccessedMembers(MembersTypesForEnums)] this Type type, object value)
    {
        return type.GetEnumMemberAttrValue(value) ?? value.ToString()!.ToLowerInvariant();
    }

    /// <summary>Gets the value declared on the <see cref="Enum"/> using <see cref="EnumMemberAttribute"/>.</summary>
    /// <param name="value">The value of the enum member/field.</param>
    public static string? GetEnumMemberAttrValue(this Enum value) => GetEnumMemberAttrValue(value.GetType(), value);

    /// <summary>Gets the value declared on the <see cref="Enum"/> using <see cref="EnumMemberAttribute"/> or the default.</summary>
    /// <param name="value">The value of the enum member/field.</param>
    public static string GetEnumMemberAttrValueOrDefault(this Enum value) => GetEnumMemberAttrValueOrDefault(value.GetType(), value);

    /// <summary>Gets the value declared on the member using <see cref="EnumMemberAttribute"/>.</summary>
    /// <typeparam name="T">The <see cref="Type"/> of the enum.</typeparam>
    /// <param name="value">The value of the enum member/field.</param>
    public static string? GetEnumMemberAttrValue<[DynamicallyAccessedMembers(MembersTypesForEnums)] T>(this T value) where T : struct, Enum => GetEnumMemberAttrValue(typeof(T), value);

    /// <summary>Gets the value declared on the member using <see cref="EnumMemberAttribute"/> or the default.</summary>
    /// <typeparam name="T">The <see cref="Type"/> of the enum.</typeparam>
    /// <param name="value">The value of the enum member/field.</param>
    public static string GetEnumMemberAttrValueOrDefault<[DynamicallyAccessedMembers(MembersTypesForEnums)] T>(this T value) where T : struct, Enum => GetEnumMemberAttrValueOrDefault(typeof(T), value);
}
