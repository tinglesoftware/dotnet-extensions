using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tingle.AspNetCore.Swagger.Filters.Operations;

internal class AuthorizationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var auth = context.MethodInfo.GetCustomAttributes(inherit: true).OfType<AuthorizeAttribute>();
        var anonymous = context.MethodInfo.GetCustomAttributes(inherit: true).OfType<AllowAnonymousAttribute>();

        if (auth.Any() && !anonymous.Any())
        {
            // if the operation does not have a response for unauthenticated, create a new one and add it
            if (!operation.Responses.TryGetValue("401", out _))
            {
                operation.Responses["401"] = new OpenApiResponse
                {
                    Description = "The request is not authenticated",
                    Headers = new Dictionary<string, OpenApiHeader>
                    {
                        [Microsoft.Net.Http.Headers.HeaderNames.WWWAuthenticate] = new OpenApiHeader
                        {
                            Description = "The reason why authentication failed",
                        }
                    }
                };
            }

            // if the operation does not have a response for forbidden, create a new one and add it
            if (!operation.Responses.TryGetValue("403", out _))
            {
                operation.Responses["403"] = new OpenApiResponse { Description = "Forbidden" };
            }
        }
    }
}
