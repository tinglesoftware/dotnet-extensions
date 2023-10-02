using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tingle.Extensions.Http;
using Tingle.Extensions.PushNotifications.FcmLegacy.Models;
using SC = Tingle.Extensions.PushNotifications.PushNotificationsJsonSerializerContext;

namespace Tingle.Extensions.PushNotifications.FcmLegacy;

/// <summary>
/// A push notification handler for Firebase Cloud Messaging Service
/// </summary>
public class FcmLegacyNotifier : AbstractHttpApiClient<FcmLegacyNotifierOptions>
{
    internal const string BaseUrl = "https://fcm.googleapis.com/fcm/send";

    /// <summary>Creates an instance of <see cref="FcmLegacyNotifier"/>.</summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> for making requests.</param>
    /// <param name="optionsAccessor">The options accessor for <see cref="FcmLegacyNotifierOptions"/>.</param>
    public FcmLegacyNotifier(HttpClient httpClient, IOptionsSnapshot<FcmLegacyNotifierOptions> optionsAccessor)
        : base(httpClient, optionsAccessor) { }

    /// <summary>Send a push notifications via Firebase Cloud Messaging (FCM).</summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    public virtual async Task<ResourceResponse<FcmLegacyResponse>> SendAsync(FcmLegacyRequest message,
                                                                             CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl) { Content = MakeJsonContent(message, SC.Default.FcmLegacyRequest), };
        return await SendAsync(request, SC.Default.FcmLegacyResponse, cancellationToken).ConfigureAwait(false);
    }
}
