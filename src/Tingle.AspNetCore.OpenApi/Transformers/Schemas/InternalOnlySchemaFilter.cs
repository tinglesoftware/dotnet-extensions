using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Tingle.AspNetCore.OpenApi.Transformers.Operations;

namespace Tingle.AspNetCore.OpenApi.Transformers.Schemas;

/// <summary>
/// An <see cref="IOpenApiSchemaTransformer"/> that decorates <see cref="OpenApiSchema"/> instances
/// with <c>x-internal</c> when <see cref="InternalOnlyAttribute"/> is present.
/// </summary>
public class InternalOnlySchemaTransformer : IOpenApiSchemaTransformer
{
    /// <inheritdoc/>
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        // Check if the type has the attribute declared/annotated
        var attr = context.ParameterDescription?.Type.GetCustomAttribute<InternalOnlyAttribute>(inherit: true);
        if (attr is null) return Task.CompletedTask;

        // At this point, the API is internal only, so just set the extension value
        schema.Extensions[InternalOnlyOperationTransformer.ExtensionName] = new OpenApiAny(true);

        return Task.CompletedTask;
    }
}
