namespace Tingle.Extensions.PushNotifications.Apple.Models;

/// <summary>
/// A header for a message to be sent to APNs
/// </summary>
public sealed class ApnsMessageHeader
{
    /// <summary>
    /// A unique identifier for the message. If there is
    /// an error sending the notification, APNs uses this
    /// value to identify the notification to your server.
    /// If set to null, a new value is created by APNs
    /// and returned in the response.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// This identifies the date when the notification is no longer valid and can be discarded.
    /// If this value is not null, APNs stores the notification and tries to deliver it at least once,
    /// repeating the attempt as needed if it is unable to deliver the notification the first time.
    /// <para></para>
    /// If the value is null, APNs treats the notification as if it expires immediately and does not
    /// store the notification or attempt to redeliver it.
    /// </summary>
    public DateTimeOffset? Expiration { get; set; }

    /// <summary>
    /// The priority of the notification.
    /// Defaults to <see cref="ApnsPriority.Immediately"/>.
    /// </summary>
    public ApnsPriority Priority { get; set; } = ApnsPriority.Immediately;

    /// <summary>
    /// Multiple notifications with the same collapse identifier are displayed to the user as a
    /// single notification. The value of this key must not exceed 64 bytes. For more information, see
    /// Quality of Service, Store-and-Forward, and Coalesced Notifications at
    /// https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/APNSOverview.html#//apple_ref/doc/uid/TP40008194-CH8-SW5
    /// </summary>
    public string? CollapseId { get; set; }

    /// <summary>
    /// The environment to send the notification to.
    /// Defaults to <see cref="ApnsEnvironment.Production"/>.
    /// </summary>
    public ApnsEnvironment Environment { get; set; } = ApnsEnvironment.Production;

    /// <summary>
    /// The type of push notification.
    /// Defaults to <see cref="ApnsPushType.Background"/>.
    /// </summary>
    public ApnsPushType PushType { get; set; } = ApnsPushType.Background;

    /// <summary>
    /// The token for the device to send the message to
    /// </summary>
    public required string DeviceToken { get; set; }
}
