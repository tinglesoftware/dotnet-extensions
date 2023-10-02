using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Apple.Models;

/// <summary>
/// Represents a payload for a push notification as specified by Apple
/// </summary>
public record ApnsMessagePayload
{
    /// <summary>
    /// Set a value when you want the system to display a standard alert or a banner.
    /// The notification settings for your app on the user’s device determine
    /// whether an alert or banner is displayed.
    /// </summary>
    [JsonPropertyName("alert")]
    public ApnsAlert Alert { get; set; } = new ApnsAlert { };

    /// <summary>
    /// Set a value when you want the system to modify the badge of your app icon.
    /// If value is null, the badge is not changed.
    /// To remove the badge, set this property to 0.
    /// Defaults to <see langword="null"/>
    /// </summary>
    [JsonPropertyName("badge")]
    public int? Badge { get; set; }

    /// <summary>
    /// Set this value when you want the system to play a sound. The value set is the
    /// name of a sound file in your app’s main bundle or in the <c>Library/Sounds</c>
    /// folder of your app’s data container. If the sound file cannot be found, or if
    /// you specify <c>default</c> for the value, the system plays the default alert sound.
    /// <para/>
    /// For details about providing sound files for notifications,
    /// see Preparing Custom Alert Sounds.
    /// https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/SupportingNotificationsinYourApp.html#//apple_ref/doc/uid/TP40008194-CH4-SW10
    /// </summary>
    [JsonPropertyName("sound")]
    public string? Sound { get; set; }

    /// <summary>
    /// Set this value to 1 to configure a background update notification.
    /// When a value is set, the system wakes up your app in the background and delivers
    /// the notification to its app delegate.
    /// <para/>
    /// For information about configuring and handling background update notifications,
    /// see Configuring a Background Update Notification.
    /// https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW8
    /// </summary>
    [JsonPropertyName("content-available")]
    public int? ContentAvailable { get; set; }

    /// <summary>
    /// Set a value that represents the notification’s type. This value corresponds to the
    /// value in the identifier property of one of your app’s registered categories.
    /// <para/>
    /// To learn more about using custom actions, see Configuring Categories and Actionable Notifications.
    /// https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/SupportingNotificationsinYourApp.html#//apple_ref/doc/uid/TP40008194-CH4-SW26
    /// </summary>
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    /// <summary>
    /// Set a value that represents the app-specific identifier for grouping notifications.
    /// If you provide a Notification Content app extension, you can use this value to group
    /// your notifications together. For local notifications, this key corresponds to the
    /// <c>threadIdentifier</c> property of the <c>UNNotificationContent</c> object.
    /// </summary>
    [JsonPropertyName("thread-id")]
    public string? ThreadId { get; set; }
}
