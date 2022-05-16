namespace Tingle.Extensions.Http.Authentication.Tests;

public class BasicHeaderHandlerTests
{
    [Fact]
    public async Task BasicHeader_Works()
    {
        // Prepare
        var inner = new DynamicHttpMessageHandler();
        var handler = new BasicHeaderAuthenticationHandler("some-key-is-usually-set", inner);

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "https://apis.example.com/v1/cars");
        var client = new HttpClient(handler);
        await client.SendAsync(request);

        // Assert
        var header = request.Headers.Authorization;
        Assert.NotNull(header);
        Assert.Equal(handler.Scheme, header!.Scheme);
        Assert.Equal("some-key-is-usually-set", header.Parameter);
    }
}
