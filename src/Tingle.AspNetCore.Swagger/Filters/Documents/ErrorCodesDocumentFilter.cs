using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tingle.AspNetCore.Swagger.Filters.Documents;

/// <summary>
/// Adds an extension to an <see cref="OpenApiDocument"/> with error codes description using the
/// vendor extension <c>x-error-codes</c>
/// </summary>
/// <seealso cref="IDocumentFilter" />
/// <param name="descriptions">
/// The descriptions for error codes.
/// The key (<see cref="KeyValuePair{TKey, TValue}.Key"/>) represents the error code whereas
/// the value (<see cref="KeyValuePair{TKey, TValue}.Value"/>) represents the description.
/// </param>
public class ErrorCodesDocumentFilter(IDictionary<string, string> descriptions) : IDocumentFilter
{
    internal const string ExtensionName = "x-error-codes";

    private readonly IDictionary<string, string> descriptions = descriptions ?? throw new ArgumentNullException(nameof(descriptions));

    /// <inheritdoc/>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // if there are no errors, do not proceed
        if (descriptions.Count <= 0) return;

        var ext = new OpenApiArray();
        foreach (var desc in descriptions)
        {
            ext.Add(new OpenApiObject
            {
                ["name"] = new OpenApiString(desc.Key),
                ["description"] = new OpenApiString(desc.Value),
            });
        };

        swaggerDoc.Extensions[ExtensionName] = ext;
    }
}
