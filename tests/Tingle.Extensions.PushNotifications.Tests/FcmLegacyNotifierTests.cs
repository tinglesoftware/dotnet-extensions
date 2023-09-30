﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tingle.Extensions.PushNotifications.FcmLegacy;
using Tingle.Extensions.PushNotifications.FcmLegacy.Models;
using Xunit.Abstractions;

namespace Tingle.Extensions.PushNotifications.Tests;

public class FcmLegacyNotifierTests
{
    private readonly ITestOutputHelper outputHelper;

    public FcmLegacyNotifierTests(ITestOutputHelper outputHelper)
    {
        this.outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
    }

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

    [Fact]
    public async Task Works()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<FcmLegacyNotifierTests>(optional: true) // local debug
            .AddEnvironmentVariables() // CI-pipeline
            .Build();

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddXUnit(outputHelper));
        services.AddFcmLegacyNotifier(options =>
        {
            options.Key = configuration.GetValue<string>("FcmLegacyTest:Key");
        });

        var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;
        var client = sp.GetRequiredService<FcmLegacyNotifier>();

        var model = new FcmLegacyRequest
        {
            RegistrationIds = new[]
            {
                configuration.GetValue<string>("FcmLegacyTest:RegistrationId")!,
            },
        };
        var response = await client.SendAsync(model);
        response.EnsureSuccess();
    }
}
