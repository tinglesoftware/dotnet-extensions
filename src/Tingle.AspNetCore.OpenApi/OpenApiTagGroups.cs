using Microsoft.OpenApi;

namespace Tingle.AspNetCore.OpenApi;

/// <summary>
/// Represents tag groups to be added to the generated <see cref="OpenApiDocument"/>.
/// </summary>
public class OpenApiTagGroups : IOpenApiExtension
{
    /// <summary>
    /// The tag groups to be written.
    /// </summary>
    public IList<OpenApiTagGroup>? Groups { get; set; }

    /// <inheritdoc/>
    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        ArgumentNullException.ThrowIfNull(writer);
        if (Groups is null || Groups.Count == 0) return;

        writer.WriteStartArray();

        foreach (var item in Groups)
        {
            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, item.Name);

            // description
            if (item.Description is not null)
            {
                writer.WriteProperty(OpenApiConstants.Description, item.Description);
            }

            // internal
            writer.WriteProperty(Transformers.Operations.InternalOnlyOperationTransformer.ExtensionName, item.Internal, defaultValue: false);

            // tags
            writer.WriteRequiredCollection(OpenApiConstants.Tags, item.Tags, (w, s) => s!.SerializeAsV3(w));

            writer.WriteEndObject();
        }

        writer.WriteEndArray();
    }
}
