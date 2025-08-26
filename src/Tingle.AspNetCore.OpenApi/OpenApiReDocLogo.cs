using Microsoft.OpenApi;

namespace Tingle.AspNetCore.OpenApi;

/// <summary>
/// Represents a model for configuring a logo using the vendor extension <c>x-logo</c>
/// </summary>
public class OpenApiReDocLogo : IOpenApiExtension
{
    /// <summary>
    /// The URL pointing to the spec logo.
    /// MUST be in the format of a URL.
    /// It SHOULD be an absolute URL so your API definition is usable from any location
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// The background color to be used. MUST be RGB color in [hexadecimal format]
    /// (https://en.wikipedia.org/wiki/Web_colors#Hex_triplet)
    /// </summary>
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// Text to use for alt tag on the logo. Defaults to 'logo' if nothing is provided.
    /// </summary>
    public string? AltText { get; set; }

    /// <summary>
    /// The URL pointing to the contact page. Default to 'info.contact.url' field of the OAS.
    /// </summary>
    public string? Href { get; set; }

    /// <inheritdoc/>
    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        ArgumentNullException.ThrowIfNull(writer);

        writer.WriteStartObject();

        // URL
        if (!string.IsNullOrWhiteSpace(Url))
        {
            writer.WriteProperty(OpenApiConstants.Url, Url);
        }

        // backgroundColor
        if (!string.IsNullOrWhiteSpace(BackgroundColor))
        {
            writer.WriteProperty("backgroundColor", BackgroundColor);
        }

        // altText
        if (!string.IsNullOrEmpty(AltText))
        {
            writer.WriteProperty("altText", AltText);
        }

        // href
        if (!string.IsNullOrEmpty(Href))
        {
            writer.WriteProperty("href", Href);
        }

        writer.WriteEndObject();
    }
}
