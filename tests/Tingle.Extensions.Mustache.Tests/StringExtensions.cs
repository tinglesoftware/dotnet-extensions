using System.Text.RegularExpressions;

namespace Tingle.Extensions.Mustache.Tests;

internal static partial class StringExtensions
{
    /// <summary>
    /// Provides a mechanism to make comparing expected and actual results a little more sane to author.
    /// You may include whitespace in resources to make them easier to read.
    /// </summary>
    /// <param name="subject"></param>
    /// <returns></returns>
    internal static string EliminateWhitespace(this string subject)
        => GetWhitespaceNormalizer().Replace(subject, "");

    [GeneratedRegex("[\\s]+", RegexOptions.Compiled)]
    private static partial Regex GetWhitespaceNormalizer();
}
