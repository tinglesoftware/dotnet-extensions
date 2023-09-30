using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

///
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FirebaseMessageAndroidNotificationPriority
{
    /// <summary>
    /// If priority is unspecified, notification priority is set to <see cref="PRIORITY_DEFAULT"/>.
    /// </summary>
    PRIORITY_UNSPECIFIED,

    /// <summary>
    /// Lowest notification priority.
    /// Notifications with this priority might not be shown to the user except under special circumstances, such as detailed notification logs.
    /// </summary>
    PRIORITY_MIN,

    /// <summary>
    /// Lower notification priority.
    /// The UI may choose to show the notifications smaller, or at a different position in the list, compared with notifications with <see cref="PRIORITY_DEFAULT"/>.
    /// </summary>
    PRIORITY_LOW,

    /// <summary>
    /// Default notification priority.
    /// If the application does not prioritize its own notifications, use this value for all notifications.
    /// </summary>
    PRIORITY_DEFAULT,

    /// <summary>
    /// Higher notification priority.
    /// Use this for more important notifications or alerts.
    /// The UI may choose to show these notifications larger, or at a different position in the notification lists, compared with notifications with <see cref="PRIORITY_DEFAULT"/>.
    /// </summary>
    PRIORITY_HIGH,

    /// <summary>
    /// Highest notification priority.
    /// Use this for the application's most important items that require the user's prompt attention or input.
    /// </summary>
    PRIORITY_MAX,
}
