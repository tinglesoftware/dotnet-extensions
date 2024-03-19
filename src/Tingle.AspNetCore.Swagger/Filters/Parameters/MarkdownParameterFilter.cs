using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tingle.AspNetCore.Swagger.Filters.Parameters;

/// <summary>
/// An <see cref="IParameterFilter"/>  that converts XML comments to Markdown.
/// </summary>
public class MarkdownParameterFilter : IParameterFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        parameter.Description = XmlCommentsHelper.ToMarkdown(parameter.Description);
    }
}
