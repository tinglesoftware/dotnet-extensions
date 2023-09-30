using System.Text.Json;

namespace Tingle.Extensions.Http.Tests;

public class HttpApiResponseProblemTests
{
    [Fact]
    public void Deserialize_Works()
    {
        var json = @"{""errors"": {""tenantId"": [""The property at path '/TenantId' is immutable.""]},""type"": ""https://tools.ietf.org/html/rfc7231#section-6.5.1"",""title"": ""One or more validation errors occurred."",""status"": 400,""traceId"": ""0HLVG7T99R7S9""}";

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var problem = JsonSerializer.Deserialize<HttpApiResponseProblem>(json, options);
        Assert.NotNull(problem);
        Assert.Equal("One or more validation errors occurred.", problem!.Title);
        Assert.Null(problem.Detail);
        Assert.Equal("https://tools.ietf.org/html/rfc7231#section-6.5.1", problem.Type);
        Assert.Equal(400, problem.Status);
        Assert.NotNull(problem.Errors);
        var errors = Assert.Contains("tenantId", problem.Errors);
        var er = Assert.Single(errors);
        Assert.Equal("The property at path '/TenantId' is immutable.", er);
    }

    [Fact]
    public void PrioritizeProblemDetailsOverLegacy()
    {
        var json = @"{""error_code"":""some_code"",""error_description"":""some description"",""title"":""some_title"",""detail"":""some detail""}";

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var problem = JsonSerializer.Deserialize<HttpApiResponseProblem>(json, options);
        Assert.NotNull(problem);
        Assert.Equal("some_title", problem!.Title);
        Assert.Equal("some detail", problem.Detail);
    }
}
