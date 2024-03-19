namespace System;

/// <summary>
/// Specifies the position to apply protection in <see cref="string"/> objects.
/// </summary>
public enum StringProtectionPosition
{
    /// <summary>
    /// Protect the start. Example result: <c>**********dttJAsQVU</c>
    /// </summary>
    Start,

    /// <summary>
    /// Protect the middle. Example result: <c>e0gNH**********AsQVU</c>
    /// </summary>
    Middle,

    /// <summary>
    /// Protect the end. Example result: <c>e0gNHBa90**********</c>
    /// </summary>
    End,
}

/// <summary>Extension methods for <see cref="string"/>.</summary>
public static partial class StringProtectionExtensions
{
    /// <summary>
    /// Protect a value such as an authentication key. Useful before serialization
    /// </summary>
    /// <param name="input"></param>
    /// <param name="toKeep"></param>
    /// <param name="position"></param>
    /// <param name="replacementChar"></param>
    /// <param name="replacementLength"></param>
    /// <returns></returns>
    public static string Protect(this string input,
                                 float toKeep = 0.2f, /* 20% is a good rule of thumb */
                                 StringProtectionPosition position = StringProtectionPosition.End,
                                 char replacementChar = '*',
                                 int? replacementLength = null)
    {
        ArgumentNullException.ThrowIfNull(input);

        var lengthToKeep = Convert.ToInt32(input.Length * toKeep); // consider a minimum
        var lengthToKeepHalf = lengthToKeep / 2;
        var lengthToReplace = input.Length - (lengthToKeepHalf * 2);
        if (replacementLength is not null)
        {
            replacementLength = replacementLength <= 0 ? input.Length : replacementLength;
            lengthToReplace = Math.Min(replacementLength.Value, lengthToReplace);
        }
        var totalWidth = Math.Min(input.Length, lengthToKeep + lengthToReplace);
        return position switch
        {
            StringProtectionPosition.Start => input[^lengthToKeep..].PadLeft(totalWidth, replacementChar),
            StringProtectionPosition.End => input[..lengthToKeep].PadRight(totalWidth, replacementChar),
            StringProtectionPosition.Middle => input[..lengthToKeepHalf] + new string(replacementChar, lengthToReplace) + input[^lengthToKeepHalf..],
            _ => throw new NotSupportedException($"'{nameof(StringProtectionPosition)}.{position}' is not yet supported."),
        };
    }
}
