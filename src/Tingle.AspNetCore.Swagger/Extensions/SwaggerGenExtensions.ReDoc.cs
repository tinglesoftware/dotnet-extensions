using Swashbuckle.AspNetCore.SwaggerGen;
using Tingle.AspNetCore.Swagger;
using Tingle.AspNetCore.Swagger.Filters.Documents;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="SwaggerGenOptions"/>
/// </summary>
public static partial class SwaggerGenExtensions
{
    /// <summary>
    /// Add logo for ReDoc to the document using the vendor extension <c>x-logo</c>
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logo">The logo details</param>
    /// <returns></returns>
    /// <seealso cref="ReDocLogoDocumentFilter" />
    public static SwaggerGenOptions AddReDocLogo(this SwaggerGenOptions options, OpenApiReDocLogo logo)
    {
        options.DocumentFilter<ReDocLogoDocumentFilter>(logo);
        return options;
    }
}
