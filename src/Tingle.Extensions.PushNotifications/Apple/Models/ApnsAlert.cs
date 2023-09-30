using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Apple.Models;

/// <summary>
/// Represents the alert delivered to the device
/// </summary>
public class ApnsAlert
{
    /// <summary>
    /// A short string describing the purpose of the notification. Apple Watch displays this
    /// string as part of the notification interface. This string is displayed only briefly
    /// and should be crafted so that it can be understood quickly.
    /// </summary>
    /// <remarks>This key was added in iOS 8.2.</remarks>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// The text of the alert message.
    /// </summary>
    [JsonPropertyName("body")]
    public string? Body { get; set; }

    /// <summary>
    /// The key to a title string in the <c>Localizable.strings</c> file for the current localization.
    /// The key string can be formatted with <c>%@</c> and <c>%n$@</c> specifiers to take the variables
    /// specified in <see cref="TitleLocalizationArgs"/>.
    /// <para/>
    /// See Localizing the Content of Your Remote Notifications for more information.
    /// https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW9
    /// </summary>
    /// <remarks>This key was added in iOS 8.2.</remarks>
    [JsonPropertyName("title-loc-key")]
    public string? TitleLocalizationKey { get; set; }

    /// <summary>
    /// Variable string values to appear in place of the format specifiers in <see cref="TitleLocalizationKey"/>.
    /// <para/>
    /// See Localizing the Content of Your Remote Notifications for more information.
    /// https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW9
    /// </summary>
    /// <remarks>This key was added in iOS 8.2.</remarks>
    [JsonPropertyName("title-loc-args")]
    public ICollection<string>? TitleLocalizationArgs { get; set; }

    /// <summary>
    /// If specified, the system displays an alert that includes the Close and View buttons.
    /// The set value is used as a key to get a localized string in the current localization
    /// to use for the right button’s title instead of "View".
    /// <para/>
    /// See Localizing the Content of Your Remote Notifications for more information.
    /// https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW9
    /// </summary>
    [JsonPropertyName("action-loc-key")]
    public string? ActionLocalizationKey { get; set; }

    /// <summary>
    /// The key to an alert-message string in a <c>Localizable.strings</c> file for the current
    /// localization (which is set by the user’s language preference). The key string can be
    /// formatted with <c>%@</c> and <c>%n$@</c> specifiers to take the variables specified in
    /// <see cref="LocalizationArgs"/>.
    /// <para/>
    /// See Localizing the Content of Your Remote Notifications for more information.
    /// https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW9
    /// </summary>
    [JsonPropertyName("loc-key")]
    public string? LocalizationKey { get; set; }

    /// <summary>
    /// Variable string values to appear in place of the format specifiers in <see cref="LocalizationKey"/>.
    /// <para/>
    /// See Localizing the Content of Your Remote Notifications for more information.
    /// https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CreatingtheNotificationPayload.html#//apple_ref/doc/uid/TP40008194-CH10-SW9
    /// </summary>
    [JsonPropertyName("loc-args")]
    public ICollection<string>? LocalizationArgs { get; set; }

    /// <summary>
    /// The filename of an image file in the app bundle, with or without the filename extension.
    /// The image is used as the launch image when users tap the action button or move the
    /// action slider. If this property is not specified, the system either uses the previous
    /// snapshot, uses the image identified by the <c>UILaunchImageFile</c> key in the app’s
    /// <c>Info.plist</c> file, or falls back to <c>Default.png</c>.
    /// </summary>
    [JsonPropertyName("launch-image")]
    public string? LaunchImage { get; set; }
}
