using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Tingle.AspNetCore.JsonPatch;

public class JsonPatchMergeDocumentProviderTests
{
    [Fact]
    public void OnProvidersExecuting_FindsJsonPatchDocuments_ProvidesOperationsArray()
    {
        // Arrange
        var metadataProvider = new EmptyModelMetadataProvider();
        var provider = new JsonPatchMergeDocumentProvider(metadataProvider);
        var jsonPatchMergeParameterDescription = new ApiParameterDescription
        {
            Type = typeof(JsonPatchMergeDocument<Customer>)
        };

        var stringParameterDescription = new ApiParameterDescription
        {
            Type = typeof(string),
        };

        var apiDescription = new ApiDescription();
        apiDescription.ParameterDescriptions.Add(jsonPatchMergeParameterDescription);
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
                Assert.Equal(typeof(Customer), description.Type);
                Assert.Equal(typeof(Customer), description.ModelMetadata.ModelType);
            },
            description =>
            {
                Assert.Equal(typeof(string), description.Type);
            });
    }

    private class Customer
    {
        public string? CustomerName { get; set; }
    }
}
