using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tingle.Extensions.PushNotifications.FcmLegacy;
using Tingle.Extensions.PushNotifications.FcmLegacy.Models;
using Xunit.Abstractions;

#pragma warning disable CS0618 // Type or member is obsolete

namespace Tingle.Extensions.PushNotifications.Tests;

public class FcmLegacyNotifierTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public void Resolution_Works()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddXUnit(outputHelper));
        services.AddFcmLegacyNotifier(o => o.Key = Guid.NewGuid().ToString());

        var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;
        var client = sp.GetRequiredService<FcmLegacyNotifier>();
    }

    [Fact]
    public async Task Authentication_IsPopulated()
    {
        var header = (string?)null;
        var handler = new DynamicHttpMessageHandler((request, ct) =>
        {
            Interlocked.Exchange(ref header, Assert.Single(request.Headers.GetValues("Authorization")));
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        });

        var key = Guid.NewGuid().ToString();
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddXUnit(outputHelper));
        services.AddFcmLegacyNotifier(options => options.Key = key)
                .ConfigurePrimaryHttpMessageHandler(() => handler);

        var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;
        var client = sp.GetRequiredService<FcmLegacyNotifier>();

        var model = new FcmLegacyRequest { };
        var rr = await client.SendAsync(model);
        Assert.Equal($"key={key}", header);
    }
}
