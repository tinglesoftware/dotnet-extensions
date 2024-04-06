using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;
using Tingle.AspNetCore.JsonPatch.Operations;

namespace Tingle.AspNetCore.JsonPatch;

/// <summary>
/// Implements a provider of <see cref="ApiDescription"/> to change parameters of
/// type <see cref="IJsonPatchDocument"/> to an array of <see cref="Operation"/>.
/// </summary>
/// <param name="modelMetadataProvider">The <see cref="IModelMetadataProvider"/>.</param>
internal sealed class JsonPatchOperationsArrayProvider(IModelMetadataProvider modelMetadataProvider) : IApiDescriptionProvider
{
    /// <inheritdoc />
    /// <remarks>
    /// The order -999 ensures that this provider is executed right after the <c>Microsoft.AspNetCore.Mvc.ApiExplorer.DefaultApiDescriptionProvider</c>.
    /// </remarks>
    public int Order => -999;

    /// <inheritdoc />
    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        foreach (var result in context.Results)
        {
            foreach (var parameterDescription in result.ParameterDescriptions)
            {
                if (typeof(IJsonPatchDocument).GetTypeInfo().IsAssignableFrom(parameterDescription.Type))
                {
                    parameterDescription.Type = typeof(Operation[]);
                    parameterDescription.ModelMetadata = modelMetadataProvider.GetMetadataForType(typeof(Operation[]));
                }
            }
        }
    }

    /// <inheritdoc />
    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
    }
}
