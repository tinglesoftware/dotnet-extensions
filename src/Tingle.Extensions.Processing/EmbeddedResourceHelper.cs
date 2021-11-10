using System.IO;
using System.Threading.Tasks;

namespace Tingle.Extensions.Processing
{
    /// <summary>
    /// Helper for accessing embedded resources
    /// </summary>
    public static class EmbeddedResourceHelper
    {
        /// <summary>
        /// Get's the content of an embedded resource
        /// </summary>
        /// <typeparam name="T">The type whose namespace is used to scope the manifest resource name.</typeparam>
        /// <param name="resourceName">The case-sensitive name of the manifest resource being requested.</param>
        /// <returns></returns>
        public static Stream? GetResourceAsStream<T>(string resourceName) => typeof(T).Assembly.GetManifestResourceStream(resourceName);

        /// <summary>
        /// Get's the content of an embedded resource as a string
        /// </summary>
        /// <typeparam name="T">The type whose namespace is used to scope the manifest resource name.</typeparam>
        /// <param name="resourceName">The case-sensitive name of the manifest resource being requested.</param>
        /// <returns></returns>
        public static async Task<string?> GetResourceAsStringAsync<T>(string resourceName)
        {
            var st = GetResourceAsStream<T>(resourceName);
            if (st is null) return  null;
            using (st)
            {
                using var reader = new StreamReader(st);
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get's the content of an embedded resource
        /// </summary>
        /// <typeparam name="T">The type whose namespace is used to scope the manifest resource name.</typeparam>
        /// <param name="folder">The case-sensitive name of the folder the resource is placed in e.g. Files</param>
        /// <param name="fileName">The case-sensitive name of the file e.g. file.json</param>
        /// <returns></returns>
        public static Task<string?> GetResourceAsStringAsync<T>(string folder, string fileName)
            => GetResourceAsStringAsync<T>(string.Join(".", typeof(T).Namespace, folder, fileName));

        /// <summary>
        /// Get's the content of an embedded resource as a string
        /// </summary>
        /// <typeparam name="T">The type whose namespace is used to scope the manifest resource name.</typeparam>
        /// <param name="folder">The case-sensitive name of the folder the resource is placed in e.g. Files</param>
        /// <param name="fileName">The case-sensitive name of the file e.g. file.json</param>
        /// <returns></returns>
        public static Stream? GetResourceAsStream<T>(string folder, string fileName)
            => GetResourceAsStream<T>(string.Join(".", typeof(T).Namespace, folder, fileName));
    }
}
