﻿using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.FcmLegacy.Models;

/// <summary>
/// Represents the priority of an FCM request in the legacy HTTP API.
/// </summary>
[Obsolete(MessageStrings.FirebaseLegacyObsoleteMessage)]
[JsonConverter(typeof(JsonStringEnumConverter<FcmLegacyPriority>))]
public enum FcmLegacyPriority
{
    /// <summary>
    /// Optimized battery consumption on device.
    /// Corresponds to <c>priority: 5</c> on APNS
    /// </summary>
    Normal,

    /// <summary>
    /// Delivered to device immediately.
    /// Corresponds to <c>priority: 10</c> on APNS
    /// </summary>
    High,
}
