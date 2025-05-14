using System.Text.RegularExpressions;

namespace Tingle.Extensions.Mustache.Parsing;

/// <summary>
/// Logic for generating <see cref="TemplateToken"/>s from a template.
/// </summary>
internal partial class TemplateTokenizer
{
    private readonly string template;
    private readonly string? sourceName;
    private readonly IEnumerable<TemplateTokenExpander> expanders;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="template">The <see cref="string"/> to tokenize.</param>
    /// <param name="sourceName"></param>
    /// <param name="expanders"></param>
    public TemplateTokenizer(string template, string? sourceName, IEnumerable<TemplateTokenExpander> expanders)
    {
        if (string.IsNullOrWhiteSpace(this.template = template))
        {
            throw new ArgumentException($"'{nameof(template)}' cannot be null or whitespace.", nameof(template));
        }

        this.sourceName = sourceName;
        this.expanders = expanders ?? throw new ArgumentNullException(nameof(expanders));
    }

    /// <summary>Generate tokens from the template string.</summary>
    public void Generate(ICollection<TemplateToken> tokens, ICollection<MustacheParsingException> exceptions)
    {
        GroupExpandersByPrecedence(expanders, out var lowExpanders, out var mediumExpanders, out var highExpanders);

        int[]? lines = null;
        var scopes = new Stack<Tuple<string, int>>();

        var matches = GetTokenFinderFormat().Matches(template).OfType<Match>().ToList();
        var index = 0;
        foreach (var match in matches)
        {
            var matchIndex = match.Index;
            var matchValue = match.Value;
            var matchLength = match.Length;

            // yield preceding content.
            if (matchIndex > index)
            {
                tokens.Add(TemplateToken.Content(template[index..matchIndex]));
            }

            // Work on the matched token now

            if (ExpandTokens(highExpanders, matchValue, tokens, exceptions))
            {
                // already tokenized; do nothing;
            }
            else if (matchValue.StartsWith("{{#each"))
            {
                scopes.Push(Tuple.Create(matchValue, matchIndex));
                var token = matchValue.TrimStart('{').TrimEnd('}').TrimStart('#').Trim();
                token = token[4..];

                if (token.StartsWith(' ') && token.Trim() != "")
                {
                    tokens.Add(new TemplateToken(TemplateTokenKind.CollectionOpen, Validate(token, matchIndex, ref lines, exceptions)));
                }
                else
                {
                    var location = HumanizeLocation(matchIndex, ref lines);
                    exceptions.Add(
                        new MustacheParsingException(
                            sourceName,
                            location,
                            @"The 'each' block being opened requires a model path to be specified in the form '{{{{#each <name>}}}}'."));
                }
            }
            else if (matchValue.StartsWith("{{/each"))
            {
                if (matchValue != "{{/each}}")
                {
                    var location = HumanizeLocation(matchIndex, ref lines);
                    exceptions.Add(
                        new MustacheParsingException(
                            sourceName,
                            location,
                            @"The syntax to close the 'each' block should be: '{{{{/each}}}}'."));
                }
                else if (scopes.Count != 0 && scopes.Peek().Item1.StartsWith("{{#each"))
                {
                    var token = scopes.Pop().Item1;
                    tokens.Add(new TemplateToken(TemplateTokenKind.CollectionClose, token));
                }
                else
                {
                    var location = HumanizeLocation(matchIndex, ref lines);
                    exceptions.Add(
                        new MustacheParsingException(
                            sourceName,
                            location,
                            @"An 'each' block is being closed, but no corresponding opening element ('{{{{#each <name>}}}}') was detected."));
                }
            }
            else if (matchValue.StartsWith("{{#"))
            {
                // open group
                var token = matchValue.TrimStart('{').TrimEnd('}').TrimStart('#').Trim();
                if (scopes.Count != 0 && scopes.Peek().Item1 == token)
                {
                    tokens.Add(new TemplateToken(TemplateTokenKind.ElementClose, Validate(token, matchIndex, ref lines, exceptions)));
                }
                else
                {
                    scopes.Push(Tuple.Create(token, matchIndex));
                }
                tokens.Add(new TemplateToken(TemplateTokenKind.ElementOpen, Validate(token, matchIndex, ref lines, exceptions)));
            }
            else if (matchValue.StartsWith("{{^"))
            {
                // open inverted group
                var token = matchValue.TrimStart('{').TrimEnd('}').TrimStart('^').Trim();

                if (scopes.Count != 0 && scopes.Peek().Item1 == token)
                {
                    tokens.Add(new TemplateToken(TemplateTokenKind.ElementClose, Validate(token, matchIndex, ref lines, exceptions)));
                }
                else
                {
                    scopes.Push(Tuple.Create(token, matchIndex));
                }
                tokens.Add(new TemplateToken(TemplateTokenKind.ElementOpenInverted, Validate(token, matchIndex, ref lines, exceptions)));
            }
            else if (matchValue.StartsWith("{{/"))
            {
                var token = matchValue.TrimStart('{').TrimEnd('}').TrimStart('/').Trim();
                // close group
                if (scopes.Count != 0 && scopes.Peek().Item1 == token)
                {
                    scopes.Pop();
                    tokens.Add(new TemplateToken(TemplateTokenKind.ElementClose, Validate(token, matchIndex, ref lines, exceptions)));
                }
                else
                {
                    var location = HumanizeLocation(matchIndex, ref lines);
                    exceptions.Add(
                        new MustacheParsingException(
                            sourceName,
                            location,
                            "It appears that open and closing elements are mismatched."));
                }
            }
            else if (ExpandTokens(mediumExpanders, matchValue, tokens, exceptions))
            {
                // already tokenized; do nothing;
            }
            else if (matchValue.StartsWith("{{{") | matchValue.StartsWith("{{&"))
            {
                // escaped single element
                var token = matchValue.TrimStart('{').TrimEnd('}').TrimStart('&').Trim();
                tokens.Add(new TemplateToken(TemplateTokenKind.SingleValueUnescaped, Validate(token, matchIndex, ref lines, exceptions)));
            }
            else if (matchValue.StartsWith("{{!"))
            {
                //it's a comment drop this on the floor, no need to even yield it.
            }
            else if (ExpandTokens(lowExpanders, matchValue, tokens, exceptions))
            {
                // already tokenized; do nothing;
            }
            else
            {
                // un-single value.
                var token = matchValue.TrimStart('{').TrimEnd('}').Trim();
                tokens.Add(new TemplateToken(TemplateTokenKind.SingleValueEscaped, Validate(token, matchIndex, ref lines, exceptions)));
            }

            // move forward in the string.
            index = matchIndex + matchLength;
        }

        // Add remaining content if present
        if (index < template.Length)
        {
            tokens.Add(TemplateToken.Content(template[index..]));
        }

        // Check for any scopes opened but not closed.
        if (scopes.Count != 0)
        {
            var unclosedScopes = scopes.Select(k =>
            {
                var value = k.Item1.Trim('{', '#', '}');
                if (value.StartsWith("each ")) value = value[5..];
                return new { scope = value, location = HumanizeLocation(k.Item2, ref lines) };
            });

            unclosedScopes = unclosedScopes.Reverse().ToArray();

            foreach (var unclosed in unclosedScopes)
            {
                var ex = new MustacheParsingException(
                    sourceName,
                    unclosed.location,
                    "A scope block to the following path was opened but not closed: '{0}', please close it using the appropriate syntax.",
                    unclosed.scope);
                exceptions.Add(ex);
            }
        }
    }

    private static void GroupExpandersByPrecedence(IEnumerable<TemplateTokenExpander> expanders,
                                                   out HashSet<TemplateTokenExpander> lowExpanders,
                                                   out HashSet<TemplateTokenExpander> mediumExpanders,
                                                   out HashSet<TemplateTokenExpander> highExpanders)
    {
        ArgumentNullException.ThrowIfNull(expanders);

        lowExpanders = [];
        mediumExpanders = [];
        highExpanders = [];

        foreach (var ex in expanders)
        {
            var pr = ex.Precedence;
            var collection = pr switch
            {
                TemplateTokenExpanderPrecedence.Low => lowExpanders,
                TemplateTokenExpanderPrecedence.Medium => mediumExpanders,
                TemplateTokenExpanderPrecedence.High => highExpanders,
                _ => throw new InvalidOperationException($"{nameof(TemplateTokenExpanderPrecedence)}.{pr} is not supported."),
            };

            collection.Add(ex);
        }
    }

    private static bool ExpandTokens(ICollection<TemplateTokenExpander> expanders,
                                     string raw,
                                     ICollection<TemplateToken> tokens,
                                     ICollection<MustacheParsingException> parseErrors)
    {
        if (expanders.Count == 0) return false;

        var expander = expanders.FirstOrDefault(e => e.Matches(raw));
        if (expander == null) return false;

        var renderer = expander.GenerateRenderer(raw, tokens);
        if (renderer != null)
        {
            tokens.Add(new TemplateToken(TemplateTokenKind.Custom, raw, renderer));
        }

        // do the expansion
        var success = expander.ExpandTokens(raw, ref tokens, ref parseErrors);
        return true; // Should we return the success?
    }

    private string Validate(string token, int index, ref int[]? lines, ICollection<MustacheParsingException> exceptions)
    {
        token = token.Trim();

        if (GetNegativePathSpec().Match(token).Success)
        {
            var location = HumanizeLocation(index, ref lines);
            var ex = new MustacheParsingException(
                sourceName: sourceName,
                location: location,
                message: "The path '{0}' is not valid. Please see documentation for examples of valid paths.",
                args: token);
            exceptions.Add(ex);
        }

        return token;
    }

    private CharacterLocation HumanizeLocation(int characterIndex, ref int[]? lines)
    {
        lines ??= [.. GetNewlineFinder().Matches(template).OfType<Match>().Select(k => k.Index)];

        var line = Array.BinarySearch(lines, characterIndex);
        line = line < 0 ? ~line : line;

        //in both of these cases, we want to increment the char index by one to account for the '\n' that is skipped in the indexes.
        var index = characterIndex;
        if (line < lines.Length && line > 0)
        {
            index = characterIndex - (lines[line - 1] + 1);
        }
        else if (line > 0)
        {
            index = characterIndex - (lines.LastOrDefault() + 1);
        }

        // Humans count from 1, so let's do that, too (hence the "+1" on these).
        return new CharacterLocation(line + 1, index + 1);
    }

    [GeneratedRegex("([{]{2}[^{}]+?[}]{2})|([{]{3}[^{}]+?[}]{3})", RegexOptions.Compiled)]
    private static partial Regex GetTokenFinderFormat();
    [GeneratedRegex("\n", RegexOptions.Compiled)]
    private static partial Regex GetNewlineFinder();
    [GeneratedRegex("([.]{3,})|([^\\w./_]+)|((?<![.]{2})[/])|([.]{2,}($|[^/]))", RegexOptions.Compiled | RegexOptions.Singleline)] //combinations of paths that don't work
    private static partial Regex GetNegativePathSpec();
}
