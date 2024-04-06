namespace Tingle.Extensions.Mustache.Rendering;

/// <summary>
/// Options for <see cref="TemplateRenderer"/>.
/// </summary>
public class TemplateRenderingOptions
{
    /// <summary>
    /// If this is true, all values will be rendered without being HTML-encoded. (regardless of using {{{ }}} or {{ }} syntax)
    /// In some cases, content should not be escaped (such as when rendering text bodies and subjects in emails). 
    /// By default, we use content escaping, but this parameter allows it to be disabled.
    /// </summary>
    public bool DisableContentSafety { get; set; } = false;

    /// <summary>
    /// Determines if case should be ignored when finding values to replace with.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool IgnoreCase { get; set; } = false;
}
