using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using Tingle.AspNetCore.Swagger;
using Tingle.AspNetCore.Swagger.Filters.Documents;
using Tingle.AspNetCore.Swagger.Filters.Operations;
using Tingle.AspNetCore.Swagger.Filters.Parameters;
using Tingle.AspNetCore.Swagger.Filters.Schemas;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="SwaggerGenOptions"/>
/// </summary>
public static partial class SwaggerGenExtensions
{
    /// <summary>
    /// Always include a description for BadRequest (400) responses
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static SwaggerGenOptions AlwaysShowBadRequestResponse(this SwaggerGenOptions options)
    {
        options.OperationFilter<BadRequestOperationFilter>();
        return options;
    }

    /// <summary>
    /// Always include a description for Unauthorized (401) and Forbidden (403) responses
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static SwaggerGenOptions AlwaysShowAuthorizationFailedResponse(this SwaggerGenOptions options)
    {
        options.OperationFilter<AuthorizationOperationFilter>();
        return options;
    }

    /// <summary>
    /// Add extra tags to operations
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static SwaggerGenOptions AddExtraTags(this SwaggerGenOptions options)
    {
        options.OperationFilter<ExtraTagsOperationFilter>();
        return options;
    }

    /// <summary>
    /// Adds <c>x-internal</c> when <see cref="InternalOnlyAttribute" /> is used
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static SwaggerGenOptions AddInternalOnlyExtensions(this SwaggerGenOptions options)
    {
        options.ParameterFilter<InternalOnlyParameterFilter>();
        options.SchemaFilter<InternalOnlySchemaFilter>();
        options.OperationFilter<InternalOnlyOperationFilter>();
        return options;
    }

    /// <summary>
    /// Add error codes to operations and the document using the vendor extension <c>x-error-codes</c>
    /// </summary>
    /// <param name="options"></param>
    /// <param name="descriptions">
    /// The descriptions for error codes.
    /// The key (<see cref="KeyValuePair{TKey, TValue}.Key"/>) represents the error code whereas
    /// the value (<see cref="KeyValuePair{TKey, TValue}.Value"/>) represents the description.
    /// </param>
    /// <returns></returns>
    /// <seealso cref="ErrorCodesDocumentFilter" />
    /// <seealso cref="ErrorCodesOperationFilter" />
    public static SwaggerGenOptions AddErrorCodes(this SwaggerGenOptions options, IDictionary<string, string>? descriptions = null)
    {
        descriptions ??= new Dictionary<string, string>();
        options.DocumentFilter<ErrorCodesDocumentFilter>(descriptions);
        options.OperationFilter<ErrorCodesOperationFilter>();
        return options;
    }

    /// <summary>
    /// Add CorrelationId headers to all operations
    /// </summary>
    /// <param name="options"></param>
    /// <param name="includeInRequests">
    /// Flag to indicate if the correlation header (<c>X-Correlation-ID</c>) should be added to an operation's parameters.
    /// </param>
    /// <returns></returns>
    /// <seealso cref="CorrelationIdDocumentFilter" />
    /// <seealso cref="CorrelationIdOperationFilter" />
    public static SwaggerGenOptions AddCorrelationIds(this SwaggerGenOptions options, bool includeInRequests = false)
    {
        options.DocumentFilter<CorrelationIdDocumentFilter>();
        options.OperationFilter<CorrelationIdOperationFilter>(includeInRequests);
        return options;
    }

    /// <summary>
    /// Add a groupings for tags to the document using the vendor extension <c>x-tagGroups</c>
    /// </summary>
    /// <param name="options"></param>
    /// <param name="groups">The tag groups.</param>
    /// <param name="addUngrouped">Whether to add a group for ungrouped tags named <c>Ungrouped</c></param>
    /// <returns></returns>
    /// <seealso cref="TagGroupsDocumentFilter" />
    public static SwaggerGenOptions AddTagGroups(this SwaggerGenOptions options, IEnumerable<OpenApiTagGroup> groups, bool addUngrouped = false)
    {
        options.DocumentFilter<TagGroupsDocumentFilter>(groups, addUngrouped);
        return options;
    }

    /// <summary>
    /// Map expected schemas for known types in the framework and from Tingle primitives. These are:
    /// <list type="bullet">
    /// <item><see cref="TimeSpan"/></item>
    /// <item><see cref="System.Net.IPAddress"/></item>
    /// <item><see cref="System.Text.Json.Nodes.JsonNode"/></item>
    /// <item><see cref="System.Text.Json.Nodes.JsonObject"/></item>
    /// <item><see cref="System.Text.Json.Nodes.JsonArray"/></item>
    /// <item><see cref="System.Text.Json.JsonElement"/></item>
    /// <item><see cref="Tingle.Extensions.Primitives.Duration"/></item>
    /// <item><see cref="Tingle.Extensions.Primitives.Etag"/></item>
    /// <item><see cref="Tingle.Extensions.Primitives.ByteSize"/></item>
    /// <item><see cref="Tingle.Extensions.Primitives.Ksuid"/></item>
    /// <item><see cref="Tingle.Extensions.Primitives.SequenceNumber"/></item>
    /// <item><see cref="Tingle.Extensions.Primitives.Continent"/></item>
    /// <item><see cref="Tingle.Extensions.Primitives.Currency"/></item>
    /// <item><see cref="Tingle.Extensions.Primitives.SwiftCode"/></item>
    /// </list>
    /// Add a groupings for tags to the document using the vendor extension <c>x-tagGroups</c>
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static SwaggerGenOptions MapFrameworkTypes(this SwaggerGenOptions options)
    {
        options.MapType<System.Net.IPAddress>(() => new OpenApi.Models.OpenApiSchema { Type = "string", Format = "ip-address", });
        options.MapType<System.Net.IPNetwork>(() => new OpenApi.Models.OpenApiSchema { Type = "string", Format = "cidr", });
        options.MapType<System.Text.Json.Nodes.JsonNode>(() => new OpenApi.Models.OpenApiSchema { });
        options.MapType<System.Text.Json.Nodes.JsonObject>(() => new OpenApi.Models.OpenApiSchema { Type = "object", AdditionalProperties = new OpenApi.Models.OpenApiSchema(), });
        options.MapType<System.Text.Json.Nodes.JsonArray>(() => new OpenApi.Models.OpenApiSchema { Type = "array", });
        options.MapType<System.Text.Json.JsonElement>(() => new OpenApi.Models.OpenApiSchema { Type = "object", AdditionalProperties = new OpenApi.Models.OpenApiSchema(), });

        options.MapType<Tingle.Extensions.Primitives.Duration>(() => new OpenApi.Models.OpenApiSchema { Type = "string", Format = "duration", });
        options.MapType<Tingle.Extensions.Primitives.Etag>(() => new OpenApi.Models.OpenApiSchema { Type = "string", });
        options.MapType<Tingle.Extensions.Primitives.ByteSize>(() => new OpenApi.Models.OpenApiSchema { Type = "string", });
        options.MapType<Tingle.Extensions.Primitives.Ksuid>(() => new OpenApi.Models.OpenApiSchema { Type = "string", });
        options.MapType<Tingle.Extensions.Primitives.SequenceNumber>(() => new OpenApi.Models.OpenApiSchema { Type = "string", Format = "int64", });
        options.MapType<Tingle.Extensions.Primitives.Continent>(() => new OpenApi.Models.OpenApiSchema { Type = "string", });
        options.MapType<Tingle.Extensions.Primitives.Currency>(() => new OpenApi.Models.OpenApiSchema { Type = "string", Format = "currency", });
        options.MapType<Tingle.Extensions.Primitives.Country>(() => new OpenApi.Models.OpenApiSchema { Type = "string", Format = "country", MinLength = 2, MaxLength = 3, });
        options.MapType<Tingle.Extensions.Primitives.Language>(() => new OpenApi.Models.OpenApiSchema { Type = "string", Format = "language", MinLength = 2, MaxLength = 3, });
        options.MapType<Tingle.Extensions.Primitives.SwiftCode>(() => new OpenApi.Models.OpenApiSchema { Type = "string", Format = "swift-code", MinLength = 11, MaxLength = 11, });

        return options;
    }
}
