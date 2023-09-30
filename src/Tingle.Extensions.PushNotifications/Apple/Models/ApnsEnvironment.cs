namespace Tingle.Extensions.PushNotifications.Apple.Models;

/// <summary>
/// The APNs environment to send the notification to.
/// </summary>
public enum ApnsEnvironment
{
    /// <summary>
    /// Represents the development environment and must be used with device tokens registered on a similar environment.
    /// </summary>
    Development,

    /// <summary>
    /// Represents the production environment and must be used with device tokens registered on a similar environment.
    /// </summary>
    Production
}
