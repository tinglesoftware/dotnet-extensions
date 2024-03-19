using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tingle.AspNetCore.Swagger.Filters.Schemas;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="SwaggerGenOptions"/>
/// </summary>
public static partial class SwaggerGenExtensions
{
    /// <summary>
    /// Adds XML comments from a file in a certain directory.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="directory">the directory where the files exists</param>
    /// <param name="fileName">the name of the file including the extension</param>
    /// <param name="includeControllerXmlComments">
    /// Flag to indicate if controller XML comments (i.e. summary) should be used to
    /// assign Tag descriptions. Don't set this flag if you're customizing the default
    /// tag for operations via TagActionsBy.
    /// </param>
    /// <param name="checkExists">Whether to check if the file exists prior to adding.</param>
    /// <returns></returns>
    public static SwaggerGenOptions IncludeXmlComments(this SwaggerGenOptions options,
                                                       string directory,
                                                       string fileName,
                                                       bool includeControllerXmlComments = false,
                                                       bool checkExists = false)
    {
        var filePath = Path.Combine(directory, fileName);
        if (checkExists && !File.Exists(filePath)) return options;

        options.IncludeXmlComments(filePath: filePath,
                                   includeControllerXmlComments: includeControllerXmlComments);
        return options;
    }

    /// <summary>
    /// Adds XML comments from a file in a certain directory
    /// </summary>
    /// <param name="options"></param>
    /// <param name="environment">The <see cref="IHostEnvironment"/> in which the application is running.</param>
    /// <param name="fileName">the name of the file including the extension</param>
    /// <param name="includeControllerXmlComments">
    /// Flag to indicate if controller XML comments (i.e. summary) should be used to
    /// assign Tag descriptions. Don't set this flag if you're customizing the default
    /// tag for operations via TagActionsBy.
    /// </param>
    /// <param name="checkExists">Whether to check if the file exists prior to adding.</param>
    /// <returns></returns>
    public static SwaggerGenOptions IncludeXmlComments(this SwaggerGenOptions options,
                                                       IHostEnvironment environment,
                                                       string fileName,
                                                       bool includeControllerXmlComments = false,
                                                       bool checkExists = false)
    {
        var filePath = Path.Combine(environment.ContentRootPath, fileName);

        if (File.Exists(filePath))
        {
            options.IncludeXmlComments(filePath: filePath, includeControllerXmlComments: includeControllerXmlComments);
            return options;
        }

        // During debug, the file will be not be present in ContentRootPath so we use the AppDomain instead
        return options.IncludeXmlComments(directory: AppDomain.CurrentDomain.BaseDirectory,
                                          fileName: fileName,
                                          includeControllerXmlComments: includeControllerXmlComments,
                                          checkExists: checkExists);
    }

    /// <summary>
    /// Adds XML comments from the assembly of the specified type
    /// </summary>
    /// <typeparam name="T">the type where to get the assembly</typeparam>
    /// <param name="options"></param>
    /// <param name="environment">The <see cref="IHostEnvironment"/> in which the application is running.</param>
    /// <param name="includeControllerXmlComments">
    /// Flag to indicate if controller XML comments (i.e. summary) should be used to
    /// assign Tag descriptions. Don't set this flag if you're customizing the default
    /// tag for operations via TagActionsBy.
    /// </param>
    /// <param name="checkExists">Whether to check if the file exists prior to adding.</param>
    /// <returns></returns>
    public static SwaggerGenOptions IncludeXmlComments<T>(this SwaggerGenOptions options,
                                                          IHostEnvironment environment,
                                                          bool includeControllerXmlComments = false,
                                                          bool checkExists = false)
    {
        var fileName = $"{typeof(T).Assembly.GetName().Name}.xml";
        return options.IncludeXmlComments(environment: environment,
                                          fileName: fileName,
                                          includeControllerXmlComments: includeControllerXmlComments,
                                          checkExists: checkExists);
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
