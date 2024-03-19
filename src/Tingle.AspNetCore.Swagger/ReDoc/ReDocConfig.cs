using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.AspNetCore.Swagger.ReDoc;

/// <summary>
/// Configuration object for use with <see cref="ReDocOptions"/> to configure the ReDoc UI in
/// the browser produced by <see cref="ReDocMiddleware"/>
/// </summary>
public class ReDocConfig
{
    /// <summary>
    /// If set, the spec is considered untrusted and all HTML/markdown is sanitized to prevent XSS.
    /// Disabled by default for performance reasons. Enable this option if you work with untrusted user data!
    /// </summary>
    [JsonPropertyName("untrustedSpec")]
    public bool UntrustedSpec { get; set; } = false;

    /// <summary>
    /// If set, specifies a vertical scroll-offset in pixels.
    /// This is often useful when there are fixed positioned elements at the top of the page, such as navbars, headers etc
    /// </summary>
    [JsonPropertyName("scrollYOffset")]
    public int? ScrollYOffset { get; set; }

    /// <summary>
    /// If set, the protocol and hostname is not shown in the operation definition
    /// </summary>
    [JsonPropertyName("hideHostname")]
    public bool HideHostname { get; set; } = false;

    /// <summary>
    /// Do not show "Download" spec button. THIS DOESN'T MAKE YOUR SPEC PRIVATE, it just hides the button
    /// </summary>
    [JsonPropertyName("hideDownloadButton")]
    public bool HideDownloadButton { get; set; } = false;

    /// <summary>
    /// Specify which responses to expand by default by response codes.
    /// Values should be passed as comma-separated list without spaces e.g. "200,201". Special value "all" expands all responses by default.
    /// Be careful: this option can slow-down documentation rendering time.
    /// </summary>
    [JsonPropertyName("expandResponses")]
    public string ExpandResponses { get; set; } = "200,201";

    /// <summary>
    /// Show required properties first ordered in the same order as in required array
    /// </summary>
    [JsonPropertyName("requiredPropsFirst")]
    public bool RequiredPropsFirst { get; set; } = false;

    /// <summary>
    /// Do not inject Authentication section automatically
    /// </summary>
    [JsonPropertyName("noAutoAuth")]
    public bool NoAutoAuth { get; set; } = false;

    /// <summary>
    /// Show path link and HTTP verb in the middle panel instead of the right one
    /// </summary>
    [JsonPropertyName("pathInMiddlePanel")]
    public bool PathInMiddlePanel { get; set; } = false;

    /// <summary>
    /// Do not show loading animation. Useful for small docs
    /// </summary>
    [JsonPropertyName("hideLoading")]
    public bool HideLoading { get; set; } = false;

    /// <summary>
    /// Use native scrollbar for sidemenu instead of perfect-scroll (scrolling performance optimization for big specs)
    /// </summary>
    [JsonPropertyName("nativeScrollbars")]
    public bool NativeScrollbars { get; set; } = false;

    /// <summary>
    /// Disable search indexing and search box
    /// </summary>
    [JsonPropertyName("disableSearch")]
    public bool DisableSearch { get; set; } = false;

    /// <summary>
    /// Show only required fields in request samples
    /// </summary>
    [JsonPropertyName("onlyRequiredInSamples")]
    public bool OnlyRequiredInSamples { get; set; } = false;

    /// <summary>
    /// Sort properties alphabetically
    /// </summary>
    [JsonPropertyName("sortPropsAlphabetically")]
    public bool SortPropsAlphabetically { get; set; } = false;

    /// <summary>
    /// Show vendor extensions ("x-" fields). Extensions used by ReDoc are ignored.
    /// Can be boolean or an array of string with names of extensions to display
    /// </summary>
    [JsonPropertyName("showExtensions")]
    [JsonConverter(typeof(AnyOfBooleanOrStringArrayJsonConverter))]
    public AnyOfTypes.AnyOf<bool, ICollection<string>>? ShowExtensions { get; set; } = false;

    /// <summary>
    /// The extras (JSON Extension Data)
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object> AdditionalItems { get; set; } = [];
}

internal partial class AnyOfBooleanOrStringArrayJsonConverter : JsonConverter<AnyOfTypes.AnyOf<bool, ICollection<string>>>
{
    public override AnyOfTypes.AnyOf<bool, ICollection<string>> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, AnyOfTypes.AnyOf<bool, ICollection<string>> value, JsonSerializerOptions options)
    {
        if (value == default) writer.WriteNullValue();
        else if (value.CurrentValue is bool b) writer.WriteBooleanValue(b);
        else if (value.CurrentValue is ICollection<string> col)
        {
            writer.WriteStartArray();
            foreach (var item in col)
            {
                writer.WriteStringValue(item);
            }
            writer.WriteEndArray();
            return;
        }
    }
}
