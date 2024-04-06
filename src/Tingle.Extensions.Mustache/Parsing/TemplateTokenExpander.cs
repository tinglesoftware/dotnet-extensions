using System.Text.RegularExpressions;
using Tingle.Extensions.Mustache.Rendering;

namespace Tingle.Extensions.Mustache.Parsing;

/// <summary>
/// An abstraction for customizing expansion of template tokens during tokenization.
/// </summary>
/// <param name="options"></param>
/// <param name="expression">RegEx used to identify the custom token.</param>
/// <param name="precedence"></param>
public abstract class TemplateTokenExpander(TemplateParserOptions options,
                                            Regex expression,
                                            TemplateTokenExpanderPrecedence precedence = TemplateTokenExpanderPrecedence.Medium)
{
    private readonly Regex expression = expression ?? throw new ArgumentNullException(nameof(expression));

    /// <summary>
    /// Options for tokenization.
    /// </summary>
    public TemplateParserOptions Options { get; } = options ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Checks if the expander matches a given raw token.
    /// </summary>
    /// <param name="raw"></param>
    /// <returns></returns>
    protected internal bool Matches(string raw) => expression.IsMatch(raw);

    /// <summary>
    /// Low precedence expanders will be evaluated after all Mustache syntax tokens (e.g.: "each" blocks, groups, etc.).
    /// Medium precedence expanders will be evaluated after "each" blocks and groups, but before unescaped variables {{{ var }}} syntax
    /// High precedence expanders will be evaluated before all Mustache syntax tokens.
    /// The order of the expanders passed in the array in ParsingOptions will be honored when applying them if more granularity is required.
    /// </summary>
    public virtual TemplateTokenExpanderPrecedence Precedence { get; set; } = precedence;

    /// <summary>
    /// Generate new tokens to be used in the parent template.
    /// Note that the custom tokens will be added before the expanded tokens.
    /// </summary>
    /// <param name="raw">The raw string token.</param>
    /// <param name="tokens">Where to add resulting tokens after expansion.</param>
    /// <param name="exceptions">Where to add any parse exceptions if failed.</param>
    /// <returns></returns>
    public abstract bool ExpandTokens(string raw,
                                      ref ICollection<TemplateToken> tokens,
                                      ref ICollection<MustacheParsingException> exceptions);

    /// <summary>
    /// Generates a <see cref="ITemplateTokenRenderer"/> for rendering of
    /// the custom token and following tokens.
    /// <br/>
    /// If the result is null, the custom token will not be rendered at all
    /// and the following tokens will be rendered using the default behaviour.
    /// </summary>
    /// <param name="raw">The raw string token.</param>
    /// <param name="tokens">The generated tokens.</param>
    /// <returns></returns>
    public virtual ITemplateTokenRenderer? GenerateRenderer(string raw, ICollection<TemplateToken> tokens) => null;
}
