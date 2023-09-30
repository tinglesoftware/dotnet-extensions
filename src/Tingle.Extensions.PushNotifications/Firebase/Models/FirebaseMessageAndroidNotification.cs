using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

///
public class FirebaseMessageAndroidNotification
{
    /// <summary>
    /// The notification's title.
    /// If present, it will override <see cref="FirebaseNotification.Title"/>
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// The notification's body text.
    /// If present, it will override <see cref="FirebaseNotification.Body"/>
    /// </summary>
    [JsonPropertyName("body")]
    public string? Body { get; set; }

    /// <summary>
    /// The notification's icon.
    /// Sets the notification icon to myicon for drawable resource myicon.
    /// If you don't send this key in the request, FCM displays the launcher icon specified in your app manifest.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// The notification's icon color, expressed in <c>#rrggbb</c> format.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

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

    /// <summary>
    /// The <see href="https://developer.android.com/preview/features/notification-channels.html">notification's</see> channel id (new in Android O).
    /// <br/>
    /// The app must create a channel with this channel ID before any notification with this channel ID is received.
    /// <br/>
    /// If you don't send this channel ID in the request, or if the channel ID provided has not yet been created by the app,
    /// FCM uses the channel ID specified in the app manifest.
    /// </summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>
    /// Sets the "ticker" text, which is sent to accessibility services.
    /// Prior to API level 21 (Lollipop), sets the text that is displayed in the status bar when the notification first arrives.
    /// </summary>
    [JsonPropertyName("ticker")]
    public string? Ticker { get; set; }

    /// <summary>
    /// When set to false or unset, the notification is automatically dismissed when the user clicks it in the panel
    /// When set to true, the notification persists even when the user clicks it.
    /// </summary>
    [JsonPropertyName("sticky")]
    public bool? Sticky { get; set; }

    /// <summary>
    /// Set the time that the event in the notification occurred.
    /// Notifications in the panel are sorted by this time.
    /// </summary>
    [JsonPropertyName("event_time")]
    public DateTimeOffset? EventTime { get; set; }

    /// <summary>
    /// Set whether or not this notification is relevant only to the current device.
    /// Some notifications can be bridged to other devices for remote display, such as a Wear OS watch.
    /// This hint can be set to recommend this notification not be bridged.
    /// See <see href="https://developer.android.com/training/wearables/notifications/bridger#existing-method-of-preventing-bridging">Wear OS guides</see>
    /// </summary>
    [JsonPropertyName("local_only")]
    public bool? LocalOnly { get; set; }

    /// <summary>
    /// Set the relative priority for this notification.
    /// Priority is an indication of how much of the user's attention should be consumed by this notification.
    /// Low-priority notifications may be hidden from the user in certain situations, while the user might be interrupted for a higher-priority notification.
    /// The effect of setting the same priorities may differ slightly on different platforms.
    /// Note this priority differs from <see cref="FirebaseMessageAndroid.Priority"/>.
    /// This priority is processed by the client after the message has been delivered,
    /// whereas <see href="https://firebase.google.com/docs/reference/fcm/rest/v1/projects.messages#androidmessagepriority">AndroidMessagePriority</see>
    /// is an FCM concept that controls when the message is delivered.
    /// </summary>
    [JsonPropertyName("notification_priority")]
    public FirebaseMessageAndroidNotificationPriority? NotificationPriority { get; set; }

    /// <summary>
    /// If set to true, use the Android framework's default sound for the notification.
    /// Default values are specified in
    /// <see href="https://android.googlesource.com/platform/frameworks/base/+/master/core/res/res/values/config.xml">config.xml</see>.
    /// </summary>
    [JsonPropertyName("default_sound")]
    public bool? DefaultSound { get; set; }

    /// <summary>
    /// If set to true, use the Android framework's default vibrate pattern for the notification.
    /// Default values are specified in
    /// <see href="https://android.googlesource.com/platform/frameworks/base/+/master/core/res/res/values/config.xml">config.xml</see>.
    /// If <see cref="DefaultVibrateTimings"/> is set to true and <see cref="VibrateTimings"/> is also set,
    /// the default value is used instead of the user-specified <see cref="VibrateTimings"/>.
    /// </summary>
    [JsonPropertyName("default_vibrate_timings")]
    public bool? DefaultVibrateTimings { get; set; }

    /// <summary>
    /// If set to true, use the Android framework's default LED light settings for the notification.
    /// Default values are specified in
    /// <see href="https://android.googlesource.com/platform/frameworks/base/+/master/core/res/res/values/config.xml">config.xml</see>.
    /// If <see cref="DefaultLightSettings"/> is set to true and <see cref="LightSettings"/> is also set,
    /// the user-specified <see cref="LightSettings"/> is used instead of the default value.
    /// </summary>
    [JsonPropertyName("default_light_settings")]
    public bool? DefaultLightSettings { get; set; }

    /// <summary>
    /// Set the vibration pattern to use.
    /// Pass in an array of protobuf.Duration to turn on or off the vibrator.
    /// The first value indicates the Duration to wait before turning the vibrator on.
    /// The next value indicates the Duration to keep the vibrator on.
    /// Subsequent values alternate between Duration to turn the vibrator off and to turn the vibrator on.
    /// If <see cref="VibrateTimings"/> is set and <see cref="DefaultVibrateTimings"/> is set to true,
    /// the default value is used instead of the user-specified <see cref="VibrateTimings"/>.
    /// <br/>
    /// A duration in seconds with up to nine fractional digits, ending with 's'. Example: "3.5s".
    /// </summary>
    [JsonPropertyName("vibrate_timings")]
    public ICollection<string>? VibrateTimings { get; set; }

    /// <summary>
    /// Set the notification visibility of the notification.
    /// </summary>
    [JsonPropertyName("visibility")]
    public FirebaseMessageAndroidVisibility? Visibility { get; set; }

    /// <summary>
    /// Sets the number of items this notification represents.
    /// May be displayed as a badge count for launchers that support badging.
    /// See <see href="https://developer.android.com/training/notify-user/badges">Notification Badge</see>. 
    /// For example, this might be useful if you're using just one notification to represent multiple new messages
    /// but you want the count here to represent the number of total new messages.
    /// If zero or unspecified, systems that support badging use the default, which is to increment a number displayed
    /// on the long-press menu each time a new notification arrives.
    /// </summary>
    [JsonPropertyName("notification_count")]
    public int? NotificationCount { get; set; }

    /// <summary>
    /// Settings to control the notification's LED blinking rate and color if LED is available on the device.
    /// The total blinking time is controlled by the OS.
    /// </summary>
    [JsonPropertyName("light_settings")]
    public FirebaseMessageAndroidLightSettings? LightSettings { get; set; }

    /// <summary>
    /// Contains the URL of an image that is going to be displayed in a notification.
    /// If present, it will override <see cref="FirebaseNotification.Image"/>
    /// </summary>
    [JsonPropertyName("image")]
    public string? Image { get; set; }
}
