using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.Primitives.Converters;

internal static class EnumConverterHelper
{
    internal const DynamicallyAccessedMemberTypes RequiredMembersTypes =
        DynamicallyAccessedMemberTypes.PublicFields |
        DynamicallyAccessedMemberTypes.NonPublicFields;
}

internal class EnumConverterHelper<[DynamicallyAccessedMembers(EnumConverterHelper.RequiredMembersTypes)] TEnum> where TEnum : struct, Enum
{
    private readonly struct EnumInfo(string name, TEnum value, ulong raw, string preferred)
    {
        public string Name { get; } = name;
        public TEnum Value { get; } = value;
        public ulong Raw { get; } = raw;

        public string Preferred { get; } = preferred;

        public override string ToString() => $"{Preferred} ({Value})";
    }

    private readonly bool allowIntegerValues;
    [DynamicallyAccessedMembers(EnumConverterHelper.RequiredMembersTypes)]
    private readonly Type type = typeof(TEnum);
    private readonly TypeCode enumTypeCode;
    private readonly bool isFlags;
    private readonly Dictionary<TEnum, EnumInfo> mapping;
    private readonly Dictionary<string, EnumInfo> lookup;

    public EnumConverterHelper(JsonNamingPolicy? namingPolicy, bool allowIntegerValues)
    {
        this.allowIntegerValues = allowIntegerValues;
        enumTypeCode = Type.GetTypeCode(type);
        isFlags = type.IsDefined(typeof(FlagsAttribute), true);

        var names = type.GetEnumNames();
        var builtInValues = type.GetEnumValues();

        int numberOfNames = names.Length;

        mapping = new Dictionary<TEnum, EnumInfo>(numberOfNames);
        lookup = new Dictionary<string, EnumInfo>(numberOfNames, StringComparer.OrdinalIgnoreCase);

        var bindings = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
        for (int i = 0; i < numberOfNames; i++)
        {
            var value = (Enum?)builtInValues.GetValue(i);
            if (value is null) continue;

            var raw = GetEnumValue(enumTypeCode, value);

            var name = names[i];
            var field = type.GetField(name, bindings)!;

            TryGetCustomAttribute<EnumMemberAttribute>(field, true, out var ema);
            TryGetCustomAttribute<JsonPropertyNameAttribute>(field, true, out var jpna);
            var chosen = ema?.Value ?? jpna?.Name ?? namingPolicy?.ConvertName(name) ?? name;

            if (value is not TEnum typed)
                throw new NotSupportedException();

            var info = new EnumInfo(name, typed, raw, chosen);
            mapping[typed] = info;

            if (ema?.Value is not null) lookup[ema.Value] = info;
            if (jpna?.Name is not null) lookup[jpna.Name] = info;
            if (namingPolicy is not null) lookup[namingPolicy.ConvertName(name)] = info;
            lookup[name] = info;
            lookup[raw.ToString()] = info;
        }
    }

    public TEnum Read(ref Utf8JsonReader reader)
    {
        JsonTokenType token = reader.TokenType;

        if (token is JsonTokenType.String or JsonTokenType.PropertyName)
        {
            string enumString = reader.GetString()!;

            if (lookup.TryGetValue(enumString, out EnumInfo enumInfo))
                return enumInfo.Value;

            if (isFlags)
            {
                var calculatedValue = 0UL;

                var flagValues = enumString.Split(", ");
                foreach (var flagValue in flagValues)
                {
                    // Case sensitive search attempted first.
                    if (lookup.TryGetValue(flagValue, out enumInfo))
                    {
                        calculatedValue |= enumInfo.Raw;
                    }
                    else
                    {
                        throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(type, flagValue);
                    }
                }

                return (TEnum)Enum.ToObject(type, calculatedValue);
            }

            throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(type, enumString);
        }

        if (token != JsonTokenType.Number || !allowIntegerValues)
        {
            throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(type);
        }

        switch (enumTypeCode)
        {
            case TypeCode.Int32:
                if (reader.TryGetInt32(out int int32))
                {
                    return (TEnum)Enum.ToObject(type, int32);
                }
                break;
            case TypeCode.Int64:
                if (reader.TryGetInt64(out long int64))
                {
                    return (TEnum)Enum.ToObject(type, int64);
                }
                break;
            case TypeCode.Int16:
                if (reader.TryGetInt16(out short int16))
                {
                    return (TEnum)Enum.ToObject(type, int16);
                }
                break;
            case TypeCode.Byte:
                if (reader.TryGetByte(out byte ubyte8))
                {
                    return (TEnum)Enum.ToObject(type, ubyte8);
                }
                break;
            case TypeCode.UInt32:
                if (reader.TryGetUInt32(out uint uint32))
                {
                    return (TEnum)Enum.ToObject(type, uint32);
                }
                break;
            case TypeCode.UInt64:
                if (reader.TryGetUInt64(out ulong uint64))
                {
                    return (TEnum)Enum.ToObject(type, uint64);
                }
                break;
            case TypeCode.UInt16:
                if (reader.TryGetUInt16(out ushort uint16))
                {
                    return (TEnum)Enum.ToObject(type, uint16);
                }
                break;
            case TypeCode.SByte:
                if (reader.TryGetSByte(out sbyte byte8))
                {
                    return (TEnum)Enum.ToObject(type, byte8);
                }
                break;
        }

        throw ThrowHelper.GenerateJsonException_DeserializeUnableToConvertValue(type);
    }

    public void Write(Utf8JsonWriter writer, TEnum value)
    {
        if (mapping.TryGetValue(value, out EnumInfo enumInfo))
        {
            writer.WriteStringValue(enumInfo.Preferred);
            return;
        }

        ulong rawValue = GetEnumValue(enumTypeCode, value);

        if (isFlags)
        {
            ulong calculatedValue = 0;

            var builder = new StringBuilder();
            foreach (var kvp in mapping)
            {
                enumInfo = kvp.Value;
                if (!value.HasFlag(enumInfo.Value)) continue;

                // Track the value to make sure all bits are represented.
                calculatedValue |= enumInfo.Raw;

                if (builder.Length > 0) builder.Append(", ");
                builder.Append(enumInfo.Preferred);
            }
            if (calculatedValue == rawValue)
            {
                string finalName = builder.ToString();
                writer.WriteStringValue(finalName);
                return;
            }
        }

        if (!allowIntegerValues)
            throw new JsonException($"Enum type {type} does not have a mapping for integer value '{rawValue.ToString(CultureInfo.CurrentCulture)}'.");

        switch (enumTypeCode)
        {
            case TypeCode.Int32:
                writer.WriteNumberValue((int)rawValue);
                break;
            case TypeCode.Int64:
                writer.WriteNumberValue((long)rawValue);
                break;
            case TypeCode.Int16:
                writer.WriteNumberValue((short)rawValue);
                break;
            case TypeCode.Byte:
                writer.WriteNumberValue((byte)rawValue);
                break;
            case TypeCode.UInt32:
                writer.WriteNumberValue((uint)rawValue);
                break;
            case TypeCode.UInt64:
                writer.WriteNumberValue(rawValue);
                break;
            case TypeCode.UInt16:
                writer.WriteNumberValue((ushort)rawValue);
                break;
            case TypeCode.SByte:
                writer.WriteNumberValue((sbyte)rawValue);
                break;
            default:
                throw new JsonException(); // GetEnumValue should have already thrown.
        }
    }

    public void WritePropertyName(Utf8JsonWriter writer, TEnum value)
    {
        if (mapping.TryGetValue(value, out EnumInfo enumInfo))
        {
            writer.WritePropertyName(enumInfo.Preferred);
            return;
        }

        ulong rawValue = GetEnumValue(enumTypeCode, value);

        if (isFlags)
        {
            ulong calculatedValue = 0;

            var builder = new StringBuilder();
            foreach (var kvp in mapping)
            {
                enumInfo = kvp.Value;
                if (!value.HasFlag(enumInfo.Value)) continue;

                // Track the value to make sure all bits are represented.
                calculatedValue |= enumInfo.Raw;

                if (builder.Length > 0) builder.Append(", ");
                builder.Append(enumInfo.Preferred);
            }
            if (calculatedValue == rawValue)
            {
                string finalName = builder.ToString();
                writer.WritePropertyName(finalName);
                return;
            }
        }
    }

    internal static ulong GetEnumValue(TypeCode enumTypeCode, object value)
    {
        return enumTypeCode switch
        {
            TypeCode.Int32 => (ulong)(int)value,
            TypeCode.Int64 => (ulong)(long)value,
            TypeCode.Int16 => (ulong)(short)value,
            TypeCode.Byte => (byte)value,
            TypeCode.UInt32 => (uint)value,
            TypeCode.UInt64 => (ulong)value,
            TypeCode.UInt16 => (ushort)value,
            TypeCode.SByte => (ulong)(sbyte)value,
            _ => throw new NotSupportedException($"Enum '{value}' of {enumTypeCode} type is not supported."),
        };
    }

    /// <summary>
    /// Tries to retrieve a custom attribute of a specified type that is applied to the member,
    /// and optionally inspects the ancestors of that member.
    /// </summary>
    /// <typeparam name="T">The type of attribute to search for.</typeparam>
    /// <param name="element">The member to inspect.</param>
    /// <param name="inherit">true to inspect the ancestors of element; otherwise, false.</param>
    /// <param name="attribute">A custom attribute that matches T, or null if no such attribute is found.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">element is null</exception>
    /// <exception cref="NotSupportedException">element is not a constructor, method, property, event, type, or field.</exception>
    /// <exception cref="AmbiguousMatchException">More than one of the requested attributes was found.</exception>
    /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded.</exception>
    internal static bool TryGetCustomAttribute<T>(MemberInfo element, bool inherit, [NotNullWhen(true)] out T? attribute)
        where T : Attribute
    {
        ArgumentNullException.ThrowIfNull(element);

        attribute = element.GetCustomAttribute<T>(inherit);
        return attribute != null;
    }
}
