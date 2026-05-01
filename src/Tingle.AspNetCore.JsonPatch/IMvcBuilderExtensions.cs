using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tingle.AspNetCore.JsonPatch;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Extension methods for <see cref="IMvcBuilder"/>.</summary>
public static class IMvcBuilderExtensions
{
    /// <summary>
    /// Adds JSON Patch and JSON Merge Patch support via <see cref="System.Text.Json"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <returns>The <see cref="IMvcBuilder"/>.</returns>
    public static IMvcBuilder AddJsonPatch(this IMvcBuilder builder)
    {
        return builder.AddJsonMergePatch();
    }

    /// <summary>
    /// Adds JSON Merge Patch support via <see cref="System.Text.Json"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <returns>The <see cref="IMvcBuilder"/>.</returns>
    public static IMvcBuilder AddJsonMergePatch(this IMvcBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var services = builder.Services;

        services.TryAddEnumerable(
            ServiceDescriptor.Transient<IApiDescriptionProvider, JsonMergePatchDocumentProvider>());

        return builder;
    }

    /// <summary>
    /// Adds JSON Merge Patch support via <see cref="System.Text.Json"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <returns>The <see cref="IMvcBuilder"/>.</returns>
    [Obsolete("Use AddJsonMergePatch(builder).", false)]
    public static IMvcBuilder AddJsonPatchMerge(this IMvcBuilder builder)
    {
        return builder.AddJsonMergePatch();
    }
}
