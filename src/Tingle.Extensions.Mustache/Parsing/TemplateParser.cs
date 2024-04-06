namespace Tingle.Extensions.Mustache.Parsing;

/// <summary>
/// Logic for parsing a <see cref="string"/> template.
/// </summary>
/// <param name="options"></param>
public class TemplateParser(TemplateParserOptions options)
{
    private readonly TemplateParserOptions options = options ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Parse a template string.
    /// </summary>
    /// <param name="template">The <see cref="string"/> to parse.</param>
    /// <returns>The tokens enqueued for processing.</returns>
    /// <exception cref="MustacheParsingException">If parsing encountered an error.</exception>
    public TemplateParsingResult Parse(string template) => TryParse(template, out var result) ? result : throw result.Errors[0];

    /// <summary>
    /// Attempts to parse a template string.
    /// </summary>
    /// <param name="template">The <see cref="string"/> to parse.</param>
    /// <param name="result">The result of parsing.</param>
    /// <returns></returns>
    public bool TryParse(string template, out TemplateParsingResult result)
    {
        var tokenizer = new TemplateTokenizer(template, options.SourceName, options.TokenExpanders);
        var tokens = new List<TemplateToken>();
        var errors = new List<MustacheParsingException>();
        tokenizer.Generate(tokens, errors);
        result = new TemplateParsingResult(tokens, errors);
        return errors.Count <= 0;
    }
}
