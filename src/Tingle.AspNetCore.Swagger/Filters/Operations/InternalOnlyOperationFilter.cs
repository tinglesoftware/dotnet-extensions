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
        // check attribute on the method
        var attr = context.MethodInfo.GetCustomAttribute<InternalOnlyAttribute>(inherit: true);

        // check attribute on the controller
        var actionDescriptor = context.ApiDescription.ActionDescriptor;
        attr ??= (actionDescriptor as ControllerActionDescriptor)?.ControllerTypeInfo.GetCustomAttribute<InternalOnlyAttribute>(inherit: true);

        // check the endpoint metadata
        attr ??= actionDescriptor.EndpointMetadata.OfType<InternalOnlyAttribute>().FirstOrDefault();
        if (attr is null) return;

        // At this point, the API is internal only, so just set the extension value
        operation.Extensions[ExtensionName] = new OpenApiBoolean(true);
    }
}
