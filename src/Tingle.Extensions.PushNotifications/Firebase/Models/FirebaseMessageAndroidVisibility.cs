using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

///
[JsonConverter(typeof(JsonStringEnumConverter<FirebaseMessageAndroidVisibility>))]
public enum FirebaseMessageAndroidVisibility
{
    /// <summary>
    /// If unspecified, default to <see cref="PRIVATE"/>.
    /// </summary>
    VISIBILITY_UNSPECIFIED,

    /// <summary>
    /// Show this notification on all lockscreens, but conceal sensitive or private information on secure lockscreens.
    /// </summary>
    PRIVATE,

    /// <summary>
    /// Show this notification in its entirety on all lockscreens.
    /// </summary>
    PUBLIC,

    /// <summary>
    /// Do not reveal any part of this notification on a secure lockscreen.
    /// </summary>
    SECRET,
}
