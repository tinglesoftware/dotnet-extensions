using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Tingle.AspNetCore.Swagger.Filters.Operations;

/// <summary>
/// An <see cref="IOperationFilter"/> that decorates <see cref="OpenApiOperation"/> instances
/// with <c>x-internal</c> when <see cref="InternalOnlyAttribute"/> is present.
/// </summary>
public class InternalOnlyOperationFilter : IOperationFilter
{
    internal const string ExtensionName = "x-internal";

    /// <inheritdoc/>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if the method or controller has the attribute declared/annotated
        var attr = context.MethodInfo.GetCustomAttribute<InternalOnlyAttribute>(inherit: true);
        if (attr is null && context.ApiDescription.ActionDescriptor is ControllerActionDescriptor cad)
        {
            attr = cad.ControllerTypeInfo.GetCustomAttribute<InternalOnlyAttribute>(inherit: true);
        }
        if (attr is null) return;

        // At this point, the API is internal only, so just set the extension value
        operation.Extensions[ExtensionName] = new OpenApiBoolean(true);
    }
}
