using System.Text.Json;
using Tingle.Extensions.Json;

namespace System
{
    /// <summary>
    /// Extension methods for <see cref="object"/> realted to copying and cloning
    /// </summary>
    public static class ObjectExtensions
    {

        /// <summary>
        /// Create an object clone via JSON serializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">the source of the data</param>
        /// <param name="options">the serialization settings if any</param>
        [Obsolete("Use record types instead")]
        public static T? JsonClone<T>(this T? source, JsonSerializerOptions? options = null)
            where T : class
        {
            if (source == default) return default;
            var json = JsonSerializer.Serialize(source, options ?? JsonHelper.DefaultOptions);
            return JsonSerializer.Deserialize<T>(json, options ?? JsonHelper.DefaultOptions);
        }

        /// <summary>
        /// Convert one object to another via JSON serializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">the source of the data</param>
        /// <param name="options">the serialization settings if any</param>
        /// <returns></returns>
        [Obsolete("Use record types instead")]
        public static T? JsonConvertTo<T>(this object? source, JsonSerializerOptions? options = null)
        {
            if (source == default) return default;
            var json = JsonSerializer.Serialize(source, options ?? JsonHelper.DefaultOptions);
            return JsonSerializer.Deserialize<T>(json, options ?? JsonHelper.DefaultOptions);
        }
    }
}
