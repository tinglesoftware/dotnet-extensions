using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

/// <param name="Color"> Set color of the LED. </param>
/// <param name="LightOnDuration"> Along with <see cref="LightOffDuration"/>, define the blink rate of LED flashes.
/// Resolution defined by proto.Duration
/// <br/>
/// A duration in seconds with up to nine fractional digits, ending with 's'. Example: "3.5s". </param>
/// <param name="LightOffDuration"> Along with <see cref="LightOnDuration"/>, define the blink rate of LED flashes.
/// Resolution defined by proto.Duration
/// <br/>
/// A duration in seconds with up to nine fractional digits, ending with 's'. Example: "3.5s". </param>///
public record FirebaseMessageAndroidLightSettings([property: JsonPropertyName("color")] FirebaseMessageAndroidColor Color,
                                                  [property: JsonPropertyName("light_on_duration")] string LightOnDuration,
                                                  [property: JsonPropertyName("light_off_duration")] string LightOffDuration);
