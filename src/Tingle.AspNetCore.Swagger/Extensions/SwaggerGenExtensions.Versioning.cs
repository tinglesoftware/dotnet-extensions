using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="SwaggerGenOptions"/>
/// </summary>
public static partial class SwaggerGenExtensions
{
    /// <summary>
    /// Adds a swagger document
    /// </summary>
    /// <param name="options"></param>
    /// <param name="documentName">the name of the document e.g. v1</param>
    /// <param name="versionName">the name of the version e.g. v1-2019</param>
    /// <param name="title">the title of the document</param>
    /// <param name="description">the description of the document, if any</param>
    /// <param name="customize">an action to customize the created instance of <see cref="OpenApiInfo"/> before adding it</param>
    /// <returns></returns>
    public static SwaggerGenOptions AddDocument(this SwaggerGenOptions options,
                                                string documentName,
                                                string? versionName = null,
                                                string? title = null,
                                                string? description = null,
                                                Action<OpenApiInfo>? customize = null)
    {
        var info = new OpenApiInfo
        {
            Version = versionName ?? documentName,
            Title = title,
            Description = description,
        };
        customize?.Invoke(info);
        options.SwaggerDoc(documentName, info);
        return options;
    }

    /// <summary>
    /// Adds a swagger document from an API version description
    /// </summary>
    /// <param name="options"></param>
    /// <param name="version">the API version description</param>
    /// <param name="title">the title of the document</param>
    /// <param name="description">the description of the document, if any</param>
    /// <param name="deprecationSuffix">the suffix to add for deprecated versions</param>
    /// <param name="customize">an action to customize the created instance of <see cref="OpenApiInfo"/> before adding it</param>
    /// <returns></returns>
    public static SwaggerGenOptions AddDocument(this SwaggerGenOptions options,
                                                ApiVersionDescription version,
                                                string? title = null,
                                                string? description = null,
                                                string? deprecationSuffix = "[deprecated]",
                                                Action<OpenApiInfo>? customize = null)
    {
        var finalTitle = title;
        if (version.IsDeprecated) finalTitle += $" {deprecationSuffix}";
        var info = new OpenApiInfo
        {
            Version = version.ApiVersion.ToString(),
            Title = finalTitle,
            Description = description,
        };
        customize?.Invoke(info);
        options.SwaggerDoc(version.GroupName, info);
        return options;
    }

    /// <summary>
    /// Adds swagger documents for each API version description provided
    /// </summary>
    /// <param name="options"></param>
    /// <param name="versions">the versions for which to add swagger documents</param>
    /// <param name="title">the title of the documents</param>
    /// <param name="description">the description of the documents, if any</param>
    /// <param name="deprecationSuffix">the suffix to add for deprecated versions</param>
    /// <param name="skipDeprecated">set true to skip versions marked as deprecated</param>
    /// <param name="customize">an action to customize the created instance of <see cref="OpenApiInfo"/> before adding it</param>
    /// <returns></returns>
    public static SwaggerGenOptions AddDocuments(this SwaggerGenOptions options,
                                                 IEnumerable<ApiVersionDescription> versions,
                                                 string? title = null,
                                                 string? description = null,
                                                 bool skipDeprecated = false,
                                                 string deprecationSuffix = "[deprecated]",
                                                 Action<OpenApiInfo>? customize = null)
    {
        // add a swagger document for each discovered API version
        foreach (var v in versions)
        {
            // skip deprecated versions if specified
            if (skipDeprecated && v.IsDeprecated) continue;

            // add document
            options.AddDocument(
                version: v,
                title: title,
                description: description,
                deprecationSuffix: deprecationSuffix,
                customize: customize);
        }

        return options;
    }
}
