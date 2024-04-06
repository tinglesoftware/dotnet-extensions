using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Tingle.AspNetCore.JsonPatch;

/// <summary>
/// Implements a provider of <see cref="ApiDescription"/> to change parameters of
/// type <see cref="JsonPatchMergeDocument{TModel}"/> to the model type.
/// </summary>
/// <param name="modelMetadataProvider">The <see cref="IModelMetadataProvider"/>.</param>
internal sealed class JsonPatchMergeDocumentProvider(IModelMetadataProvider modelMetadataProvider) : IApiDescriptionProvider
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
                var parameterType = parameterDescription.Type;
                if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(JsonPatchMergeDocument<>))
                {
                    var modelType = parameterType.GetGenericArguments()[0];

                    parameterDescription.Type = modelType;
                    parameterDescription.ModelMetadata = modelMetadataProvider.GetMetadataForType(modelType);
                }
            }
        }
    }

    /// <inheritdoc />
    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
    }
}
