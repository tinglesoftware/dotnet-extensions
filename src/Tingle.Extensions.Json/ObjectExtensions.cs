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

        // TODO: enable once officially supported https://github.com/dotnet/runtime/issues/29538

        ///// <summary>
        ///// Copy data from one object to another via JSON serializer
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="source">the source of the data</param>
        ///// <param name="target"></param>
        ///// <param name="options">the serialization settings if any</param>
        //public static void JsonCopyTo<T>(this T source, T target, JsonSerializerOptions options = null)
        //    where T : class
        //{
        //    if (source == default || target == default) return;
        //    var json = JsonSerializer.Serialize(source, options ?? JsonHelper.defaultOptions);
        //    JsonSerializer.PopulateObject(json, target, options ?? JsonHelper.defaultOptions);
        //}

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
