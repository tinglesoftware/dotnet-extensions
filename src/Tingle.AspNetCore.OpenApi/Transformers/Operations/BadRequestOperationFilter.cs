using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Tingle.AspNetCore.OpenApi.Transformers.Operations;

internal class BadRequestOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        // if the operation does not have a response for bad request, create a new one and add it
        operation.Responses ??= [];
        if (!operation.Responses.TryGetValue("400", out var response))
        {
            response = operation.Responses["400"] = new OpenApiResponse();
        }

        // set the description
        if (string.IsNullOrWhiteSpace(response.Description))
        {
            response.Description = "The request is invalid, see response for more details.";
        }

        return Task.CompletedTask;
    }
}
