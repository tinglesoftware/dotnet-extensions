namespace Tingle.Extensions.Http.Authentication.Tests;

public class ApiKeyHandlersTests
{
    [Fact]
    public async Task ApiKeyHeader_Works()
    {
        // Prepare
        var inner = new DynamicHttpMessageHandler();
        var handler = new ApiKeyHeaderAuthenticationHandler("some-key-is-usually-set", inner);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "https://apis.example.com/v1/cars");
        var client = new HttpClient(handler);
        await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var header = request.Headers.Authorization;
        Assert.NotNull(header);
        Assert.Equal("ApiKey", header!.Scheme);
        Assert.Equal("some-key-is-usually-set", header.Parameter);
    }

    [Fact]
    public async Task ApiKeyQuery_Works_AddsQuery()
    {
        // Prepare
        var inner = new DynamicHttpMessageHandler();
        var handler = new ApiKeyQueryAuthenticationHandler("some#funny:key", inner);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "https://apis.example.com/v1/cars");
        var client = new HttpClient(handler);
        await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var uri = request.RequestUri?.ToString();
        Assert.NotNull(uri);
        Assert.EndsWith("?auth=some%23funny%3akey", uri);
    }

    [Fact]
    public async Task ApiKeyQuery_Works_AppendsToQuery()
    {
        // Prepare
        var inner = new DynamicHttpMessageHandler();
        var handler = new ApiKeyQueryAuthenticationHandler("some#funny:key", inner);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "https://apis.example.com/v1/cars?c2bOnly=true");
        var client = new HttpClient(handler);
        await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var uri = request.RequestUri?.ToString();
        Assert.NotNull(uri);
        Assert.EndsWith("&auth=some%23funny%3akey", uri);
    }
}
