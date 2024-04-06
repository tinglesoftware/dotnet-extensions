using Microsoft.Extensions.FileProviders;
using Tingle.Extensions.Mustache;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Extension methods for <see cref="IHostEnvironment"/>
/// </summary>
public static class IHostEnvironmentExtensions
{
    /// <summary>
    /// Gets a template by reading a template file and parsing it.
    /// </summary>
    /// <param name="environment">
    /// The <see cref="IHostEnvironment"/> that provides an instance of <see cref="IFileProvider"/>.
    /// </param>
    /// <param name="filePath">Path of the file. It can either be absolute or scoped to the <see cref="IFileProvider"/> used.</param>
    /// <returns></returns>
    public static MustacheTemplate GetTemplate(this IHostEnvironment environment, string filePath)
    {
        ArgumentNullException.ThrowIfNull(environment);
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException($"'{nameof(filePath)}' cannot be null or whitespace.", nameof(filePath));
        }

        var fi = environment.ContentRootFileProvider.GetFileInfo(filePath);
        using var fs = fi.CreateReadStream();
        return MustacheTemplate.Create(fs);
    }

    /// <summary>
    /// Gets a template by reading a template file and parsing it.
    /// </summary>
    /// <param name="environment">
    /// The <see cref="IHostEnvironment"/> that provides an instance of <see cref="IFileProvider"/>.
    /// </param>
    /// <param name="fileName">Path of the file. It can either be absolute or scoped to the <see cref="IFileProvider"/> used.</param>
    /// <returns></returns>
    public static async Task<MustacheTemplate> GetTemplateAsync(this IHostEnvironment environment, string fileName)
    {
        ArgumentNullException.ThrowIfNull(environment);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException($"'{nameof(fileName)}' cannot be null or whitespace.", nameof(fileName));
        }

        var fi = environment.ContentRootFileProvider.GetFileInfo(fileName);
        using var fs = fi.CreateReadStream();
        return await MustacheTemplate.CreateAsync(fs).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a template by reading a template file and parsing it.
    /// </summary>
    /// <param name="environment">
    /// The <see cref="IHostEnvironment"/> that provides an instance of <see cref="IFileProvider"/>.
    /// </param>
    /// <param name="folderName">Name of the folder containing the file, scoped to the <see cref="IFileProvider"/> used.</param>
    /// <param name="fileName">Name of the file scoped to the <paramref name="folderName"/> folder.</param>
    /// <returns></returns>
    public static MustacheTemplate GetTemplate(this IHostEnvironment environment, string folderName, string fileName)
    {
        ArgumentNullException.ThrowIfNull(environment);
        if (string.IsNullOrWhiteSpace(folderName))
        {
            throw new ArgumentException($"'{nameof(folderName)}' cannot be null or whitespace.", nameof(folderName));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException($"'{nameof(fileName)}' cannot be null or whitespace.", nameof(fileName));
        }

        return environment.GetTemplate(Path.Combine(folderName, fileName));
    }

    /// <summary>
    /// Gets a template by reading a template file and parsing it.
    /// </summary>
    /// <param name="environment">
    /// The <see cref="IHostEnvironment"/> that provides an instance of <see cref="IFileProvider"/>.
    /// </param>
    /// <param name="folderName">Name of the folder containing the file, scoped to the <see cref="IFileProvider"/> used.</param>
    /// <param name="fileName">Name of the file scoped to the <paramref name="folderName"/> folder.</param>
    /// <returns></returns>
    public static Task<MustacheTemplate> GetTemplateAsync(this IHostEnvironment environment, string folderName, string fileName)
    {
        ArgumentNullException.ThrowIfNull(environment);
        if (string.IsNullOrWhiteSpace(folderName))
        {
            throw new ArgumentException($"'{nameof(folderName)}' cannot be null or whitespace.", nameof(folderName));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException($"'{nameof(fileName)}' cannot be null or whitespace.", nameof(fileName));
        }

        return environment.GetTemplateAsync(Path.Combine(folderName, fileName));
    }
}
