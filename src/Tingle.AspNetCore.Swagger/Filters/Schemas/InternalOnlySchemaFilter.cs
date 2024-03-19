using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Tingle.AspNetCore.Swagger.Filters.Operations;

namespace Tingle.AspNetCore.Swagger.Filters.Schemas;

/// <summary>
/// An <see cref="ISchemaFilter"/> that decorates <see cref="OpenApiSchema"/> instances
/// with <c>x-internal</c> when <see cref="InternalOnlyAttribute"/> is present.
/// </summary>
public class InternalOnlySchemaFilter : ISchemaFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Check if the type has the attribute declared/annotated
        var attr = context.MemberInfo?.GetCustomAttribute<InternalOnlyAttribute>(inherit: true)
                ?? context.ParameterInfo?.GetCustomAttribute<InternalOnlyAttribute>(inherit: true)
                ?? context.Type.GetCustomAttribute<InternalOnlyAttribute>(inherit: true);
        if (attr is null) return;

        // At this point, the API is internal only, so just set the extension value
        schema.Extensions[InternalOnlyOperationFilter.ExtensionName] = new OpenApiBoolean(true);
    }
}
