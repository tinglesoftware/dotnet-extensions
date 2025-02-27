using Microsoft.AspNetCore.OpenApi;
using System.ComponentModel.DataAnnotations;
using Tingle.AspNetCore.OpenApi;
using Tingle.AspNetCore.OpenApi.Transformers.Documents;
using Tingle.AspNetCore.OpenApi.Transformers.Operations;
using Tingle.AspNetCore.OpenApi.Transformers.Schemas;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Extensions for <see cref="OpenApiOptions"/>.</summary>
public static class OpenApiOptionsExtensions
{
    /// <summary>Add logo for ReDoc to the document using the vendor extension <c>x-logo</c>.</summary>
    /// <param name="options"></param>
    /// <param name="logo">The logo details</param>
    /// <returns></returns>
    /// <seealso cref="ReDocLogoDocumentTransformer" />
    public static OpenApiOptions AddReDocLogo(this OpenApiOptions options, OpenApiReDocLogo logo)
        => options.AddDocumentTransformer(new ReDocLogoDocumentTransformer(logo));

    /// <summary>
    /// Add a groupings for tags to the document using the vendor extension <c>x-tagGroups</c>
    /// </summary>
    /// <param name="options"></param>
    /// <param name="groups">The tag groups.</param>
    /// <param name="addUngrouped">Whether to add a group for ungrouped tags named <c>Ungrouped</c></param>
    /// <returns></returns>
    /// <seealso cref="TagGroupsDocumentTransformer" />
    public static OpenApiOptions AddTagGroups(this OpenApiOptions options, IEnumerable<OpenApiTagGroup> groups, bool addUngrouped = false)
        => options.AddDocumentTransformer(new TagGroupsDocumentTransformer(groups, addUngrouped));

    /// <summary>Always include a description for BadRequest (400) responses.</summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static OpenApiOptions AlwaysShowBadRequestResponse(this OpenApiOptions options)
        => options.AddOperationTransformer<BadRequestOperationTransformer>();

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
    /// <seealso cref="ErrorCodesDocumentTransformer" />
    /// <seealso cref="ErrorCodesOperationTransformer" />
    public static OpenApiOptions AddErrorCodes(this OpenApiOptions options, IDictionary<string, string>? descriptions = null)
    {
        descriptions ??= new Dictionary<string, string>();
        options.AddDocumentTransformer(new ErrorCodesDocumentTransformer(descriptions));
        options.AddOperationTransformer<ErrorCodesOperationTransformer>();
        return options;
    }

    /// <summary>
    /// Adds <c>x-internal</c> when <see cref="InternalOnlyAttribute" /> is used
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static OpenApiOptions AddInternalOnlyExtensions(this OpenApiOptions options)
    {
        options.AddSchemaTransformer<InternalOnlySchemaTransformer>();
        options.AddOperationTransformer<InternalOnlyOperationTransformer>();
        return options;
    }
}
