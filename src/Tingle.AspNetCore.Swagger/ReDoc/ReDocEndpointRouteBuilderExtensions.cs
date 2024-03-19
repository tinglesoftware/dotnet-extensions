using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Tingle.AspNetCore.Swagger.ReDoc;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add ReDoc.
/// </summary>
public static class ReDocEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Adds a ReDoc endpoint to the <see cref="IEndpointRouteBuilder"/> with the default pattern..
    /// The default template is '/docs/{documentName=v1}'
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the ReDoc endpoint to.</param>
    /// <param name="setupAction">The action to setup options for the endpoint.</param>
    /// <returns>A convention routes for the ReDoc endpoint.</returns>
    public static IEndpointConventionBuilder MapReDoc(this IEndpointRouteBuilder endpoints,
                                                      Action<ReDocOptions>? setupAction = null)
    {
        return endpoints.MapReDoc("/docs/{documentName=v1}", setupAction);
    }

    /// <summary>
    /// Adds a ReDoc endpoint to the <see cref="IEndpointRouteBuilder"/> with the specified template.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the ReDoc endpoint to.</param>
    /// <param name="pattern">The URL pattern of the ReDoc endpoint. Must include the {documentName} parameter.</param>
    /// <param name="setupAction">The action to setup options for the endpoint.</param>
    /// <returns>A convention routes for the ReDoc endpoint.</returns>
    public static IEndpointConventionBuilder MapReDoc(this IEndpointRouteBuilder endpoints,
                                                      string pattern,
                                                      Action<ReDocOptions>? setupAction = null)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        // ensure pattern contains {documentName}
        if (!RoutePatternFactory.Parse(pattern).Parameters.Any(x => x.Name == "documentName"))
        {
            throw new ArgumentException(
                $"The {nameof(pattern)} must contain '{{documentName}}' parameter."
                + "Try something similar to '/docs/{documentName=v1}'",
                nameof(pattern));
        }

        var builder = endpoints.CreateApplicationBuilder();

        if (setupAction == null)
        {
            // Don't pass options so it can be configured/injected via DI container instead
            builder.UseMiddleware<ReDocMiddleware>();
        }
        else
        {
            // Configure an options instance here and pass directly to the middleware
            var options = new ReDocOptions();
            setupAction.Invoke(options);

            // ensure pattern contains {documentName}
            if (!RoutePatternFactory.Parse(options.SpecUrlTemplate).Parameters.Any(x => x.Name == "documentName"))
            {
                throw new InvalidOperationException(
                    $"The {nameof(options.SpecUrlTemplate)} must contain '{{documentName}}' parameter.");
            }

            builder.UseMiddleware<ReDocMiddleware>(options);
        }

        return endpoints.MapGet(pattern, builder.Build()).WithDisplayName("redoc");
    }
}
