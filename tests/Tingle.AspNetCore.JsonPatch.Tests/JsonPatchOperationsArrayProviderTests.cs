using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Tingle.AspNetCore.JsonPatch.Operations;

namespace Tingle.AspNetCore.JsonPatch;

public class JsonPatchOperationsArrayProviderTests
{
    [Fact]
    public void OnProvidersExecuting_FindsJsonPatchDocuments_ProvidesOperationsArray()
    {
        // Arrange
        var metadataProvider = new EmptyModelMetadataProvider();
        var provider = new JsonPatchOperationsArrayProvider(metadataProvider);
        var jsonPatchParameterDescription = new ApiParameterDescription
        {
            Type = typeof(JsonPatchDocument)
        };

        var stringParameterDescription = new ApiParameterDescription
        {
            Type = typeof(string),
        };

        var apiDescription = new ApiDescription();
        apiDescription.ParameterDescriptions.Add(jsonPatchParameterDescription);
        apiDescription.ParameterDescriptions.Add(stringParameterDescription);

        var actionDescriptorList = new List<ActionDescriptor>();
        var apiDescriptionProviderContext = new ApiDescriptionProviderContext(actionDescriptorList);
        apiDescriptionProviderContext.Results.Add(apiDescription);

        // Act
        provider.OnProvidersExecuting(apiDescriptionProviderContext);

        // Assert
        Assert.Collection(apiDescription.ParameterDescriptions,
            description =>
            {
                Assert.Equal(typeof(Operation[]), description.Type);
                Assert.Equal(typeof(Operation[]), description.ModelMetadata.ModelType);
            },
            description =>
            {
                Assert.Equal(typeof(string), description.Type);
            });
    }
}
