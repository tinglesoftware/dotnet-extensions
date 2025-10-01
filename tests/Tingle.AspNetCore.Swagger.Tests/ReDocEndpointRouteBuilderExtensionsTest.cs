using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net;

namespace Tingle.AspNetCore.Swagger.Tests;

public class ReDocEndpointRouteBuilderExtensionsTest
{
    private static TestServer CreateTestServer(Action<WebApplicationBuilder>? configureBuilder = null, Action<WebApplication>? configureApp = null)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        configureBuilder?.Invoke(builder);

        var app = builder.Build();
        configureApp?.Invoke(app);

        _ = app.StartAsync(); // Start the app asynchronously
        return app.GetTestServer();
    }

    [Fact]
    public void ThrowFriendlyErrorForWrongPathFormat()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
        {
            using var server = CreateTestServer(
                builder =>
                {
                    builder.Services.AddRouting();
                    builder.Services.AddReDoc();
                },
                app =>
                {
                    app.UseRouting();
                    app.MapReDoc("/docs/{documentNam}");
                });
        });

        Assert.Equal(
            "The pattern must contain '{documentName}' parameter." +
            "Try something similar to '/docs/{documentName=v1}' (Parameter 'pattern')",
            ex.Message);
    }

    [Fact] // Matches based on '.Map'
    public async Task IgnoresRequestThatDoesNotMatchPath()
    {
        using var server = CreateTestServer(
            builder =>
            {
                builder.Services.AddRouting();
                builder.Services.AddReDoc();
            },
            app =>
            {
                app.UseRouting();
                app.MapReDoc("/docs/{documentName=v1}");
            });
        var client = server.CreateClient();

        var response = await client.GetAsync("/frob", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact] // Matches based on '.Map'
    public async Task MatchIsCaseInsensitive()
    {
        using var server = CreateTestServer(
            builder =>
            {
                builder.Services.AddRouting();
                builder.Services.AddReDoc();
            },
            app =>
            {
                app.UseRouting();
                app.MapReDoc();
            });
        var client = server.CreateClient();

        var response = await client.GetAsync("/DOCS/v1", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.ToString());
        Assert.NotEqual(0, response.Content.Headers.ContentLength);
    }

    [Fact] // Matches based on '.Map'
    public async Task DefaultPathIsUsed()
    {
        using var server = CreateTestServer(
            builder =>
            {
                builder.Services.AddRouting();
                builder.Services.AddReDoc();
            },
            app =>
            {
                app.UseRouting();
                app.MapReDoc("/docs/{documentName=v1}");
            });
        var client = server.CreateClient();

        var response = await client.GetAsync("/DOCS/v1", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.ToString());
        Assert.NotEqual(0, response.Content.Headers.ContentLength);
    }

    [Fact] // Matches based on '.Map'
    public async Task DefaultDocumentNameIsUsed()
    {
        using var server = CreateTestServer(
            builder =>
            {
                builder.Services.AddRouting();
            },
            app =>
            {
                app.UseRouting();
                app.MapReDoc("/docs/{documentName=v2}");
            });
        var client = server.CreateClient();

        var response = await client.GetAsync("/docs", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.ToString());
        Assert.Contains("/swagger/v2/swagger.json", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact] // Matches based on '.Map'
    public async Task DefaultDocumentNameIsNotUsed()
    {
        using var server = CreateTestServer(
            builder =>
            {
                builder.Services.AddRouting();
            },
            app =>
            {
                app.UseRouting();
                app.MapReDoc("/docs/{documentName=v2}");
            });
        var client = server.CreateClient();

        var response = await client.GetAsync("/docs/v1", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.ToString());
        Assert.NotEqual(0, response.Content.Headers.ContentLength);
        Assert.Contains("/swagger/v1/swagger.json", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task StatusCodeIs404IfNotGet()
    {
        using var server = CreateTestServer(
            builder =>
            {
                builder.Services.AddRouting();
                builder.Services.AddReDoc();
            },
            app =>
            {
                app.UseRouting();
                app.MapReDoc();
            });
        var client = server.CreateClient();

        var response = await client.DeleteAsync("/docs/v1", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);

        response = await client.PostAsync("/docs/v1", new StringContent(""), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        Assert.Equal(0, response.Content.Headers.ContentLength);
    }

    [Fact]
    public async Task MapReDoc_ReturnsOk()
    {
        // Arrange
        using var server = CreateTestServer(
            builder =>
            {
                builder.Services.AddRouting();
                builder.Services.AddReDoc(options =>
                {
                    options.Config.ShowExtensions = new List<string> { "x-cake" };
                });
            },
            app =>
            {
                app.UseRouting();
                app.MapReDoc();
            });
        var client = server.CreateClient();

        // Act
        var response = await client.GetAsync("/docs/v1", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.ToString());
        Assert.NotEqual(0, response.Content.Headers.ContentLength);
    }
}
