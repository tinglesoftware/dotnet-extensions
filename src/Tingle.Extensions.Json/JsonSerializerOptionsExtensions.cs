using System.Text.Json.Serialization;
using Tingle.Extensions.Json;

namespace System.Text.Json
{
    /// <summary>
    /// Extension methods for <see cref="JsonSerializerOptions"/>
    /// </summary>
    public static class JsonSerializerOptionsExtensions
    {
        /// <summary>
        /// Add a converter
        /// </summary>
        /// <typeparam name="TConverter">the type to add</typeparam>
        /// <param name="options">the options to add the converter to</param>
        /// <returns></returns>
        public static JsonSerializerOptions AddConverter<TConverter>(this JsonSerializerOptions options)
            where TConverter : JsonConverter, new()
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            options.Converters.Add(new TConverter());
            return options;
        }

        /// <summary>
        /// Add a converter for a type
        /// </summary>
        /// <typeparam name="TConverter">the type to add</typeparam>
        /// <typeparam name="TType">the type being converted</typeparam>
        /// <param name="options">the options to add the converter to</param>
        /// <returns></returns>
        public static JsonSerializerOptions AddConverterForType<TConverter, TType>(this JsonSerializerOptions options)
            where TConverter : JsonConverter<TType>, new()
        {
            return options.AddConverter<TConverter>();
        }

        /// <summary>
        /// Add a converter the enum/string converter <see cref="JsonStringEnumConverter"/>
        /// </summary>
        /// <param name="options">the options to add the converter to</param>
        /// <param name="namingPolicy">the naming policy, if not set, the one in the options is used</param>
        /// <returns></returns>
        public static JsonSerializerOptions AddConverterForEnumsAsStrings(this JsonSerializerOptions options,
                                                                          JsonNamingPolicy? namingPolicy = null)
        {
            options.Converters.Add(new JsonStringEnumConverter(namingPolicy ?? options?.PropertyNamingPolicy));
            return options!;
        }

        /// <summary>
        /// Add extra converters that are known
        /// </summary>
        /// <param name="options">the options to add the converter to</param>
        /// <param name="namingPolicy">the naming policy, if not set, the one in the options is used</param>
        /// <returns></returns>
        public static JsonSerializerOptions AddKnownConverters(this JsonSerializerOptions options,
                                                               JsonNamingPolicy? namingPolicy = null)
        {
            return options.AddConverterForEnumsAsStrings(namingPolicy: namingPolicy);
        }
    }
}
