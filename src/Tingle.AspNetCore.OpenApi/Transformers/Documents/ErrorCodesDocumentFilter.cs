using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Tingle.AspNetCore.OpenApi.Transformers.Documents;

/// <summary>
/// Adds an extension to an <see cref="OpenApiDocument"/> with error codes description using the
/// vendor extension <c>x-error-codes</c>
/// </summary>
/// <seealso cref="IOpenApiDocumentTransformer" />
/// <param name="descriptions">
/// The descriptions for error codes.
/// The key (<see cref="KeyValuePair{TKey, TValue}.Key"/>) represents the error code whereas
/// the value (<see cref="KeyValuePair{TKey, TValue}.Value"/>) represents the description.
/// </param>
public class ErrorCodesDocumentTransformer(IDictionary<string, string> descriptions) : IOpenApiDocumentTransformer
{
    internal const string ExtensionName = "x-error-codes";

    private readonly IDictionary<string, string> descriptions = descriptions ?? throw new ArgumentNullException(nameof(descriptions));

    /// <inheritdoc/>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        // if there are no errors, do not proceed
        if (descriptions.Count <= 0) return Task.CompletedTask;

        var ext = new OpenApiArray();
        foreach (var desc in descriptions)
        {
            ext.Add(new OpenApiObject
            {
                ["name"] = new OpenApiString(desc.Key),
                ["description"] = new OpenApiString(desc.Value),
            });
        };

        document.Extensions[ExtensionName] = ext;

        return Task.CompletedTask;
    }
}
