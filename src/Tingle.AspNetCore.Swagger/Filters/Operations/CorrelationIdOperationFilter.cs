using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tingle.AspNetCore.Swagger.Filters.Documents;

namespace Tingle.AspNetCore.Swagger.Filters.Operations;

/// <summary>
/// Adds an <see cref="OpenApiHeader"/> to all instances of <see cref="OpenApiResponse"/> in an
/// operation with a description of the <c>X-Correlation-ID</c> header.
/// </summary>
/// <seealso cref="IOperationFilter" />
/// <param name="includeInRequests">
/// Flag to indicate if the correlation header (<c>X-Correlation-ID</c>) should be added to an operation's parameters.
/// </param>
public class CorrelationIdOperationFilter(bool includeInRequests) : IOperationFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (includeInRequests)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Reference = new OpenApiReference
                {
                    Id = CorrelationIdDocumentFilter.HeaderName,
                    Type = ReferenceType.Parameter,
                }
            });
        }

        foreach (var kvp in operation.Responses)
        {
            var r = kvp.Value;
            r.Headers[CorrelationIdDocumentFilter.HeaderName] = new OpenApiHeader
            {
                Reference = new OpenApiReference
                {
                    Id = CorrelationIdDocumentFilter.HeaderName,
                    Type = ReferenceType.Header,
                }
            };
        }
    }
}
