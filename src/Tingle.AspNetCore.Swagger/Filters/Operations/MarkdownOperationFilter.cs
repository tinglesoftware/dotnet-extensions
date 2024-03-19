using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tingle.AspNetCore.Swagger.Filters.Operations;

/// <summary>
/// An <see cref="IOperationFilter"/>  that converts XML comments to Markdown.
/// </summary>
public class MarkdownOperationFilter : IOperationFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Summary = XmlCommentsHelper.ToMarkdown(operation.Summary);
        operation.Description = XmlCommentsHelper.ToMarkdown(operation.Description);
        foreach (var kvp in operation.Responses)
        {
            var response = kvp.Value;
            response.Description = XmlCommentsHelper.ToMarkdown(response.Description);
        }
    }
}
