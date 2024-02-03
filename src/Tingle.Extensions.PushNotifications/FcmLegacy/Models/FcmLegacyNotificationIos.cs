using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.FcmLegacy.Models;

/// <summary>
/// Represents an <see cref="FcmLegacyNotification"/> for iOS.
/// </summary>
[Obsolete(MessageStrings.FirebaseLegacyObsoleteMessage)]
public class FcmLegacyNotificationIos : FcmLegacyNotification
{
    /// <summary>
    /// The sound to play when the device receives the notification.
    /// <br/>
    /// String specifying sound files in the main bundle of the client app or in the <c>Library/Sounds</c> folder of the app's data container.
    /// </summary>
    /// <remarks>
    /// See the <see href="https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/SupportingNotificationsinYourApp.html#//apple_ref/doc/uid/TP40008194-CH4-SW10">iOS Developer Library</see> for more information.
    /// </remarks>
    [JsonPropertyName("sound")]
    public string? Sound { get; set; }

    /// <summary>
    /// The value of the badge on the home screen app icon.
    /// If not specified, the badge is not changed.
    /// If set to 0, the badge is removed.
    /// </summary>
    [JsonPropertyName("badge")]
    public string? Badge { get; set; }

    /// <summary>
    /// The action associated with a user click on the notification.
    /// Corresponds to <c>category</c> in the APNs payload.
    /// </summary>
    [JsonPropertyName("click_action")]
    public string? ClickAction { get; set; }

    /// <summary>
    /// The notification's subtitle.
    /// </summary>
    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }

    /// <summary>
    /// The key to the body string in the app's string resources to use to localize the body text to the user's current localization.
    /// Corresponds to <c>loc-key</c> in the APNs payload.
    /// </summary>
    /// <remarks>
    /// See
    /// <see href="https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/PayloadKeyReference.html">Payload Key Reference</see>
    /// and
    /// <see href="https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW9">Localizing the Content of Your Remote Notifications</see> for more information.
    /// </remarks>
    [JsonPropertyName("body_loc_key")]
    public string? BodyLocalizationKey { get; set; }

    /// <summary>
    /// Variable string values to be used in place of the format specifiers in <see cref="BodyLocalizationKey"/>
    /// to use to localize the body text to the user's current localization.
    /// Corresponds to <c>loc-args</c> in the APNs payload.
    /// </summary>
    /// <remarks>
    /// See
    /// <see href="https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/PayloadKeyReference.html">Payload Key Reference</see>
    /// and
    /// <see href="https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW9">Localizing the Content of Your Remote Notifications</see> for more information.
    /// </remarks>
    [JsonPropertyName("body_loc_args")]
    public ICollection<string>? BodyLocalizationArgs { get; set; }

    /// <summary>
    /// The key to the title string in the app's string resources to use to localize the title text to the user's current localization.
    /// Corresponds to <c>title-loc-key</c> in the APNs payload.
    /// </summary>
    /// <remarks>
    /// See
    /// <see href="https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/PayloadKeyReference.html">Payload Key Reference</see>
    /// and
    /// <see href="https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW9">Localizing the Content of Your Remote Notifications</see> for more information.
    /// </remarks>
    [JsonPropertyName("title_loc_key")]
    public string? TitleLocalizationKey { get; set; }

    /// <summary>
    /// Variable string values to be used in place of the format specifiers in <see cref="TitleLocalizationKey"/>
    /// to use to localize the title text to the user's current localization.
    /// Corresponds to <c>title-loc-args</c> in the APNs payload.
    /// </summary>
    /// <remarks>
    /// See
    /// <see href="https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/PayloadKeyReference.html">Payload Key Reference</see>
    /// and
    /// <see href="https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW9">Localizing the Content of Your Remote Notifications</see> for more information.
    /// </remarks>
    [JsonPropertyName("title_loc_args")]
    public ICollection<string>? TitleLocalizationKeyArgs { get; set; }
}
