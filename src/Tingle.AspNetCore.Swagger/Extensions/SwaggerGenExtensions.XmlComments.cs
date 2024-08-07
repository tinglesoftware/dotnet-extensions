using Swashbuckle.AspNetCore.SwaggerGen;
using Tingle.AspNetCore.Swagger.Filters.Schemas;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="SwaggerGenOptions"/>
/// </summary>
public static partial class SwaggerGenExtensions
{
    /// <summary>
    /// Adds XML comments from the assembly of the specified type
    /// </summary>
    /// <typeparam name="T">the type where to get the assembly</typeparam>
    /// <param name="options"></param>
    /// <param name="includeControllerXmlComments">
    /// Flag to indicate if controller XML comments (i.e. summary) should be used to
    /// assign Tag descriptions. Don't set this flag if you're customizing the default
    /// tag for operations via TagActionsBy.
    /// </param>
    /// <returns></returns>
    public static SwaggerGenOptions IncludeXmlComments<T>(this SwaggerGenOptions options, bool includeControllerXmlComments = false)
    {
        options.IncludeXmlComments(typeof(T).Assembly, includeControllerXmlComments);
        return options;
    }

    /// <summary>
    /// Add descriptions to schemas based on &lt;inheritdoc/&gt; XML summary comments.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="excludedTypes">Types for which inheritance will be excluded.</param>
    /// <returns></returns>
    /// <seealso cref="InheritDocSchemaFilter"/>
    public static SwaggerGenOptions IncludeXmlCommentsFromInheritDocs(this SwaggerGenOptions options, params Type[] excludedTypes)
    {
        var distinctExcludedTypes = excludedTypes?.Distinct().ToArray();
        options.SchemaFilter<InheritDocSchemaFilter>(options, distinctExcludedTypes);
        return options;
    }
}
