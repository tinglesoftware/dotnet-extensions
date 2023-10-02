using Tingle.Extensions.Http;
using Tingle.Extensions.PushNotifications.Apple;
using Tingle.Extensions.PushNotifications.FcmLegacy;
using Tingle.Extensions.PushNotifications.Firebase;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> related to push notifications
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>Add notification services for Firebase Cloud Messaging (FCM) using the legacy HTTP API.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add to.</param>
    /// <param name="configure">Action to configure <see cref="FcmLegacyNotifierOptions"/> instances.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddFcmLegacyNotifier(this IServiceCollection services, Action<FcmLegacyNotifierOptions>? configure = null)
    {
        // configure authentication
        services.AddTransient<FcmLegacyAuthenticationHandler>();
        services.ConfigureOptions<FcmLegacyNotifierConfigureOptions>();

        var builder = services.AddNotifier<FcmLegacyNotifier, FcmLegacyNotifierOptions>(configure)
                              .AddAuthenticationHandler<FcmLegacyAuthenticationHandler>();

        return builder;
    }

    /// <summary>Add notification services for Firebase Cloud Messaging (FCM).</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add to.</param>
    /// <param name="configure">Action to configure <see cref="FirebaseNotifierOptions"/> instances.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddFirebaseNotifier(this IServiceCollection services, Action<FirebaseNotifierOptions>? configure = null)
    {
        // configure authentication
        services.AddTransient<FirebaseAuthenticationHandler>();
        services.ConfigureOptions<FirebaseNotifierConfigureOptions>();

        var builder = services.AddNotifier<FirebaseNotifier, FirebaseNotifierOptions>(configure)
                              .AddAuthenticationHandler<FirebaseAuthenticationHandler>();

        return builder;
    }

    /// <summary>Add notification services for Apple Push Notification Service (APNs).</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add to.</param>
    /// <param name="configure">Action to configure <see cref="ApnsNotifierOptions"/> instances.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder AddApnsNotifier(this IServiceCollection services, Action<ApnsNotifierOptions>? configure = null)
    {
        // configure authentication
        services.AddTransient<ApnsAuthenticationHandler>();
        services.ConfigureOptions<ApnsNotifierConfigureOptions>();

        var builder = services.AddNotifier<ApnsNotifier, ApnsNotifierOptions>(configure)
                              .AddAuthenticationHandler<ApnsAuthenticationHandler>();

        // APNS requires TLS 1.2 or later
        // https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/sending_notification_requests_to_apns#2947606
        builder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                         | System.Security.Authentication.SslProtocols.Tls13,
        });

        return builder;
    }

    private static IHttpClientBuilder AddNotifier<TNotifier, TOptions>(this IServiceCollection services, Action<TOptions>? configure = null)

        where TNotifier : AbstractHttpApiClient<TOptions>
        where TOptions : AbstractHttpApiClientOptions, new()
    {
        return services.AddHttpApiClient<TNotifier, TOptions>(options =>
        {
            // include error details in the exception
            options.IncludeHeadersInExceptionMessage = true;
            options.IncludeRawBodyInExceptionMessage = true;

            configure?.Invoke(options);
        });
    }
}
