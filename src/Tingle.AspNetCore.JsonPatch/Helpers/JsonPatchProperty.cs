using System.Text.Json;

namespace Tingle.AspNetCore.JsonPatch;

/// <summary>
/// Metadata for JsonProperty.
/// </summary>
public class JsonPatchProperty(JsonProperty property, object parent)
{
    /// <summary>
    /// Gets or sets JsonProperty.
    /// </summary>
    public JsonProperty Property { get; set; } = property;

    /// <summary>
    /// Gets or sets Parent.
    /// </summary>
    public object Parent { get; set; } = parent ?? throw new ArgumentNullException(nameof(parent));
}
