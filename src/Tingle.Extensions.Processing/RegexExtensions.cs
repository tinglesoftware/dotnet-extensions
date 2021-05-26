namespace System.Text.RegularExpressions
{
    /// <summary>
    /// Extension methods for <see cref="Regex"/>
    /// </summary>
    public static class RegexExtensions
    {
        /// <summary>
        /// Searches the specified input string for the first occurrence of the regular expression
        /// specified in the <see cref="Regex"/> constructor.
        /// </summary>
        /// <param name="regex">the instance to use</param>
        /// <param name="input">the string to search for a match.</param>
        /// <param name="match"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">the input is null</exception>
        /// <exception cref="RegexMatchTimeoutException">a time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public static bool Match(this Regex regex, string input, out Match match)
        {
            match = regex.Match(input);
            return match.Success;
        }
    }
}
