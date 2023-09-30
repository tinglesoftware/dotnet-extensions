namespace Tingle.Extensions.PushNotifications.Apple.Models;

/// <summary>
/// The priority of a message
/// </summary>
public enum ApnsPriority
{
    /// <summary>
    /// Send the push message immediately. Notifications with this priority
    /// must trigger an alert, sound, or badge on the target device. It is
    /// an error to use this priority for a push notification that contains
    /// only the <c>content-available</c> key.
    /// </summary>
    Immediately = 10,

    /// <summary>
    /// Send the push message at a time that takes into account power
    /// considerations for the device. Notifications with this priority
    /// might be grouped and delivered in bursts. They are throttled,
    /// and in some cases are not delivered.
    /// </summary>
    Normal = 5,
}
