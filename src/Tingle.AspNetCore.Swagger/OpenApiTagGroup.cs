using Microsoft.OpenApi.Models;

namespace Tingle.AspNetCore.Swagger;

/// <summary>
/// Represents a tag group to be added to the generated <see cref="OpenApiDocument"/> via <see cref="OpenApiTagGroups"/>.
/// </summary>
public class OpenApiTagGroup
{
    /// <summary>Creates an instance of <see cref="OpenApiTagGroup"/>.</summary>
    /// <param name="name">The name of the tag. group</param>
    /// <param name="tags">The tags in the group.</param>
    /// <param name="internal">Whether the tag group is an internal one.</param>
    public OpenApiTagGroup(string name, IEnumerable<string> tags, bool @internal = false)
        : this(name: name, description: null, tags: tags, @internal: @internal) { }

    /// <summary>Creates an instance of <see cref="OpenApiTagGroup"/>.</summary>
    /// <param name="name">The name of the tag group.</param>
    /// <param name="tags">The tags in the group.</param>
    /// <param name="internal">Whether the tag group is an internal one.</param>
    public OpenApiTagGroup(string name, List<OpenApiReference> tags, bool @internal = false)
        : this(name: name, description: null, tags: tags, @internal: @internal) { }

    /// <summary>Creates an instance of <see cref="OpenApiTagGroup"/>.</summary>
    /// <param name="name">The name of the tag group.</param>
    /// <param name="description">The description of the tag group (optional).</param>
    /// <param name="tags">The tags in the group.</param>
    /// <param name="internal">Whether the tag group is an internal one.</param>
    public OpenApiTagGroup(string name, string? description, IEnumerable<string> tags, bool @internal = false)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Tags = tags.Select(m => new OpenApiReference { Id = m, Type = ReferenceType.Tag, })
                   .ToList();
        Internal = @internal;
    }

    /// <summary>Creates an instance of <see cref="OpenApiTagGroup"/>.</summary>
    /// <param name="name">The name of the tag group.</param>
    /// <param name="description">The description of the tag group (optional).</param>
    /// <param name="tags">The tags in the group.</param>
    /// <param name="internal">Whether the tag group is an internal one.</param>
    public OpenApiTagGroup(string name, string? description, List<OpenApiReference> tags, bool @internal = false)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Tags = tags ?? throw new ArgumentNullException(nameof(tags));
        Internal = @internal;
    }

    /// <summary>The name of the tag group.</summary>
    public string Name { get; init; }

    /// <summary>The description of the tag group (optional).</summary>
    public string? Description { get; init; }

    /// <summary>The tags in the group.</summary>
    public List<OpenApiReference> Tags { get; init; }

    /// <summary>Whether the tag group is an internal one.</summary>
    public bool Internal { get; init; }
}
