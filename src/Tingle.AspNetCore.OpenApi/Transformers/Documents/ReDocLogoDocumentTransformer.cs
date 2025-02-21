using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Tingle.AspNetCore.OpenApi.Transformers.Documents;

/// <summary>
/// Adds an extension to an <see cref="OpenApiDocument"/> with a logo using the vendor extension <c>x-logo</c>
/// </summary>
/// <seealso cref="IOpenApiDocumentTransformer" />
/// <param name="logo"></param>
public class ReDocLogoDocumentTransformer(OpenApiReDocLogo logo) : IOpenApiDocumentTransformer
{
    /// <inheritdoc/>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info.Extensions["x-logo"] = logo;
        return Task.CompletedTask;
    }
}
