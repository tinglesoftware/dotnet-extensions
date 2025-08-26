using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Tingle.AspNetCore.OpenApi.Transformers.Operations;

/// <summary>
/// An <see cref="IOpenApiOperationTransformer"/> that decorates <see cref="OpenApiOperation"/> instances
/// with <c>x-internal</c> when <see cref="InternalOnlyAttribute"/> is present.
/// </summary>
public class InternalOnlyOperationTransformer : IOpenApiOperationTransformer
{
    internal const string ExtensionName = "x-internal";

    /// <inheritdoc/>
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        // check attribute on the method
        InternalOnlyAttribute? attr = null;
        if (context.Description.TryGetMethodInfo(out var methodInfo))
            attr = methodInfo.GetCustomAttribute<InternalOnlyAttribute>(inherit: true);

        // check attribute on the controller
        var actionDescriptor = context.Description.ActionDescriptor;
        attr ??= (actionDescriptor as ControllerActionDescriptor)?.ControllerTypeInfo.GetCustomAttribute<InternalOnlyAttribute>(inherit: true);

        // check the endpoint metadata
        attr ??= actionDescriptor.EndpointMetadata.OfType<InternalOnlyAttribute>().FirstOrDefault();
        if (attr is null) return Task.CompletedTask;

        // At this point, the API is internal only, so just set the extension value
        operation.Extensions ??= new Dictionary<string, IOpenApiExtension>();
        operation.Extensions[ExtensionName] = new JsonNodeExtension(true);

        return Task.CompletedTask;
    }
}
