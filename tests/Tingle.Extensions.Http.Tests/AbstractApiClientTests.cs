using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

namespace Tingle.Extensions.Http.Tests;

public class AbstractApiClientTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public async Task ExtractResponseAsync_Get_ApplicationJson_Works()
    {
        var cancellationToken = CancellationToken.None;
        var client = CreateClient();

        // make first request -> OK with application/json
        var response = await client.SendTestOkJsonAppAsync(cancellationToken);
        Assert.NotNull(response);
        Assert.Null(response.Problem);
        var resource = Assert.IsAssignableFrom<TestResource>(response.Resource);
        Assert.Equal("who", resource.Name1);
        Assert.Equal("me", resource.Name2);
    }

    [Fact]
    public async Task ExtractResponseAsync_Get_TextJson_Works()
    {
        var cancellationToken = CancellationToken.None;
        var client = CreateClient();

        // make request -> OK with text/json
        var response = await client.SendTestOkTextJsonAsync(cancellationToken);
        Assert.NotNull(response);
        Assert.Null(response.Problem);
        var resource = Assert.IsAssignableFrom<TestResource>(response.Resource);
        Assert.Equal("who", resource.Name1);
        Assert.Equal("me", resource.Name2);
    }

    [Fact]
    public async Task ExtractResponseAsync_Post_ApplicationJson_Works()
    {
        var cancellationToken = CancellationToken.None;
        var client = CreateClient();

        // make (POST) request -> OK with application/json
        var rr1 = new TestResource { Name1 = "jane", Name2 = "peters" };
        var response = await client.SendTestOkPostJsonAsync(rr1, cancellationToken);
        Assert.NotNull(response);
        Assert.Null(response.Problem);
        var resource = Assert.IsAssignableFrom<TestResource>(response.Resource);
        Assert.Equal("jane", resource.Name1);
        Assert.Equal("peters", resource.Name2);
    }

    [Fact]
    public async Task ExtractResponseAsync_Put_ApplicationJson_Works()
    {
        var cancellationToken = CancellationToken.None;
        var client = CreateClient();

        // make (PUT) request -> OK with application/json
        var rr1 = new TestResource { Name1 = "jane", Name2 = "peters" };
        var response = await client.SendTestOkPutJsonAsync(rr1, cancellationToken);
        Assert.NotNull(response);
        Assert.Null(response.Problem);
        var resource = Assert.IsAssignableFrom<TestResource>(response.Resource);
        Assert.Equal("jane", resource.Name1);
        Assert.Equal("peters", resource.Name2);
    }

    private TestApiClient CreateClient(Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddXUnit(outputHelper));
        services.AddHttpApiClient<TestApiClient, TestApiClientOptions>()
                .ConfigurePrimaryHttpMessageHandler(() => new TestHttpMessageHandler())
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("http://localhost/"));

        configure?.Invoke(services);

        var provider = services.BuildServiceProvider(validateScopes: true);
        var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;
        return sp.GetRequiredService<TestApiClient>();
    }

    class TestResource
    {
        public string? Name1 { get; set; }
        public string? Name2 { get; set; }
    }

    class TestHttpMessageHandler : HttpMessageHandler
    {
        private static readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var resource = new TestResource { Name1 = "who", Name2 = "me" };
            var response = new HttpResponseMessage();
            var path = request.RequestUri?.AbsolutePath;
            switch (path)
            {
                case "/test/ok/json-app":
                    {
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        response.Content = new StringContent(JsonSerializer.Serialize(resource, serializerOptions),
                                                             Encoding.UTF8,
                                                             "application/json");
                        break;
                    }
                case "/test/ok/text-json":
                    {
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        response.Content = new StringContent(JsonSerializer.Serialize(resource, serializerOptions),
                                                             Encoding.UTF8,
                                                             "text/json");
                        break;
                    }
                case "/test/ok-post/json-app":
                    {
                        var js = await request.Content!.ReadAsStringAsync(cancellationToken);
                        var r = JsonSerializer.Deserialize<TestResource>(js, serializerOptions);
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        response.Content = new StringContent(JsonSerializer.Serialize(r, serializerOptions),
                                                             Encoding.UTF8,
                                                             "application/json");
                        break;
                    }
                case "/test/ok-put/json-app":
                    {
                        var js = await request.Content!.ReadAsStringAsync(cancellationToken);
                        var r = JsonSerializer.Deserialize<TestResource>(js, serializerOptions);
                        response.StatusCode = System.Net.HttpStatusCode.OK;
                        response.Content = new StringContent(JsonSerializer.Serialize(r, serializerOptions),
                                                             Encoding.UTF8,
                                                             "application/json");
                        break;
                    }
                default: throw new NotImplementedException($"'{path}' is not supported");
            }

            return response;
        }
    }

    class TestApiClient(HttpClient httpClient, IOptionsSnapshot<TestApiClientOptions> optionsAccessor) : AbstractHttpApiClient<TestApiClientOptions>(httpClient, optionsAccessor)
    {
        public Task<ResourceResponse<TestResource>> SendTestOkJsonAppAsync(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/test/ok/json-app");
            return SendAsync<TestResource>(request, cancellationToken);
        }

        public Task<ResourceResponse<TestResource>> SendTestOkTextJsonAsync(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/test/ok/text-json");
            return SendAsync<TestResource>(request, cancellationToken);
        }

        public async Task<ResourceResponse<TestResource>> SendTestOkPostJsonAsync(TestResource rr1, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/test/ok-post/json-app") { Content = MakeJsonContent(rr1), };
            return await SendAsync<TestResource>(request, cancellationToken);
        }

        public async Task<ResourceResponse<TestResource>> SendTestOkPutJsonAsync(TestResource rr1, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "/test/ok-put/json-app") { Content = MakeJsonContent(rr1), };
            return await SendAsync<TestResource>(request, cancellationToken);
        }
    }

    public class TestApiClientOptions : AbstractHttpApiClientOptions
    {

    }
}
