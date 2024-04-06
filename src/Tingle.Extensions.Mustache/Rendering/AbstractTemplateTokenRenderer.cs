using System.Text;
using Tingle.Extensions.Mustache.Contexts;
using Tingle.Extensions.Mustache.Parsing;

namespace Tingle.Extensions.Mustache.Rendering;

///
abstract class AbstractTemplateTokenRenderer(TemplateToken token, TemplateRenderingOptions options) : ITemplateTokenRenderer
{
    ///
    protected TemplateToken Token { get; } = token;

    ///
    protected TemplateRenderingOptions Options { get; } = options ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc/>
    public abstract void Render(StringBuilder builder, ProvidedValuesContext context);

    protected ProvidedValuesContext GetContextForPath(ProvidedValuesContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.GetContextForPath(Token.Value, Options.IgnoreCase);
    }
}
