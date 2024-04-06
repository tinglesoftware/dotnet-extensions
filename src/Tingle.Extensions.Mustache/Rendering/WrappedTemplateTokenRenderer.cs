using System.Text;
using Tingle.Extensions.Mustache.Contexts;
using Tingle.Extensions.Mustache.Parsing;

namespace Tingle.Extensions.Mustache.Rendering;

abstract class WrappedTemplateTokenRenderer(TemplateToken token,
                                            IReadOnlyList<ITemplateTokenRenderer> renderers,
                                            TemplateRenderingOptions options) : AbstractTemplateTokenRenderer(token, options)
{
    private readonly IReadOnlyList<ITemplateTokenRenderer> renderers = renderers ?? throw new ArgumentNullException(nameof(renderers));

    /// <inheritdoc/>
    public override void Render(StringBuilder builder, ProvidedValuesContext context) => RenderInternal(builder, context, renderers);

    protected static void RenderInternal(StringBuilder builder, ProvidedValuesContext context, IReadOnlyList<ITemplateTokenRenderer> renderers)
    {
        foreach (var r in renderers)
        {
            r.Render(builder, context);
        }
    }
}
