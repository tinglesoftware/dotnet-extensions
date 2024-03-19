using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tingle.AspNetCore.Swagger.Filters.RequestBodies;

/// <summary>
/// An <see cref="IRequestBodyFilter"/>  that converts XML comments to Markdown.
/// </summary>
public class MarkdownRequestBodyFilter : IRequestBodyFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiRequestBody requestBody, RequestBodyFilterContext context)
    {
        requestBody.Description = XmlCommentsHelper.ToMarkdown(requestBody.Description);
    }
}
