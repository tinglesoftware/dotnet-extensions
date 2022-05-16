using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Tingle.Extensions.Http.Authentication.Tests;

public class OAuthClientCredentialHandlerTests
{
    private const string AuthEndpoint = "https://localhost/oauth/v1";
    private const string CacheKey = "cakes";

    private readonly IMemoryCache cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

    [Fact]
    public async Task OAuthProvider_Works_WithoutCache()
    {
        // Prepare
        var resp = new DummyResponse
        {
            token_type = "Bearer",
            expires_in = "3599",
            expires_on = DateTimeOffset.UtcNow.AddSeconds(3599).ToUnixTimeSeconds().ToString(),
            not_before = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            resource = "http://localhost/",
            access_token = "..."
        };
        var inner = new DynamicHttpMessageHandler();
        var handler = new OAuthClientCredentialHandler(inner, new HttpClient(new DummyHttpClientHandler(() => resp)))
        {
            AuthenticationEndpoint = AuthEndpoint,
            CacheKey = CacheKey,
            ClientId = Guid.NewGuid().ToString(),
            ClientSecret = Guid.NewGuid().ToString(),
            Resource = "https://localhost/",
        };

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "https://apis.example.com/v1/cars");
        var client = new HttpClient(handler);
        await client.SendAsync(request);

        // Assert
        var header = request.Headers.Authorization;
        Assert.NotNull(header);
        Assert.Equal(handler.Scheme, header!.Scheme);
        Assert.Equal(resp.access_token, header.Parameter);
        Assert.Equal(0, Assert.IsType<MemoryCache>(cache).Count);
        Assert.Null(cache.Get<string>(CacheKey));
    }

    [Fact]
    public async Task OAuthProvider_Works_WithoutCacheKey()
    {
        // Prepare
        var resp = new DummyResponse
        {
            token_type = "Bearer",
            expires_in = "3599",
            expires_on = DateTimeOffset.UtcNow.AddSeconds(3599).ToUnixTimeSeconds().ToString(),
            not_before = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            resource = "http://localhost/",
            access_token = "..."
        };
        var inner = new DynamicHttpMessageHandler();
        var handler = new OAuthClientCredentialHandler(inner, new HttpClient(new DummyHttpClientHandler(() => resp)))
        {
            AuthenticationEndpoint = AuthEndpoint,
            Cache = new(cache),
            ClientId = Guid.NewGuid().ToString(),
            ClientSecret = Guid.NewGuid().ToString(),
            Resource = "https://localhost/",
        };

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "https://apis.example.com/v1/cars");
        var client = new HttpClient(handler);
        await client.SendAsync(request);

        // Assert
        var header = request.Headers.Authorization;
        Assert.NotNull(header);
        Assert.Equal(handler.Scheme, header!.Scheme);
        Assert.Equal(resp.access_token, header.Parameter);
        Assert.Equal(0, Assert.IsType<MemoryCache>(cache).Count);
        Assert.Null(cache.Get<string>(CacheKey));
    }

    [Fact]
    public async Task OAuthProvider_Works_WithCache_ExpiresOn()
    {
        // Prepare
        var resp = new DummyResponse
        {
            token_type = "Bearer",
            expires_on = DateTimeOffset.UtcNow.AddSeconds(3599).ToUnixTimeSeconds().ToString(),
            not_before = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            resource = "http://localhost/",
            access_token = "..."
        };
        var inner = new DynamicHttpMessageHandler();
        var handler = new OAuthClientCredentialHandler(inner, new HttpClient(new DummyHttpClientHandler(() => resp)))
        {
            AuthenticationEndpoint = AuthEndpoint,
            Cache = new(cache),
            CacheKey = "cakes",
            ClientId = Guid.NewGuid().ToString(),
            ClientSecret = Guid.NewGuid().ToString(),
            Resource = "https://localhost/",
        };

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "https://apis.example.com/v1/cars");
        var client = new HttpClient(handler);
        await client.SendAsync(request);

        // Assert
        var header = request.Headers.Authorization;
        Assert.NotNull(header);
        Assert.Equal(handler.Scheme, header!.Scheme);
        Assert.Equal(resp.access_token, header.Parameter);
        Assert.Equal(1, Assert.IsType<MemoryCache>(cache).Count);
        Assert.NotNull(cache.Get<string>(CacheKey));
    }

    [Fact]
    public async Task OAuthProvider_Works_WithCache_ExpiresIn()
    {
        // Prepare
        var resp = new DummyResponse
        {
            token_type = "Bearer",
            expires_in = "3599",
            not_before = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            resource = "http://localhost/",
            access_token = "..."
        };
        var inner = new DynamicHttpMessageHandler();
        var handler = new OAuthClientCredentialHandler(inner, new HttpClient(new DummyHttpClientHandler(() => resp)))
        {
            AuthenticationEndpoint = AuthEndpoint,
            Cache = new(cache),
            CacheKey = "cakes",
            ClientId = Guid.NewGuid().ToString(),
            ClientSecret = Guid.NewGuid().ToString(),
            Resource = "https://localhost/",
        };

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "https://apis.example.com/v1/cars");
        var client = new HttpClient(handler);
        await client.SendAsync(request);

        // Assert
        var header = request.Headers.Authorization;
        Assert.NotNull(header);
        Assert.Equal(handler.Scheme, header!.Scheme);
        Assert.Equal(resp.access_token, header.Parameter);
        Assert.Equal(1, Assert.IsType<MemoryCache>(cache).Count);
        Assert.NotNull(cache.Get<string>(CacheKey));
    }

    [Fact]
    public async Task OAuthProvider_BadResponse_Throws_HttpRequestException()
    {
        // Prepare
        var resp = new DummyResponse
        {
            token_type = "Bearer",
            expires_in = "3599",
            not_before = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            resource = "http://localhost/",
            access_token = "..."
        };
        var inner = new DynamicHttpMessageHandler();
        var handler = new OAuthClientCredentialHandler(inner, new HttpClient(new DummyHttpClientHandler(() => resp)))
        {
            AuthenticationEndpoint = AuthEndpoint + "/cakes",
            Cache = new(cache),
            CacheKey = "cakes",
            ClientId = Guid.NewGuid().ToString(),
            ClientSecret = Guid.NewGuid().ToString(),
            Resource = "https://localhost/",
        };

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "https://apis.example.com/v1/cars");
        var client = new HttpClient(handler);
        var ex = await Assert.ThrowsAnyAsync<HttpRequestException>(() => client.SendAsync(request));

        // Assert
        var header = request.Headers.Authorization;
        Assert.Null(header);
        Assert.Equal(0, Assert.IsType<MemoryCache>(cache).Count);
        Assert.Null(cache.Get<string>(CacheKey));
        Assert.StartsWith("Response status code does not indicate success: 400 (Bad Request)", ex.Message);
    }

    struct DummyResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        public string? token_type { get; set; }
        public string? expires_in { get; set; }
        public string? expires_on { get; set; }
        public string? not_before { get; set; }
        public string? resource { get; set; }
        public string? access_token { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    class DummyHttpClientHandler : HttpClientHandler
    {
        private readonly Func<DummyResponse> modelFunc;
        public DummyHttpClientHandler(Func<DummyResponse> modelFunc)
        {
            this.modelFunc = modelFunc ?? throw new ArgumentNullException(nameof(modelFunc));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri?.ToString() == AuthEndpoint)
            {
                Assert.IsType<FormUrlEncodedContent>(request.Content);
                var n = modelFunc();

                var response = new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(n), Encoding.UTF8, "application/json")
                };

                return Task.FromResult(response);
            }
            else
            {
                var response = new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Content = new ReadOnlyMemoryContent(Array.Empty<byte>())
                };
                return Task.FromResult(response);
            }

            throw new NotImplementedException($"'{request.RequestUri}' is not supported");
        }
    }
}
