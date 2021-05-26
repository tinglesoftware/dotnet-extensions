using System.Text.Json;

namespace Tingle.Extensions.Json
{
    /// <summary>
    /// Helper class for JSON stuff
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// The default options when using SJT
        /// </summary>
        public static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        }.AddKnownConverters();
    }
}
