using System.Text.RegularExpressions;

namespace System;

/// <summary>Extension methods for <see cref="string"/> for splitting.</summary>
public static partial class StringSplitExtensions
{
    /// <summary>
    /// Split a string in Pascal casing into multiple words by adding the specified <paramref name="separator"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string PascalSplit(this string source, string separator = " ")
    {
        ArgumentNullException.ThrowIfNull(source);

        return GetPascalSplitFormat().Replace(source, separator);
    }

    [GeneratedRegex(@"(?<=[A-Za-z])(?=[A-Z][a-z])|(?<=[a-z0-9])(?=[0-9]?[A-Z])")]
    private static partial Regex GetPascalSplitFormat();
}
