namespace Tingle.Extensions.Mustache.Parsing;

/// <summary>
/// Options for <see cref="TemplateParser"/>.
/// </summary>
public class TemplateParserOptions
{
    /// <summary>
    /// The source name for this template. Will be used in error reporting to identify the location of parsing errors.
    /// </summary>
    public string? SourceName { get; set; }

    /// <summary>
    /// Allows support for unknown tokens. You can use this to include partials,
    /// or any custom behaviour such as date/time formatters, localization, etc.
    /// </summary>
    public ICollection<TemplateTokenExpander> TokenExpanders { get; } = new HashSet<TemplateTokenExpander>();
}
