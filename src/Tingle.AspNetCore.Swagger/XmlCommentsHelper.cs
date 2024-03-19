using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Tingle.AspNetCore.Swagger;

/// <summary>
/// Functionality for working with XML comments in Swagger.
/// </summary>
public static partial class XmlCommentsHelper
{
    // TODO: Remove this and related classes once main library supports
    // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/2392

    private static readonly Regex SeeHrefPattern = GetSeeHrefPattern();
    private static readonly Regex BrPattern = GetBrPattern();

    ///
    [return: NotNullIfNotNull(nameof(text))]
    public static string? ToMarkdown(string? text)
    {
        if (text is null) return null;

        return text.ConvertSeeHrefTags()
                   .ConvertBrTags();
    }

    private static string ConvertSeeHrefTags(this string text)
    {
        return SeeHrefPattern.Replace(text, m => $"[{m.Groups[2].Value}]({m.Groups[1].Value})");
    }

    private static string ConvertBrTags(this string text)
    {
        return BrPattern.Replace(text, m => Environment.NewLine);
    }

    [GeneratedRegex(@"<see href=\""(.*)\"">(.*)<\/see>")]
    private static partial Regex GetSeeHrefPattern();
    [GeneratedRegex(@"(<br \/>|<br\/>|<br>)")]
    private static partial Regex GetBrPattern();
}
