using System.Diagnostics.CodeAnalysis;
using Tingle.Extensions.Mustache.Parsing;
using Tingle.Extensions.Mustache.Rendering;

namespace Tingle.Extensions.Mustache;

/// <summary>
/// Implementation for handling <see href="https://mustache.github.io/">Mustache</see> based text templates.
/// </summary>
public class MustacheTemplate
{
    private static readonly TemplateParser parser = new(new TemplateParserOptions { });

    private readonly string template;
    private readonly TemplateRenderingOptions renderingOptions;
    private readonly bool inference;

    private TemplateRenderer? renderer;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="template"></param>
    /// <param name="ignoreCase">
    /// Determines if case should be ignored when finding values to replace with.
    /// Defaults to <see langword="false"/>.
    /// </param>
    /// <param name="inference">
    /// Determines if the renderer should be created with model inference support.
    /// Defaults to <see inference="false"/>.
    /// </param>
    public MustacheTemplate(string template, bool ignoreCase = false, bool inference = false)
        : this(template, new TemplateRenderingOptions { DisableContentSafety = true, IgnoreCase = ignoreCase, }, inference) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="template"></param>
    /// <param name="renderingOptions"><see cref="TemplateRenderingOptions"/> to use for rending.</param>
    /// <param name="inference">
    /// Determines if the renderer should be created with model inference support.
    /// Defaults to <see inference="true"/>.
    /// </param>
    public MustacheTemplate(string template, TemplateRenderingOptions renderingOptions, bool inference = false)
    {
        if (string.IsNullOrWhiteSpace(this.template = template))
        {
            throw new ArgumentException($"'{nameof(template)}' cannot be null or whitespace.", nameof(template));
        }

        this.renderingOptions = renderingOptions;
        this.inference = inference;
    }

    private TemplateRenderer GetRenderer()
    {
        if (renderer is null)
        {
            var result = parser.Parse(template);
            renderer = new TemplateRenderer(result, renderingOptions, inference);
        }
        return renderer;
    }

    private bool TryGetRenderer([NotNullWhen(true)] out TemplateRenderer? value)
    {
        if (renderer is null)
        {
            if (parser.TryParse(template, out var result))
            {
                renderer = new TemplateRenderer(result, renderingOptions, inference);
                value = renderer;
                return true;
            }
            value = null;
            return false;
        }
        else
        {
            value = renderer;
            return true;
        }
    }

    /// <summary>
    /// Render the template using the provided values.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public MustacheRenderingResult Render(IDictionary<string, object?> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var renderer = GetRenderer();
        var rendered = renderer.Render(values);
        return new MustacheRenderingResult(rendered);
    }

    /// <summary>
    /// Tries to render the template.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="result">The rendered result.</param>
    /// <returns></returns>
    public bool TryRender(IDictionary<string, object?> values, [NotNullWhen(true)] out MustacheRenderingResult? result)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (TryGetRenderer(out var renderer))
        {
            var rendered = renderer.Render(values);
            result = new MustacheRenderingResult(rendered);
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Render the template using provided values or infer where missing.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public MustacheRenderingResult RenderInferred(IReadOnlyDictionary<string, object?>? values = null)
    {
        var renderer = GetRenderer();

        // prepare the template model
        var inferred = renderer.GetInferredModel();

        // combine the model provided and that one inferred to viable model for rendering
        // give preference to the one provided
        var used = Combine(values, inferred);

        // render the template
        var rendered = renderer.Render(used);
        return new MustacheRenderingResult(rendered, used);
    }

    /// <summary>
    /// Tries to render the template using provided values or infer where missing.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="result">The rendered result.</param>
    /// <returns></returns>
    public bool TryRenderInferred(IReadOnlyDictionary<string, object?>? values, [NotNullWhen(true)] out MustacheRenderingResult? result)
    {
        if (TryGetRenderer(out var renderer))
        {
            // prepare the template model
            var inferred = renderer.GetInferredModel();

            // combine the model provided and that one inferred to viable model for rendering
            // give preference to the one provided
            var used = Combine(values, inferred);
            // render the template
            var rendered = renderer.Render(used);
            result = new MustacheRenderingResult(rendered, used);
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Tries to render the template using provided values or infer where missing.
    /// </summary>
    /// <param name="result">The rendered result.</param>
    /// <returns></returns>
    public bool TryRenderInferred([NotNullWhen(true)] out MustacheRenderingResult? result) => TryRenderInferred(values: null, out result);

    /// <summary>
    /// Render the template using the provided values.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public ValueTask<MustacheRenderingResult> RenderAsync(IReadOnlyDictionary<string, object?> values,
                                                          CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(values);
        var renderer = GetRenderer();
        var rendered = renderer.Render(values);
        return new ValueTask<MustacheRenderingResult>(new MustacheRenderingResult(rendered));
    }

    /// <summary>
    /// Render the template using provided values or infer where missing.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public ValueTask<MustacheRenderingResult> RenderInferredAsync(IReadOnlyDictionary<string, object?>? values = null,
                                                                  CancellationToken cancellationToken = default)
    {
        var renderer = GetRenderer();

        // prepare the template model
        var inferred = renderer.GetInferredModel();

        // combine the model provided and that one inferred to viable model for rendering
        // give preference to the one provided
        var used = Combine(values, inferred);

        // render the template
        var rendered = renderer.Render(used);
        return new ValueTask<MustacheRenderingResult>(new MustacheRenderingResult(rendered, used));
    }

    private Dictionary<string, object?> Combine(IReadOnlyDictionary<string, object?>? provided, IReadOnlyDictionary<string, object?>? inferred)
    {
        var list = new List<KeyValuePair<string, object?>>();
        if (provided is not null) list.AddRange(provided); // prefer the provided values
        if (inferred is not null) list.AddRange(inferred);

        var comparer = renderingOptions.IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

        return list.GroupBy(kvp => kvp.Key, comparer)
                   .ToDictionary(keySelector: g => g.Key,
                                 elementSelector: g =>
                                 {
                                     var count = g.Count();
                                     if (count == 1) return g.Single().Value;
                                     if (count == 2)
                                     {
                                         var providedValue = g.First().Value;
                                         var inferredValue = g.Last().Value;
                                         if (providedValue is IReadOnlyDictionary<string, object?> p1
                                             && inferredValue is IReadOnlyDictionary<string, object?> p2)
                                         {
                                             return Combine(p1, p2);
                                         }

                                         // if we have either but not both, then return the one we have
                                         if (providedValue is null || inferredValue is null)
                                         {
                                             return providedValue ?? inferredValue;
                                         }

                                         // When the types are the same, prioritize the provided one.
                                         // This will handle primitives e.g. string, int, etc.
                                         if (providedValue.GetType() == inferredValue.GetType())
                                         {
                                             return providedValue;
                                         }

                                         // At this point the provided value has wrong type
                                         // So we use the inferred one.
                                         return inferredValue;
                                     }

                                     // TODO: ask Kimani what to do here
                                     return g.First().Value;
                                 },
                                 comparer: comparer);
    }

    /// <summary>
    /// Create from a template in a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream"><see cref="Stream"/> containing the template.</param>
    /// <returns></returns>
    public static MustacheTemplate Create(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var sr = new StreamReader(stream);
        var content = sr.ReadToEnd();
        return new MustacheTemplate(content);
    }

    /// <summary>
    /// Create from a template in a <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream"><see cref="Stream"/> containing the template.</param>
    /// <returns></returns>
    public static async Task<MustacheTemplate> CreateAsync(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var sr = new StreamReader(stream);
        var content = await sr.ReadToEndAsync().ConfigureAwait(false);
        return new MustacheTemplate(content);
    }
}
