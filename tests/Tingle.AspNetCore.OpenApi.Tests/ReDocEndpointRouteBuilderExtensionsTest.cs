﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net;

namespace Tingle.AspNetCore.OpenApi.Tests;

public class ReDocEndpointRouteBuilderExtensionsTest
{
    [Fact]
    public void ThrowFriendlyErrorForWrongPathFormat()
    {
        var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapReDoc("/docs/{documentNam}");
                });
            })
            .ConfigureServices(services =>
            {
                services.AddRouting();
                services.AddReDoc();
            });

        var ex = Assert.Throws<ArgumentException>(() => new TestServer(builder));

        Assert.Equal(
            "The pattern must contain '{documentName}' parameter." +
            "Try something similar to '/docs/{documentName=v1}' (Parameter 'pattern')",
            ex.Message);
    }

    [Fact] // Matches based on '.Map'
    public async Task IgnoresRequestThatDoesNotMatchPath()
    {
        var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapReDoc("/docs/{documentName=v1}");
                });
            })
            .ConfigureServices(services =>
            {
                services.AddRouting();
                services.AddReDoc();
            });
        using var server = new TestServer(builder);
        var client = server.CreateClient();

        var response = await client.GetAsync("/frob", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact] // Matches based on '.Map'
    public async Task MatchIsCaseInsensitive()
    {
        var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapReDoc();
                });
            })
            .ConfigureServices(services =>
            {
                services.AddRouting();
                services.AddReDoc();
            });
        using var server = new TestServer(builder);
        var client = server.CreateClient();

        var response = await client.GetAsync("/DOCS/v1", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.ToString());
        Assert.NotEqual(0, response.Content.Headers.ContentLength);
    }

    [Fact] // Matches based on '.Map'
    public async Task DefaultPathIsUsed()
    {
        var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapReDoc("/docs/{documentName=v1}");
                });
            })
            .ConfigureServices(services =>
            {
                services.AddRouting();
                services.AddReDoc();
            });
        using var server = new TestServer(builder);
        var client = server.CreateClient();

        var response = await client.GetAsync("/DOCS/v1", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.ToString());
        Assert.NotEqual(0, response.Content.Headers.ContentLength);
    }

    [Fact] // Matches based on '.Map'
    public async Task DefaultDocumentNameIsUsed()
    {
        var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapReDoc("/docs/{documentName=v2}");
                });
            })
            .ConfigureServices(services =>
            {
                services.AddRouting();
            });
        using var server = new TestServer(builder);
        var client = server.CreateClient();

        var response = await client.GetAsync("/docs", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.ToString());
        Assert.Contains("/openapi/v2.json", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact] // Matches based on '.Map'
    public async Task DefaultDocumentNameIsNotUsed()
    {
        var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapReDoc("/docs/{documentName=v2}");
                });
            })
            .ConfigureServices(services =>
            {
                services.AddRouting();
            });
        using var server = new TestServer(builder);
        var client = server.CreateClient();

        var response = await client.GetAsync("/docs/v1", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.ToString());
        Assert.NotEqual(0, response.Content.Headers.ContentLength);
        Assert.Contains("/openapi/v1.json", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task StatusCodeIs404IfNotGet()
    {
        var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapReDoc();
                });
            })
            .ConfigureServices(services =>
            {
                services.AddRouting();
                services.AddReDoc();
            });
        using var server = new TestServer(builder);
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
        var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapReDoc();
                });
            })
            .ConfigureServices(services =>
            {
                services.AddRouting();
                services.AddReDoc(options =>
                {
                    options.Config.ShowExtensions = new List<string> { "x-cake" };
                });
            });
        using var server = new TestServer(builder);
        var client = server.CreateClient();

        // Act
        var response = await client.GetAsync("/docs/v1", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.ToString());
        Assert.NotEqual(0, response.Content.Headers.ContentLength);
    }
}
