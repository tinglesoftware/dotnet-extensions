namespace Tingle.Extensions.Mustache.Parsing;

/// <summary>
/// An exception resulting from parsing of a mustache template.
/// </summary>
public class MustacheParsingException(string message) : MustacheException(message)
{
    ///
    public MustacheParsingException(string message, params object[] args)
        : this(string.Format(message, args)) { }

    ///
    public MustacheParsingException(string? sourceName, string message, params object[] args)
        : this(message, args)
    {
        SourceName = sourceName;
    }

    ///
    public MustacheParsingException(string? sourceName, CharacterLocation location, string message, params object[] args)
        : this(sourceName, message, args)
    {
        Location = location;
    }

    ///
    public string? SourceName { get; }

    /// <summary>
    /// Position at which the error occurred.
    /// </summary>
    public CharacterLocation? Location { get; }
}
