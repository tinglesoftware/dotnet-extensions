using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Tingle.AspNetCore.Swagger;

/// <summary>
/// Functionality for working with XML comments in Swagger.
/// </summary>
public static partial class XmlCommentsHelper
{
    // TODO: Contribute this back to the library

    private static readonly Regex BrPattern = GetBrPattern();

    ///
    [return: NotNullIfNotNull(nameof(text))]
    public static string? ToMarkdown(string? text)
    {
        if (text is null) return null;

        return text.ConvertBrTags();
    }

    private static string ConvertBrTags(this string text)
    {
        return BrPattern.Replace(text, m => Environment.NewLine);
    }

    [GeneratedRegex(@"(<br \/>|<br\/>|<br>)")]
    private static partial Regex GetBrPattern();
}
