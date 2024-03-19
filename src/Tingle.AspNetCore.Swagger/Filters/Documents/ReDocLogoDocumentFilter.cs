using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tingle.AspNetCore.Swagger.Filters.Documents;

/// <summary>
/// Adds an extension to an <see cref="OpenApiDocument"/> with a logo using the vendor extension <c>x-logo</c>
/// </summary>
/// <seealso cref="IDocumentFilter" />
/// <param name="logo"></param>
public class ReDocLogoDocumentFilter(OpenApiReDocLogo logo) : IDocumentFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Info.Extensions["x-logo"] = logo;
    }
}
