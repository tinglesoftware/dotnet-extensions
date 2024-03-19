using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tingle.AspNetCore.Swagger.Filters.Documents;

/// <summary>
/// An <see cref="IDocumentFilter"/>  that converts XML comments to Markdown.
/// </summary>
public class MarkdownDocumentFilter : IDocumentFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (swaggerDoc.Tags is null) return;

        foreach (var t in swaggerDoc.Tags)
        {
            t.Description = XmlCommentsHelper.ToMarkdown(t.Description);
        }
    }
}
