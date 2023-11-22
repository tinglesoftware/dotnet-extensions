﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;
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
    public virtual Task<ResourceResponse<FcmLegacyResponse>> SendAsync(FcmLegacyRequest message, CancellationToken cancellationToken = default)
        => SendAsync(message, SC.Default.FcmLegacyRequest, cancellationToken);

    /// <summary>Send a push notifications via Firebase Cloud Messaging (FCM).</summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    public virtual Task<ResourceResponse<FcmLegacyResponse>> SendAsync(FcmLegacyRequestAndroid message, CancellationToken cancellationToken = default)
        => SendAsync(message, SC.Default.FcmLegacyRequestAndroid, cancellationToken);

    /// <summary>Send a push notifications via Firebase Cloud Messaging (FCM).</summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    public virtual Task<ResourceResponse<FcmLegacyResponse>> SendAsync(FcmLegacyRequestIos message, CancellationToken cancellationToken = default)
        => SendAsync(message, SC.Default.FcmLegacyRequestIos, cancellationToken);

    /// <summary>Send a push notifications via Firebase Cloud Messaging (FCM).</summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    public virtual Task<ResourceResponse<FcmLegacyResponse>> SendAsync(FcmLegacyRequestWeb message, CancellationToken cancellationToken = default)
        => SendAsync(message, SC.Default.FcmLegacyRequestWeb, cancellationToken);

    /// <summary>Send a push notifications via Firebase Cloud Messaging (FCM).</summary>
    /// <param name="message">The message.</param>
    /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    protected virtual async Task<ResourceResponse<FcmLegacyResponse>> SendAsync<TMessage>(TMessage message,
                                                                                          JsonTypeInfo<TMessage> jsonTypeInfo,
                                                                                          CancellationToken cancellationToken = default)
        where TMessage : FcmLegacyRequest
    {
        var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl) { Content = JsonContent.Create(message, jsonTypeInfo), };
        return await SendAsync(request, SC.Default.FcmLegacyResponse, cancellationToken).ConfigureAwait(false);
    }
}
