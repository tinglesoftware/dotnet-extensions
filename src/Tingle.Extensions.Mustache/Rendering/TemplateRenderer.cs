using System.Text;
using Tingle.Extensions.Mustache.Contexts;
using Tingle.Extensions.Mustache.Parsing;

namespace Tingle.Extensions.Mustache.Rendering;

/// <summary>
/// Renderer for templates
/// </summary>
public class TemplateRenderer
{
    private readonly IReadOnlyList<TemplateToken> tokens;
    private readonly TemplateRenderingOptions options;
    private readonly IReadOnlyList<ITemplateTokenRenderer> renderers;

    private readonly InferredValuesContext? inferred;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <param name="options"></param>
    /// <param name="inference"></param>
    public TemplateRenderer(TemplateParsingResult result, TemplateRenderingOptions options, bool inference = false)
    {
        this.tokens = result.Tokens ?? throw new ArgumentNullException(nameof(result));
        this.options = options ?? throw new ArgumentNullException(nameof(options));

        // Prepare the renderers
        inferred = inference ? new InferredValuesContext(key: "") : null;
        var tokens = new Queue<TemplateToken>(this.tokens);
        renderers = BuildRenderers(tokens, inferred);
    }

    ///
    public string Render(object values)
    {
        // Render the template with the given values
        var context = new ProvidedValuesContext(key: "", value: values); // root object
        var builder = new StringBuilder();
        foreach (var r in renderers)
        {
            r.Render(builder, context);
        }
        return builder.ToString();
    }

    ///
    public IReadOnlyDictionary<string, object?> GetInferredModel()
    {
        if (inferred is null)
        {
            throw new InvalidOperationException("Inferred model representation is only available when inference is enabled on creation.");
        }

        // casting can be forced on the root
        return (Dictionary<string, object?>)inferred.ToModel();
    }

    private List<ITemplateTokenRenderer> BuildRenderers(Queue<TemplateToken> tokens, InferredValuesContext? inferred)
    {
        // Prepare the renderers
        var renderers = new List<ITemplateTokenRenderer>();
        while (tokens.TryDequeue(out var token))
        {
            var remaining = tokens;
            switch (token.Kind)
            {
                case TemplateTokenKind.Content:
                    renderers.Add(new ContentTemplateTokenRenderer(token, options));
                    break;

                case TemplateTokenKind.CollectionOpen:
                    renderers.Add(MakeCollectionRenderer(token, remaining, inferred));
                    break;

                case TemplateTokenKind.ElementOpen:
                    renderers.Add(MakeElementRenderer(token, remaining, inferred, inverted: false));
                    break;

                case TemplateTokenKind.ElementOpenInverted:
                    renderers.Add(MakeElementRenderer(token, remaining, inferred, inverted: true));
                    break;

                case TemplateTokenKind.CollectionClose:
                case TemplateTokenKind.ElementClose:
                    // This should immediately return if we're in the element scope, 
                    // and if we're not, this should have been detected by the tokenizer!
                    return renderers;

                case TemplateTokenKind.SingleValueEscaped:
                case TemplateTokenKind.SingleValueUnescaped:
                    renderers.Add(MakeScalarRenderer(token, inferred));
                    break;

                case TemplateTokenKind.Custom:
                    renderers.Add(token.Renderer ?? throw new InvalidOperationException("A custom token must have a Renderer set."));
                    break;

                case TemplateTokenKind.Comment:
                default:
                    break;
            }
        }

        return renderers;
    }

    private CollectionTemplateTokenRenderer MakeCollectionRenderer(TemplateToken token, Queue<TemplateToken> remaining, InferredValuesContext? inferred)
    {
        inferred = GetInferredContextForPath(inferred, token, InferredUsage.Collection);
        return new CollectionTemplateTokenRenderer(token, BuildRenderers(remaining, inferred), options);
    }

    private ElementTemplateTokenRenderer MakeElementRenderer(TemplateToken token,
                                                       Queue<TemplateToken> remaining,
                                                       InferredValuesContext? inferred,
                                                       bool inverted)
    {
        inferred = GetInferredContextForPath(inferred, token, InferredUsage.ConditionalValue);
        return new ElementTemplateTokenRenderer(token, BuildRenderers(remaining, inferred), options, inverted: inverted);
    }

    private ScalarTemplateTokenRenderer MakeScalarRenderer(TemplateToken token, InferredValuesContext? inferred)
    {
        _ = GetInferredContextForPath(inferred, token, InferredUsage.Scalar);
        return new ScalarTemplateTokenRenderer(token, options);
    }

    private InferredValuesContext? GetInferredContextForPath(InferredValuesContext? context, TemplateToken token, InferredUsage usage)
    {
        return context?.GetInferredContextForPath(token.Value, usage, options.IgnoreCase);
    }
}
