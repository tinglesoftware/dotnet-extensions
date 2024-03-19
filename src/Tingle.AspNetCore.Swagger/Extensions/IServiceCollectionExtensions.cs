#if NET8_0_OR_GREATER
using Asp.Versioning.ApiExplorer;
#endif
using Microsoft.Extensions.Options;
#if NET8_0_OR_GREATER
using Microsoft.OpenApi.Models;
#endif
using Swashbuckle.AspNetCore.SwaggerGen;
using Tingle.AspNetCore.Swagger.Filters;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds conversion of XML comments extracted for Swagger to markdown.
    /// </summary>
    /// <param name="services">the service collection to use</param>
    /// <returns></returns>
    public static IServiceCollection AddSwaggerXmlToMarkdown(this IServiceCollection services)
    {
        return services.AddTransient<IPostConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenXmlToMarkdown>();
    }

    /// <summary>
    /// Adds enum descriptions.
    /// This should be called after all XML documents have been added.
    /// </summary>
    /// <param name="services">the service collection to use</param>
    /// <returns></returns>
    public static IServiceCollection AddSwaggerEnumDescriptions(this IServiceCollection services)
    {
        return services.AddTransient<IPostConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenEnumDescriptions>();
    }

#if NET8_0_OR_GREATER

    /// <summary>
    /// Adds swagger documents for api version descriptions declared in code.
    /// This is resolved through <see cref="IApiVersionDescriptionProvider.ApiVersionDescriptions"/>
    /// </summary>
    /// <param name="services">the service collection to use</param>
    /// <param name="title">the title of the documents</param>
    /// <param name="description">the description of the documents, if any</param>
    /// <param name="deprecationSuffix">the suffix to add for deprecated versions</param>
    /// <param name="skipDeprecated">set true to skip versions marked as deprecated</param>
    /// <param name="customize">an action to customize the created instance of <see cref="OpenApiInfo"/> before adding it</param>
    /// <returns></returns>
    public static IServiceCollection AddSwaggerDocsAutoDiscovery(this IServiceCollection services,
                                                                 string? title = null,
                                                                 string? description = null,
                                                                 bool skipDeprecated = false,
                                                                 string deprecationSuffix = "[deprecated]",
                                                                 Action<OpenApiInfo>? customize = null)
    {
        return services.AddTransient<IConfigureOptions<SwaggerGenOptions>>(serviceProvider =>
        {
            void configureAllVersions(IEnumerable<ApiVersionDescription> versions, SwaggerGenOptions options)
            {
                options.AddDocuments(versions: versions,
                                     title: title,
                                     description: description,
                                     skipDeprecated: skipDeprecated,
                                     deprecationSuffix: deprecationSuffix,
                                     customize: customize);
            }
            var provider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
            return new ConfigureSwaggerGenAddDocs(provider, configureAllVersions);
        });
    }

    /// <summary>
    /// Configures the Swagger generation options.
    /// </summary>
    /// <remarks>
    /// This allows API versioning to define a Swagger document per API version after the
    /// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.
    /// </remarks>
    /// <param name="provider">the <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
    /// <param name="configure">the action to call when configuring</param>
    internal class ConfigureSwaggerGenAddDocs(IApiVersionDescriptionProvider provider, Action<IEnumerable<ApiVersionDescription>, SwaggerGenOptions> configure)
        : IConfigureOptions<SwaggerGenOptions>
    {
        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options) => configure?.Invoke(provider.ApiVersionDescriptions, options);
    }

#endif
}
