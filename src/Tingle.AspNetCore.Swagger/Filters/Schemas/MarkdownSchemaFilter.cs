using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tingle.AspNetCore.Swagger.Filters.Schemas;

/// <summary>
/// An <see cref="ISchemaFilter"/>  that converts XML comments to Markdown.
/// </summary>
public class MarkdownSchemaFilter : ISchemaFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        schema.Description = XmlCommentsHelper.ToMarkdown(schema.Description);

        // The InheritDocSchemaFilter modifies all the properties so we need to do the same here

        // Handle parameters such as in form data
        if (context.ParameterInfo != null && context.MemberInfo != null)
        {
            ApplyPropertyComments(schema);
        }

        if (schema.Properties == null) return;

        // Add the summary and examples for the properties.
        foreach (var entry in schema.Properties)
        {
            ApplyPropertyComments(entry.Value);
        }
    }

    private static void ApplyPropertyComments(OpenApiSchema propertySchema)
    {
        propertySchema.Description = XmlCommentsHelper.ToMarkdown(propertySchema.Description);

        if (propertySchema.Example is OpenApiString str)
        {
            propertySchema.Example = new OpenApiString(XmlCommentsHelper.ToMarkdown(str.Value));
        }
    }
}
