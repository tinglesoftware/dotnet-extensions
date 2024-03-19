using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tingle.AspNetCore.Swagger.Filters.Operations;

internal class BadRequestOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // if the operation does not have a response for bad request, create a new one and add it
        if (!operation.Responses.TryGetValue("400", out var response))
        {
            response = operation.Responses["400"] = new OpenApiResponse();
        }

        // set the description
        if (string.IsNullOrWhiteSpace(response.Description))
        {
            response.Description = "The request is invalid, see response for more details.";
        }
    }
}
