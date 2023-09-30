using System.Text.Json.Serialization;

namespace Tingle.Extensions.PushNotifications.Firebase.Models;

/// <param name="Red">The amount of red in the color.</param>
/// <param name="Green"> The amount of green in the color. </param>
/// <param name="Blue"> The amount of blue in the color. </param>
/// <param name="Alpha"> The fraction of this color that should be applied to the pixel. </param>
public record struct FirebaseMessageAndroidColor([property: JsonPropertyName("red")] float Red,
                                                 [property: JsonPropertyName("green")] float Green,
                                                 [property: JsonPropertyName("blue")] float Blue,
                                                 [property: JsonPropertyName("alpha")] float? Alpha = null);
