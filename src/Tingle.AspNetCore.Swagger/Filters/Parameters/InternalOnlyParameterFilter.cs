using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Tingle.AspNetCore.Swagger.Filters.Operations;

namespace Tingle.AspNetCore.Swagger.Filters.Parameters;

/// <summary>
/// An <see cref="IParameterFilter"/> that decorates <see cref="OpenApiOperation"/> instances
/// with <c>x-internal</c> when <see cref="InternalOnlyAttribute"/> is present.
/// </summary>
public class InternalOnlyParameterFilter : IParameterFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        // Check if the type has the attribute declared/annotated
        var attr = context.PropertyInfo?.GetCustomAttribute<InternalOnlyAttribute>(inherit: true)
                ?? context.ParameterInfo?.GetCustomAttribute<InternalOnlyAttribute>(inherit: true);
        if (attr is null) return;

        // At this point, the API is internal only, so just set the extension value
        parameter.Extensions[InternalOnlyOperationFilter.ExtensionName] = new OpenApiBoolean(true);
    }
}
