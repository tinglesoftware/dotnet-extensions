using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Tingle.AspNetCore.JsonPatch.Internal;

// inspired by https://github.com/Havunen/SystemTextJsonPatch/tree/main/SystemTextJsonPatch/Internal/Proxies

internal static class PropertyProxyCache
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> CachedTypeProperties = new();
    private static readonly ConcurrentDictionary<(Type, string), PropertyProxy?> CachedPropertyProxies = new();

    internal static PropertyProxy? GetPropertyProxy(Type type, string name, JsonSerializerOptions options)
    {
        var key = (type, name);
        if (CachedPropertyProxies.TryGetValue(key, out var proxy)) return proxy;

        if (!CachedTypeProperties.TryGetValue(type, out var properties))
        {
            properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            CachedTypeProperties[type] = properties;
        }

        proxy = FindPropertyInfo(properties, name, options);
        CachedPropertyProxies[key] = proxy;

        return proxy;
    }

    private static PropertyProxy? FindPropertyInfo(PropertyInfo[] properties, string name, JsonSerializerOptions options)
    {
        // First check through all properties if property name matches JsonPropertyNameAttribute
        foreach (var propertyInfo in properties)
        {
            var attr = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (attr != null && string.Equals(attr.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return new PropertyProxy(propertyInfo);
            }
        }

        // If it didn't find match by JsonPropertyName then use serialized name
        foreach (var pi in properties)
        {
            if (options.PropertyNamingPolicy is not null && string.Equals(options.PropertyNamingPolicy.ConvertName(pi.Name), name))
            {
                return new PropertyProxy(pi);
            }
        }

        // If it didn't find match by JsonPropertyName or serialized name then use property name
        foreach (var pi in properties)
        {
            if (string.Equals(pi.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return new PropertyProxy(pi);
            }
        }

        return null;
    }
}

public interface IPropertyProxy
{
    object? GetValue(object target);
    void SetValue(object target, object? convertedValue);
    void RemoveValue(object target);
    bool Readable { get; }
    bool Writeable { get; }
    Type PropertyType { get; }
}

internal sealed class PropertyProxy(PropertyInfo info) : IPropertyProxy
{
    public object? GetValue(object target) => info.GetValue(target);
    public void SetValue(object target, object? convertedValue) => info.SetValue(target, convertedValue);
    public void RemoveValue(object target) => info.SetValue(target, null);

    public bool Readable => info.CanRead;
    public bool Writeable => info.CanWrite;
    public Type PropertyType => info.PropertyType;
}

internal sealed class JsonNodeProxy(JsonNode node, string name) : IPropertyProxy
{
    public object? GetValue(object target) => node[name];
    public void SetValue(object target, object? convertedValue) => node[name] = convertedValue != null ? JsonSerializer.SerializeToNode(convertedValue) : null;
    public void RemoveValue(object target) => node.AsObject().Remove(name);

    public bool Readable => true;
    public bool Writeable => true;
    public Type PropertyType => typeof(JsonNode);
}

internal sealed class JsonArrayProxy(JsonArray array, string name) : IPropertyProxy
{
    public object? GetValue(object target) => array[name == "-" ? array.Count - 1 : PropIndex];
    public void SetValue(object target, object? convertedValue)
    {
        var value = convertedValue == null ? null : JsonSerializer.SerializeToNode(convertedValue);

        if (name == "-") array.Add(value);
        else
        {
            var idx = PropIndex;

            array.RemoveAt(idx);
            array.Insert(idx, value);
        }
    }
    public void RemoveValue(object target) => array.RemoveAt(index: name == "-" ? array.Count - 1 : PropIndex);

    public bool Readable => true;
    public bool Writeable => true;
    public Type PropertyType => typeof(JsonNode);

    private int PropIndex => int.Parse(name, NumberStyles.Number, CultureInfo.InvariantCulture);
}
