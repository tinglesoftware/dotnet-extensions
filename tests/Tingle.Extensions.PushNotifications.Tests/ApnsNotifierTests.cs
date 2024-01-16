using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using Tingle.Extensions.PushNotifications.Apple;
using Tingle.Extensions.PushNotifications.Apple.Models;
using Xunit.Abstractions;

namespace Tingle.Extensions.PushNotifications.Tests;

public class ApnsNotifierTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public void Resolution_Works()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddXUnit(outputHelper));
        services.AddMemoryCache();
        services.AddApnsNotifier(o =>
        {
            o.BundleId = "cake";
            o.PrivateKeyBytes = (keyId) => Task.FromResult(Encoding.UTF8.GetBytes("cake"));
            o.KeyId = "cake";
            o.TeamId = "cake";
        });

        var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;
        var client = sp.GetRequiredService<ApnsNotifier>();
    }

    [Fact]
    public async Task Authentication_IsPopulated()
    {
        var header = (string?)null;
        var handler = new DynamicHttpMessageHandler((request, ct) =>
        {
            Interlocked.Exchange(ref header, request.Headers.Authorization?.ToString());
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        });

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddXUnit(outputHelper));
        services.AddMemoryCache();
        services.AddApnsNotifier(options =>
        {
            options.BundleId = "cake";
            options.PrivateKeyBytes = (keyId) => Task.FromResult(Encoding.UTF8.GetBytes("cake"));
            options.KeyId = "cake";
            options.TeamId = "cake";
        }).ConfigurePrimaryHttpMessageHandler(() => handler);

        var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;
        var cache = sp.GetRequiredService<IMemoryCache>();
        cache.Set("apns:tokens:cake:cake", "cake-token");
        var client = sp.GetRequiredService<ApnsNotifier>();

        var rr = await client.SendAsync(new ApnsMessageHeader { DeviceToken = "cake" }, new ApnsMessageData(new ApnsMessagePayload { }));
        Assert.Equal("bearer cake-token", header);
    }

    [Fact]
    public void ParsePrivateKey_Works()
    {
        using var ecdsa = ECDsa.Create();
        var key = ecdsa.ExportPkcs8PrivateKey();

        // bas64 only
        var parsed = ApnsNotifierOptionsExtensions.ParsePrivateKey(Convert.ToBase64String(key));
        Assert.Equal<byte>(key, parsed); // sequence equal

        // base64 wrapped with headers (PEM)
        parsed = ApnsNotifierOptionsExtensions.ParsePrivateKey(new string(PemEncoding.Write("PRIVATE KEY", key)));
        Assert.Equal<byte>(key, parsed); // sequence equal
    }
}
