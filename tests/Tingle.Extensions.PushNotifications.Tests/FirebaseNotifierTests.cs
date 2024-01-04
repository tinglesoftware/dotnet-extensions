using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using Tingle.Extensions.PushNotifications.Firebase;
using Tingle.Extensions.PushNotifications.Firebase.Models;
using Xunit.Abstractions;

namespace Tingle.Extensions.PushNotifications.Tests;

public class FirebaseNotifierTests
{
    private readonly ITestOutputHelper outputHelper;

    public FirebaseNotifierTests(ITestOutputHelper outputHelper)
    {
        this.outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
    }

    [Fact]
    public void Resolution_Works()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddXUnit(outputHelper));
        services.AddMemoryCache();
        services.AddFirebaseNotifier(options =>
        {
            options.ProjectId = Guid.NewGuid().ToString();
            options.ClientEmail = Guid.NewGuid().ToString();
            options.TokenUri = Guid.NewGuid().ToString();
            options.PrivateKey = Guid.NewGuid().ToString();
        });

        var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;
        var client = sp.GetRequiredService<FirebaseNotifier>();
    }

    [Fact]
    public void Resolution_Work_WithConfigurationFile()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddXUnit(outputHelper));
        services.AddMemoryCache();
        services.AddFirebaseNotifier(o =>
        {
            using var rsa = RSA.Create();
            using var stream = new MemoryStream();
            System.Text.Json.JsonSerializer.Serialize(stream, new JsonObject
            {
                ["type"] = "service_account",
                ["project_id"] = "dummy-123",
                ["private_key"] = new string(PemEncoding.Write("PRIVATE KEY", rsa.ExportPkcs8PrivateKey())),
                ["client_email"] = "firebase-adminsdk-12346@dummy-123.iam.gserviceaccount.com",
                ["token_uri"] = "https://oauth2.googleapis.com/token",
            });
            stream.Seek(0, SeekOrigin.Begin);

            o.UseConfigurationFromStream(stream);
        });

        var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;
        var client = sp.GetRequiredService<FirebaseNotifier>();
    }

    [Fact]
    public async Task Authentication_IsPopulated()
    {
        var header = (string?)null;
        var handler = new DynamicHttpMessageHandler((request, ct) =>
        {
            if (request.RequestUri == new Uri("https://oauth2.googleapis.com/token"))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = System.Net.Http.Json.JsonContent.Create(new Dictionary<string, object>
                    {
                        ["access_token"] = "stupid_token",
                        ["expires_in"] = "3600",
                    }),
                };
            }

            Interlocked.Exchange(ref header, Assert.Single(request.Headers.GetValues("Authorization")));
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        });

        var key = Guid.NewGuid().ToString();
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddXUnit(outputHelper));
        services.AddMemoryCache();
        services.AddFirebaseNotifier(options =>
        {
            using var rsa = RSA.Create();
            using var stream = new MemoryStream();
            System.Text.Json.JsonSerializer.Serialize(stream, new JsonObject
            {
                ["type"] = "service_account",
                ["project_id"] = "dummy-12346",
                ["private_key"] = new string(PemEncoding.Write("PRIVATE KEY", rsa.ExportPkcs8PrivateKey())),
                ["client_email"] = "firebase-adminsdk-12346@dummy-12346.iam.gserviceaccount.com",
                ["token_uri"] = "https://oauth2.googleapis.com/token",
            });
            stream.Seek(0, SeekOrigin.Begin);

            options.UseConfigurationFromStream(stream);
        }).ConfigurePrimaryHttpMessageHandler(() => handler);

        // override creation of FirebaseAuthenticationHandler to set the backChannel
        services.AddTransient<FirebaseAuthenticationHandler>(provider =>
        {
            return new FirebaseAuthenticationHandler(
                provider.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>(),
                provider.GetRequiredService<Microsoft.Extensions.Options.IOptionsSnapshot<FirebaseNotifierOptions>>(),
                provider.GetRequiredService<ILogger<FirebaseAuthenticationHandler>>(),
                new HttpClient(handler));
        });

        var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;
        var client = sp.GetRequiredService<FirebaseNotifier>();

        var msg = new FirebaseRequestMessage();
        var model = new FirebaseRequest(msg);
        var response = await client.SendAsync(model);
        response.EnsureSuccess();
        Assert.Equal("Bearer stupid_token", header);
    }
}
