using System.Text;
using System.Web;
using Tingle.Extensions.Mustache.Contexts;
using Tingle.Extensions.Mustache.Parsing;

namespace Tingle.Extensions.Mustache.Rendering;

class ScalarTemplateTokenRenderer(TemplateToken token, TemplateRenderingOptions options) : AbstractTemplateTokenRenderer(token, options)
{
    /// <inheritdoc/>
    public override void Render(StringBuilder builder, ProvidedValuesContext context)
    {
        // try to locate the value in the context, if it exists, append it.
        var c = GetContextForPath(context);
        var value = c.Value;
        if (value is not null)
        {
            // get the string value
            var formatted = value.ToString();

            // if the value is escaped and content safety is not disabled, encode the formatted value.
            if (Token.Kind == TemplateTokenKind.SingleValueEscaped && !Options.DisableContentSafety)
            {
                formatted = HttpUtility.HtmlEncode(formatted);
            }

            builder.Append(formatted);
        }
    }
}
