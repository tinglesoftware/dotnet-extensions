using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

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

        var ext = new JsonArray([.. descriptions.Select(desc => new JsonObject
        {
            ["name"] = desc.Key,
            ["description"] = desc.Value,
        })]);

        document.Extensions ??= new Dictionary<string, IOpenApiExtension>();
        document.Extensions[ExtensionName] = new JsonNodeExtension(ext);

        return Task.CompletedTask;
    }
}
