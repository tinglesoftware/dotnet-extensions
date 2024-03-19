using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tingle.AspNetCore.Swagger.Filters.Documents;
using Tingle.AspNetCore.Swagger.Filters.Operations;
using Tingle.AspNetCore.Swagger.Filters.Parameters;
using Tingle.AspNetCore.Swagger.Filters.RequestBodies;
using Tingle.AspNetCore.Swagger.Filters.Schemas;

namespace Tingle.AspNetCore.Swagger.Filters;

/// <summary>
/// An <see cref="IPostConfigureOptions{TOptions}"/> for <see cref="SwaggerGenOptions"/>
/// that adds filters which convert XML comments to Markdown.
/// This should happen at the last step of configuration so that the comments are already pulled.
/// Hence why the use of <see cref="IPostConfigureOptions{TOptions}"/>.
/// </summary>
internal class ConfigureSwaggerGenXmlToMarkdown : IPostConfigureOptions<SwaggerGenOptions>
{
    /// <inheritdoc/>
    public void PostConfigure(string? name, SwaggerGenOptions options)
    {
        options.ParameterFilter<MarkdownParameterFilter>();
        options.RequestBodyFilter<MarkdownRequestBodyFilter>();
        options.OperationFilter<MarkdownOperationFilter>();
        options.SchemaFilter<MarkdownSchemaFilter>();
        options.DocumentFilter<MarkdownDocumentFilter>();
    }
}
