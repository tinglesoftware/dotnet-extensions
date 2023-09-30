namespace Tingle.Extensions.PushNotifications.Apple.Models;

/// <summary>
/// The type of push request being made.
/// It is required on watchOS 6 and later. It is recommended on macOS, iOS, tvOS, and iPadOS.
/// </summary>
public enum ApnsPushType
{
    /// <summary>
    /// Use the alert push type for notifications that trigger a user interaction—for example,
    /// an alert, badge, or sound. If you set this push type, the <c>apns-topic</c> header field must
    /// use your app’s bundle ID as the topic. For more information, see
    /// <see href="https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/generating_a_remote_notification">
    /// Generating a Remote Notification
    /// </see>.
    /// </summary>
    Alert,

    /// <summary>
    /// Use the background push type for notifications that deliver content in the background, and don’t
    /// trigger any user interactions. If you set this push type, the <c>apns-topic</c> header field must
    /// use your app’s bundle ID as the topic. For more information, see
    /// <see href="https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/pushing_background_updates_to_your_app">
    /// Pushing Background Updates to Your App
    /// </see>.
    /// </summary>
    Background,

    /// <summary>
    /// Use the voip push type for notifications that provide information about an incoming Voice-over-IP
    /// (VoIP) call. For more information, see 
    /// <see href="https://developer.apple.com/documentation/pushkit/responding_to_voip_notifications_from_pushkit">
    /// Responding to VoIP Notifications from PushKit
    /// </see>.
    /// If you set this push type, the <c>apns-topic</c> header field must use your app’s bundle ID with <c>.voip</c>
    /// appended to the end. If you’re using certificate-based authentication, you must also register the certificate
    /// for VoIP services. The topic is then part of the <c>1.2.840.113635.100.6.3.4</c> or <c>1.2.840.113635.100.6.3.6</c>
    /// extension.
    /// </summary>
    Voip,

    /// <summary>
    /// Use the complication push type for notifications that contain update information for a watchOS app’s
    /// complications. For more information, see
    /// <see href="https://developer.apple.com/documentation/clockkit/adding_a_complication_to_your_watchos_app/providing_data_for_your_complication/updating_your_timeline">
    /// Updating Your Timeline
    /// </see>.
    /// If you set this push type, the <c>apns-topic</c> header field must use your app’s bundle ID with <c>.complication</c>
    /// appended to the end. If you’re using certificate-based authentication, you must also register the certificate
    /// for WatchKit services. The topic is then part of the <c>1.2.840.113635.100.6.3.6</c> extension.
    /// </summary>
    Complication,

    /// <summary>
    /// Use the fileprovider push type to signal changes to a File Provider extension. If you set this push type,
    /// the <c>apns-topic</c> header field must use your app’s bundle ID with <c>.pushkit.fileprovider</c> appended
    /// to the end. For more information, see 
    /// <see href="https://developer.apple.com/documentation/fileprovider/content_and_change_tracking/tracking_your_file_provider_s_changes/using_push_notifications_to_signal_changes">
    /// Using Push Notifications to Signal Changes
    /// </see>.
    /// </summary>
    FileProvider,

    /// <summary>
    /// Use the mdm push type for notifications that tell managed devices to contact the MDM server. If you set
    /// this push type, you must use the topic from the UID attribute in the subject of your MDM push certificate.
    /// For more information, see
    /// <see href="https://developer.apple.com/documentation/devicemanagement">
    /// Device Management
    /// </see>.
    /// </summary>
    Mdm,
}
