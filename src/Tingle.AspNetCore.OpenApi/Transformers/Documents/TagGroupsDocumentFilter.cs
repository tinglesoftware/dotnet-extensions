using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Tingle.AspNetCore.OpenApi.Transformers.Documents;

/// <summary>
/// Adds an extension to an <see cref="OpenApiDocument"/> with tag groups using the vendor extension <c>x-tagGroups</c>
/// </summary>
/// <seealso cref="IOpenApiDocumentTransformer" />
/// <param name="groups">The tag groups.</param>
/// <param name="addUngrouped">Whether to add a group for ungrouped tags named <c>Ungrouped</c></param>
public class TagGroupsDocumentTransformer(IEnumerable<OpenApiTagGroup> groups, bool addUngrouped = false) : IOpenApiDocumentTransformer
{
    private const string UngroupedGroupName = "ungrouped";

    private readonly List<OpenApiTagGroup> groups = groups?.ToList() ?? throw new ArgumentNullException(nameof(groups));

    /// <inheritdoc/>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        if (addUngrouped)
        {
            // find tags that have not been grouped
            var operationTags = document.Paths.Select(p => p.Value)
                                              .SelectMany(pi => pi.Operations?.Select(op => op.Value) ?? [])
                                              .SelectMany(op => op.Tags ?? [])
                                              .Select(t => t.Name!);

            var docTags = document.Tags?.Select(t => t.Name!) ?? [];
            var allUniqueTags = docTags.Concat(operationTags).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var alreadyGroupedTags = groups.Select(g => g.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var ungroupedTags = allUniqueTags.Except(alreadyGroupedTags, StringComparer.OrdinalIgnoreCase).ToList();

            // add group for the ungrouped tags so as to ensure they still show up in the documentation
            if (ungroupedTags.Count > 0 && !groups.Any(g => g.Name == UngroupedGroupName))
            {
                groups.Add(new OpenApiTagGroup(UngroupedGroupName, null, ungroupedTags));
            }
        }

        // Add to the document
        document.Extensions ??= [];
        document.Extensions["x-tagGroups"] = new OpenApiTagGroups { Groups = groups, };

        return Task.CompletedTask;
    }
}
