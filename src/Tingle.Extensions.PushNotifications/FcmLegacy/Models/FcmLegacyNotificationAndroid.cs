using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.FcmLegacy.Models;

/// <summary>
/// Represents an <see cref="FcmLegacyNotification"/> for Android in the legacy HTTP API.
/// </summary>
public class FcmLegacyNotificationAndroid : FcmLegacyNotification
{
    /// <summary>
    /// The <see href="https://developer.android.com/preview/features/notification-channels.html">notification's</see> channel id (new in Android O).
    /// <br/>
    /// The app must create a channel with this channel ID before any notification with this channel ID is received.
    /// <br/>
    /// If you don't send this channel ID in the request, or if the channel ID provided has not yet been created by the app,
    /// FCM uses the channel ID specified in the app manifest.
    /// </summary>
    [JsonPropertyName("android_channel_id")]
    public string? AndroidChannelId { get; set; }

    /// <summary>
    /// The notification's icon.
    /// Sets the notification icon to myicon for drawable resource myicon.
    /// If you don't send this key in the request, FCM displays the launcher icon specified in your app manifest.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// The sound to play when the device receives the notification.
    /// Supports <c>default</c> or the filename of a sound resource bundled in the app.
    /// Sound files must reside in <c>/res/raw/</c> folder.
    /// </summary>
    [JsonPropertyName("sound")]
    public string? Sound { get; set; }

    /// <summary>
    /// Identifier used to replace existing notifications in the notification drawer.
    /// If not specified, each request creates a new notification.
    /// If specified and a notification with the same tag is already being shown,
    /// the new notification replaces the existing one in the notification drawer.
    /// </summary>
    [JsonPropertyName("tag")]
    public string? Tag { get; set; }

    /// <summary>
    /// The notification's icon color, expressed in <c>#rrggbb</c> format.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>
    /// The action associated with a user click on the notification.
    /// If specified, an activity with a matching intent filter is launched when a user clicks on the notification.
    /// </summary>
    [JsonPropertyName("click_action")]
    public string? ClickAction { get; set; }

    /// <summary>
    /// The key to the body string in the app's string resources to use to localize the body text to the user's current localization.
    /// </summary>
    /// <remarks>
    /// See <see href="https://developer.android.com/guide/topics/resources/string-resource.html">String Resources</see> for more information.
    /// </remarks>
    [JsonPropertyName("body_loc_key")]
    public string? BodyLocalizationKey { get; set; }

    /// <summary>
    /// Variable string values to be used in place of the format specifiers in <see cref="BodyLocalizationKey"/>
    /// to use to localize the body text to the user's current localization.
    /// </summary>
    /// <remarks>
    /// See <see href="https://developer.android.com/guide/topics/resources/string-resource.html#FormattingAndStyling">Formatting and Styling</see>
    /// for more information.
    /// </remarks>
    [JsonPropertyName("body_loc_args")]
    public ICollection<string>? BodyLocalizationArgs { get; set; }

    /// <summary>
    /// The key to the title string in the app's string resources to use to localize the title text to the user's current localization.
    /// </summary>
    /// <remarks>
    /// See <see href="https://developer.android.com/guide/topics/resources/string-resource.html">String Resources</see> for more information.
    /// </remarks>
    [JsonPropertyName("title_loc_key")]
    public string? TitleLocalizationKey { get; set; }

    /// <summary>
    /// Variable string values to be used in place of the format specifiers in <see cref="TitleLocalizationKey"/>
    /// to use to localize the title text to the user's current localization.
    /// </summary>
    /// <remarks>
    /// See <see href="https://developer.android.com/guide/topics/resources/string-resource.html#FormattingAndStyling">Formatting and Styling</see>
    /// for more information.
    /// </remarks>
    [JsonPropertyName("title_loc_args")]
    public ICollection<string>? TitleLocalizationArgs { get; set; }
}
