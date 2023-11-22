using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Tingle.Extensions.Http;
using Tingle.Extensions.PushNotifications.FcmLegacy;
using Tingle.Extensions.PushNotifications.Firebase.Models;
using SC = Tingle.Extensions.PushNotifications.PushNotificationsJsonSerializerContext;

namespace Tingle.Extensions.PushNotifications.Firebase;

/// <summary>
/// A push notification handler for Firebase Cloud Messaging Service
/// </summary>
public class FirebaseNotifier : AbstractHttpApiClient<FirebaseNotifierOptions>
{
    /// <summary>Creates an instance of <see cref="FcmLegacyNotifier"/>.</summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> for making requests.</param>
    /// <param name="optionsAccessor">The options accessor for <see cref="FirebaseNotifierOptions"/>.</param>
    public FirebaseNotifier(HttpClient httpClient, IOptionsSnapshot<FirebaseNotifierOptions> optionsAccessor)
        : base(httpClient, optionsAccessor) { }

    /// <summary>Send a push notifications via Firebase Cloud Messaging (FCM).</summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    public virtual async Task<ResourceResponse<FirebaseResponse, FirebaseResponseProblem>> SendAsync(FirebaseRequest message,
                                                                                                     CancellationToken cancellationToken = default)
    {
        var url = $"https://fcm.googleapis.com/v1/projects/{Options.ProjectId}/messages:send";
        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = JsonContent.Create(message, SC.Default.FirebaseRequest), };
        return await SendAsync(request, SC.Default.FirebaseResponse, SC.Default.FirebaseResponseProblem, cancellationToken).ConfigureAwait(false);
    }
}
