using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tingle.AspNetCore.Swagger.Filters.Documents;

namespace Tingle.AspNetCore.Swagger.Filters.Operations;

/// <summary>
/// Adds an extension to an <see cref="OpenApiOperation"/> with error codes using
/// the vendor extension <c>x-error-codes</c>
/// </summary>
/// <seealso cref="IOperationFilter" />
public class ErrorCodesOperationFilter : IOperationFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = new List<OperationErrorCodesAttribute>();

        // get attributes from the method
        var methodAttributes = context.MethodInfo.GetCustomAttributes(inherit: true).OfType<OperationErrorCodesAttribute>();
        attributes.AddRange(methodAttributes);

        // get attributes from the controller
        var actionDescriptor = context.ApiDescription.ActionDescriptor;
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
        if (uniqueErrorCodes.Count <= 0) return;

        var ext = new OpenApiArray();
        foreach (var code in uniqueErrorCodes)
        {
            ext.Add(new OpenApiString(code));
        }

        operation.Extensions[ErrorCodesDocumentFilter.ExtensionName] = ext;
    }
}
