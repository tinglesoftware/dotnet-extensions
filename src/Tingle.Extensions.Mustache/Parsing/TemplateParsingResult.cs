namespace Tingle.Extensions.Mustache.Parsing;

///
public readonly struct TemplateParsingResult(IReadOnlyList<TemplateToken> tokens, IReadOnlyList<MustacheParsingException> errors)
{
    /// <summary>
    /// The tokens extracted for processing.
    /// </summary>
    internal IReadOnlyList<TemplateToken> Tokens { get; } = tokens ?? throw new ArgumentNullException(nameof(tokens));

    /// <summary>
    /// The exceptions generated.
    /// </summary>
    public IReadOnlyList<MustacheParsingException> Errors { get; } = errors ?? throw new ArgumentNullException(nameof(errors));
}
