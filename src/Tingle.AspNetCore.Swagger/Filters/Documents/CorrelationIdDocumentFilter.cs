using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tingle.AspNetCore.Swagger.Filters.Documents;

/// <summary>
/// Adds an <see cref="OpenApiHeader"/> to a <see cref="OpenApiDocument"/> with a description of the <c>X-Correlation-ID</c> header.
/// </summary>
/// <seealso cref="IDocumentFilter" />
public class CorrelationIdDocumentFilter : IDocumentFilter
{
    internal const string HeaderName = "X-Correlation-ID";
    internal const string HeaderDescription = "Used to uniquely identify the HTTP request. This ID is used to correlate the HTTP request between a client and server.";
    internal const string HeaderExample = "00-982607166a542147b435be3a847ddd71-fc75498eb9f09d48-00";

    /// <inheritdoc/>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Components.Headers[HeaderName] = new OpenApiHeader
        {
            Description = HeaderDescription,
            Schema = new OpenApiSchema { Type = "string", },
            Example = new OpenApiString(HeaderExample),
        };

        swaggerDoc.Components.Parameters[HeaderName] = new OpenApiParameter
        {
            Description = HeaderDescription,
            In = ParameterLocation.Header,
            Name = HeaderName,
            Required = false,
            Schema = new OpenApiSchema { Type = "string", },
            Example = new OpenApiString(HeaderExample),
        };
    }
}
