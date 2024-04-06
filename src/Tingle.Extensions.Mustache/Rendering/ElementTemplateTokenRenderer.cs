using System.Text;
using Tingle.Extensions.Mustache.Contexts;
using Tingle.Extensions.Mustache.Parsing;

namespace Tingle.Extensions.Mustache.Rendering;

class ElementTemplateTokenRenderer(TemplateToken token,
                                   IReadOnlyList<ITemplateTokenRenderer> renderers,
                                   TemplateRenderingOptions options,
                                   bool inverted = false) : WrappedTemplateTokenRenderer(token, renderers, options)
{
    /// <inheritdoc/>
    public override void Render(StringBuilder builder, ProvidedValuesContext context)
    {
        var c = GetContextForPath(context);

        /*
         * |--------|----------|---------------|
         * | Exists | Inverted | Should Render |
         * |  false |   false  |      false    |
         * |  false |   true   |      true     |
         * |  true  |   false  |      true     |
         * |  false |   true   |      false    |
         * 
         * Conclusion, we only render if both are not equal
         */
        //"falsey" values by Javascript standards...
        var exists = c.Exists();
        if (exists != inverted)
        {
            base.Render(builder, c);
        }
    }
}
