using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Tingle.AspNetCore.OpenApi.Transformers.Documents;

namespace Tingle.AspNetCore.OpenApi.Transformers.Operations;

/// <summary>
/// Adds an extension to an <see cref="OpenApiOperation"/> with error codes using
/// the vendor extension <c>x-error-codes</c>
/// </summary>
/// <seealso cref="IOpenApiOperationTransformer" />
public class ErrorCodesOperationTransformer : IOpenApiOperationTransformer
{
    /// <inheritdoc/>
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var attributes = new List<OperationErrorCodesAttribute>();

        // get attributes from the method
        if (context.Description.TryGetMethodInfo(out var methodInfo))
        {
            var methodAttributes = methodInfo.GetCustomAttributes(inherit: true).OfType<OperationErrorCodesAttribute>();
            attributes.AddRange(methodAttributes);
        }

        // get attributes from the controller
        var actionDescriptor = context.Description.ActionDescriptor;
        if (actionDescriptor is ControllerActionDescriptor cad)
        {
            var controllerAttributes = cad.ControllerTypeInfo.GetCustomAttributes(inherit: true).OfType<OperationErrorCodesAttribute>();
            attributes.AddRange(controllerAttributes);
        }

        // get attributes from the endpoint metadata
        var metadataAttributes = actionDescriptor.EndpointMetadata.OfType<OperationErrorCodesAttribute>();
        attributes.AddRange(metadataAttributes);

        // make unique error codes
        var uniqueErrorCodes = attributes.SelectMany(attr => attr.Errors)
                                         .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // if there are no errors, do not proceed
        if (uniqueErrorCodes.Count <= 0) return Task.CompletedTask;

        var ext = new System.Text.Json.Nodes.JsonArray([.. uniqueErrorCodes.Select(code => code)]);

        operation.Extensions ??= new Dictionary<string, IOpenApiExtension>();
        operation.Extensions[ErrorCodesDocumentTransformer.ExtensionName] = new JsonNodeExtension(ext);

        return Task.CompletedTask;
    }
}
