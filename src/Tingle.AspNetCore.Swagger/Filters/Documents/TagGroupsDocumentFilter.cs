using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tingle.AspNetCore.Swagger.Filters.Documents;

/// <summary>
/// Adds an extension to an <see cref="OpenApiDocument"/> with tag groups using the vendor extension <c>x-tagGroups</c>
/// </summary>
/// <seealso cref="IDocumentFilter" />
/// <param name="groups">The tag groups.</param>
/// <param name="addUngrouped">Whether to add a group for ungrouped tags named <c>Ungrouped</c></param>
public class TagGroupsDocumentFilter(IEnumerable<OpenApiTagGroup> groups, bool addUngrouped = false) : IDocumentFilter
{
    private const string UngroupedGroupName = "ungrouped";

    private readonly List<OpenApiTagGroup> groups = groups?.ToList() ?? throw new ArgumentNullException(nameof(groups));

    /// <inheritdoc/>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (addUngrouped)
        {
            // find tags that have not been grouped
            var operationTags = swaggerDoc.Paths.Select(p => p.Value)
                                                .SelectMany(pi => pi.Operations.Select(op => op.Value))
                                                .SelectMany(op => op.Tags)
                                                .Select(t => t.Name);

            var docTags = swaggerDoc.Tags.Select(t => t.Name);
            var allUniqueTags = docTags.Concat(operationTags).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var alreadyGroupedTags = groups.Select(g => g.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var ungroupedTags = allUniqueTags.Except(alreadyGroupedTags, StringComparer.OrdinalIgnoreCase).ToList();

            // add group for the ungrouped tags so as to ensure they still show up in the documentation
            if (ungroupedTags.Count > 0 && !groups.Any(g => g.Name == UngroupedGroupName))
            {
                groups.Add(new OpenApiTagGroup(UngroupedGroupName, null, ungroupedTags));
            }
        }

        // Add to the swagger spec
        swaggerDoc.Extensions["x-tagGroups"] = new OpenApiTagGroups { Groups = groups, };
    }
}
