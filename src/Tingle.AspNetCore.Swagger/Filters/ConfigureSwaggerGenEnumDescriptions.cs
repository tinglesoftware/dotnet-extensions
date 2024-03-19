using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tingle.AspNetCore.Swagger.Filters.Schemas;

namespace Tingle.AspNetCore.Swagger.Filters;

/// <summary>
/// An <see cref="IPostConfigureOptions{TOptions}"/> for <see cref="SwaggerGenOptions"/>
/// that adds filters which add enum descriptions.
/// This should happen at the last step of configuration so that the comments are already pulled.
/// Hence why the use of <see cref="IPostConfigureOptions{TOptions}"/>.
/// </summary>
internal class ConfigureSwaggerGenEnumDescriptions : IPostConfigureOptions<SwaggerGenOptions>
{
    /// <inheritdoc/>
    public void PostConfigure(string? name, SwaggerGenOptions options)
    {
        // Add matching schema filters for enum descriptions after caller
        // configuration has been completed so that the target descriptors are present.
        foreach (var f in options.SchemaFilterDescriptors.ToArray())
        {
            if (f.Type == typeof(XmlCommentsSchemaFilter))
            {
                options.SchemaFilterDescriptors.Add(new FilterDescriptor
                {
                    Type = typeof(EnumDescriptionsSchemaFilter),
                    Arguments = f.Arguments,
                });
            }
        }
    }
}
