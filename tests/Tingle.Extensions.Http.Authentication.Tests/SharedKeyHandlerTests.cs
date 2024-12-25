using System.Net.Http.Headers;
using System.Text;

namespace Tingle.Extensions.Http.Authentication.Tests;

public class SharedKeyHandlerTests
{
    [Fact]
    public async Task SharedKeyProvider_Works_GET()
    {
        // Prepare
        var inner = new DynamicHttpMessageHandler();
        var handler = new SharedKeyAuthenticationHandler(inner, "TiR0p2ZwnUuBGBEDU5LADWBXpxXy3Y9Aq4Fb1nD+6CM=");

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "https://apis.example.com/v1/search?number=12345678");
        request.Headers.TryAddWithoutValidation("x-ts-date", "Tue, 26 Dec 2017 23:09:28 GMT");
        var client = new HttpClient(handler);
        await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var header = request.Headers.Authorization;
        Assert.NotNull(header);
        Assert.Equal(header!.Scheme, header.Scheme);
        Assert.Equal("07GYCpDpdzsUFIF6IRQ/CfXxeJsj3ZQ+r8UE5XSTAkc=", header.Parameter);
    }

    [Fact]
    public async Task SharedKeyProvider_Works_POST()
    {
        // Prepare
        var inner = new DynamicHttpMessageHandler();
        var handler = new SharedKeyAuthenticationHandler(inner, "TiR0p2ZwnUuBGBEDU5LADWBXpxXy3Y9Aq4Fb1nD+6CM=");

        // Act
        var request = new HttpRequestMessage(HttpMethod.Post, "https://apis.example.com/v1/search?number=12345678");
        request.Headers.TryAddWithoutValidation("x-ts-date", "Tue, 26 Dec 2017 23:09:28 GMT");
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
        request.Content = new StreamContent(stream);
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        var client = new HttpClient(handler);
        await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var header = request.Headers.Authorization;
        Assert.NotNull(header);
        Assert.Equal(handler.Scheme, header!.Scheme);
        Assert.Equal("n/D6O9T39BRyp6L11hsYbjG2efJ2Gdo1hAoWuL+rYi0=", header.Parameter);
    }

    [Fact]
    public async Task SharedKeyProvider_AddsDateHeader()
    {
        // Prepare
        var inner = new DynamicHttpMessageHandler();
        var handler = new SharedKeyAuthenticationHandler(inner, "TiR0p2ZwnUuBGBEDU5LADWBXpxXy3Y9Aq4Fb1nD+6CM=");

        // invoke the authentication provider
        var request = new HttpRequestMessage(HttpMethod.Get, "https://apis.example.com/v1/search?number=12345678");
        var client = new HttpClient(handler);
        await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        IReadOnlyDictionary<string, IEnumerable<string>> dict = request.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        var values = Assert.Contains(handler.DateHeaderName, dict);
        var date = Assert.Single(values);
        Assert.NotNull(date);
        var dto = DateTimeOffset.Parse(date);
        Assert.True(dto < DateTimeOffset.UtcNow);
    }
}
