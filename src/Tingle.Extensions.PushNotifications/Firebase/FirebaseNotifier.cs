﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
    /// <summary>
    /// Creates an instance of <see cref="FcmLegacyNotifier"/>
    /// </summary>
    /// <param name="httpClient">the client for making requests</param>
    /// <param name="optionsAccessor">the accessor for the configuration options</param>
    public FirebaseNotifier(HttpClient httpClient, IOptionsSnapshot<FirebaseNotifierOptions> optionsAccessor)
        : base(httpClient, optionsAccessor) { }

    /// <summary>
    /// Send a push notifications via Firebase Cloud Messaging (FCM)
    /// </summary>
    /// <param name="message">the message</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<ResourceResponse<FirebaseResponse, FirebaseResponseProblem>> SendAsync(FirebaseRequest message,
                                                                                                     CancellationToken cancellationToken = default)
    {
        var url = $"https://fcm.googleapis.com/v1/projects/{Options.ProjectId}/messages:send";
        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = MakeJsonContent(message, SC.Default.FirebaseRequest), };
        return await SendAsync(request, SC.Default.FirebaseResponse, SC.Default.FirebaseResponseProblem, cancellationToken).ConfigureAwait(false);
    }
}