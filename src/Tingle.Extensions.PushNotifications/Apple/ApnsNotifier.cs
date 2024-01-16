using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json.Serialization.Metadata;
using Tingle.Extensions.Http;
using Tingle.Extensions.PushNotifications.Apple.Models;
using SC = Tingle.Extensions.PushNotifications.PushNotificationsJsonSerializerContext;

namespace Tingle.Extensions.PushNotifications.Apple;

/// <summary>
/// A push notifier for Apple's Push Notification Service
/// </summary>
/// <param name="httpClient">The <see cref="HttpClient"/> for making requests.</param>
/// <param name="optionsAccessor">The options accessor for <see cref="ApnsNotifierOptions"/>.</param>
public class ApnsNotifier(HttpClient httpClient, IOptionsSnapshot<ApnsNotifierOptions> optionsAccessor) : AbstractHttpApiClient<ApnsNotifierOptions>(httpClient, optionsAccessor)
{
    private const string ProductionBaseUrl = "https://api.push.apple.com:443";
    private const string DevelopmentBaseUrl = "https://api.development.push.apple.com:443";

    /// <summary>Send a push notification via Apple Push Notification Service (APNS).</summary>
    /// <param name="header">The header for the notification.</param>
    /// <param name="data">The data.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    public virtual Task<ResourceResponse<ApnsMessageResponse, ApnsResponseError>> SendAsync(ApnsMessageHeader header,
                                                                                            ApnsMessageData data,
                                                                                            CancellationToken cancellationToken = default)
        => SendAsync(header, data, SC.Default.ApnsMessageData, cancellationToken);

    /// <summary>Send a push notification with custom data via Apple Push Notification Service (APNS).</summary>
    /// <param name="header">The header for the notification.</param>
    /// <param name="data">The customized data.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    public virtual Task<ResourceResponse<ApnsMessageResponse, ApnsResponseError>> SendAsync<TData>(ApnsMessageHeader header,
                                                                                                   TData data,
                                                                                                   CancellationToken cancellationToken = default)
        where TData : ApnsMessageData
    {
        // ensure we have the aps node
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (data.Aps == null) throw new ArgumentException($"{nameof(data.Aps)} cannot be null", nameof(data));

        var content = MakeJsonContent(data);
        return SendAsync(header, content, cancellationToken);
    }

    /// <summary>Send a push notification with custom data via Apple Push Notification Service (APNS).</summary>
    /// <param name="header">The header for the notification.</param>
    /// <param name="data">The customized data.</param>
    /// <param name="jsonTypeInfo">Metadata about the <typeparamref name="TData"/> to convert.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    public virtual Task<ResourceResponse<ApnsMessageResponse, ApnsResponseError>> SendAsync<TData>(ApnsMessageHeader header,
                                                                                                   TData data,
                                                                                                   JsonTypeInfo<TData> jsonTypeInfo,
                                                                                                   CancellationToken cancellationToken = default)
        where TData : ApnsMessageData
    {
        // ensure we have the aps node
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (data.Aps == null) throw new ArgumentException($"{nameof(data.Aps)} cannot be null", nameof(data));

        var content = MakeJsonContent(data, jsonTypeInfo);
        return SendAsync(header, content, cancellationToken);
    }

    /// <summary>Send a push notification via Apple Push Notification Service (APNS).</summary>
    /// <param name="header">The header for the notification.</param>
    /// <param name="content">The <see cref="HttpContent"/> to use in the body.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    protected virtual async Task<ResourceResponse<ApnsMessageResponse, ApnsResponseError>> SendAsync(ApnsMessageHeader header,
                                                                                                     HttpContent content,
                                                                                                     CancellationToken cancellationToken = default)
    {
        // infer the endpoint from the provided environment
        var endpoint = new Uri(header.Environment == ApnsEnvironment.Production ? ProductionBaseUrl : DevelopmentBaseUrl);

        // ensure we have a valid device token
        if (string.IsNullOrWhiteSpace(header.DeviceToken))
        {
            throw new ArgumentException($"{nameof(header.DeviceToken)} cannot be null", nameof(header));
        }

        // ensure we have the content
        if (content == null) throw new ArgumentNullException(nameof(content));

        var path = $"/3/device/{header.DeviceToken}";
        var uri = new Uri(endpoint, path);
        var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Version = new Version(2, 0), // APNs requires HTTP/2
            Content = content,
        };

        // specify the header for content we can accept
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

        // add header specific to APNs
        request.Headers.TryAddWithoutValidation(":method", "POST");
        request.Headers.TryAddWithoutValidation(":path", path);

        // add an Id if specified
        if (header.Id != null)
        {
            request.Headers.Add("apns-id", header.Id.ToString()!.ToLower());
        }

        // add expiration in UNIX seconds since Epoch.
        var expiration = header.Expiration?.ToUnixTimeSeconds() ?? 0;
        request.Headers.Add("apns-expiration", expiration.ToString());

        // add priority as an int: 10,5.
        var priority = (int)header.Priority;
        request.Headers.Add("apns-priority", priority.ToString());

        // add the push type (string, all lowercase)
        var type = header.PushType.ToString().ToLower();
        request.Headers.Add("apns-push-type", type);

        // add the topic
        // The topic of the remote notification, which is typically the bundle ID for the app.
        // The certificate created in the developer account must include the capability for this topic.
        // If the certificate includes multiple topics, you value for this header must be specified.
        // If this request header is omitted and the APNs certificate does not specify multiple topics,
        // the APNs server uses the certificate’s Subject as the default topic.
        // If a provider token ins used instead of a certificate, a value for this request header must be specified.
        // The topic provided should be provisioned for the team named in your developer account.
        // For certain types of push, the topic needs to be suffixed with a pre-defined string
        var topic = Options.BundleId;
        topic += GetTopicSuffixFromPushType(header.PushType);
        request.Headers.Add("apns-topic", topic);

        // add a collapseId if specified
        if (!string.IsNullOrEmpty(header.CollapseId))
        {
            // ensure the value is not longer than 64 bytes
            var value = header.CollapseId[..Math.Min(header.CollapseId.Length, 64)];
            request.Headers.Add("apns-collapse-id", value);
        }

        return await SendAsync(request, SC.Default.ApnsMessageResponse, SC.Default.ApnsResponseError, cancellationToken).ConfigureAwait(false);
    }

    internal static string? GetTopicSuffixFromPushType(ApnsPushType type)
    {
        return type switch
        {
            ApnsPushType.Voip => ".voip",
            ApnsPushType.Complication => ".complication",
            ApnsPushType.FileProvider => ".pushkit.fileprovider",
            _ => null,
        };
    }
}
