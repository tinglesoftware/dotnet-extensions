using System.Text;
using Tingle.Extensions.Mustache.Contexts;
using Tingle.Extensions.Mustache.Parsing;

namespace Tingle.Extensions.Mustache.Rendering;

class ContentTemplateTokenRenderer(TemplateToken token, TemplateRenderingOptions options) : AbstractTemplateTokenRenderer(token, options)
{
    /// <inheritdoc/>
    public override void Render(StringBuilder builder, ProvidedValuesContext context)
    {
        builder.Append(Token.Value);
    }
}
