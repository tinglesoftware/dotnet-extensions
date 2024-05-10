using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tingle.AspNetCore.Swagger.Filters.Operations;

/// <summary>
/// Adds instances of <see cref="OpenApiTag"/> to an <see cref="OpenApiOperation"/> for all extra tags defined by
/// using <see cref="OperationExtraTagAttribute"/> on the controller or action
/// </summary>
/// <seealso cref="IOperationFilter" />
internal class ExtraTagsOperationFilter : IOperationFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = new List<OperationExtraTagAttribute>();

        // get attributes from the method
        var methodAttributes = context.MethodInfo.GetCustomAttributes(inherit: true).OfType<OperationExtraTagAttribute>();
        attributes.AddRange(methodAttributes);

        // get attributes from the controller
        var actionDescriptor = context.ApiDescription.ActionDescriptor;
        if (actionDescriptor is ControllerActionDescriptor cad)
        {
            var controllerAttributes = cad.ControllerTypeInfo.GetCustomAttributes(inherit: true).OfType<OperationExtraTagAttribute>();
            attributes.AddRange(controllerAttributes);
        }

        // get attributes from the endpoint metadata
        var metadataAttributes = actionDescriptor.EndpointMetadata.OfType<OperationExtraTagAttribute>();
        attributes.AddRange(metadataAttributes);

        // make the attributes unique by name
        var uniqueAttributes = attributes.DistinctBy(attr => attr.Name, StringComparer.OrdinalIgnoreCase);

        operation.Tags ??= [];

        foreach (var attr in uniqueAttributes)
        {
            operation.Tags.Add(new OpenApiTag
            {
                Name = attr.Name,
                Description = attr.Description,
                ExternalDocs = attr.ExternalDocsUrl != null
                    ? new OpenApiExternalDocs { Url = new Uri(attr.ExternalDocsUrl) }
                    : null,
            });
        }
    }
}
