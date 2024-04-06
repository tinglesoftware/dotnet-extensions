using System.Collections;
using System.Text;
using Tingle.Extensions.Mustache.Contexts;
using Tingle.Extensions.Mustache.Parsing;

namespace Tingle.Extensions.Mustache.Rendering;

class CollectionTemplateTokenRenderer(TemplateToken token,
                                      IReadOnlyList<ITemplateTokenRenderer> renderers,
                                      TemplateRenderingOptions options) : WrappedTemplateTokenRenderer(token, renderers, options)
{
    public override void Render(StringBuilder builder, ProvidedValuesContext context)
    {
        //if we're in the same scope, just negating, then we want to use the same object
        var c = GetContextForPath(context);

        //"falsey" values by Javascript standards...
        if (!c.Exists()) return;

        IEnumerable? enumerable;
        if (c.Value is IEnumerable e and not string and not IDictionary<string, object>)
        {
            enumerable = e;
        }
        else
        {
            // Ok, this is a scalar value or an Object. So lets box it into an IEnumerable
            enumerable = new[] { c.Value };
        }

        var index = 0;
        foreach (object i in enumerable)
        {
            var inner = new ProvidedValuesContext(key: $"[{index}]", value: i, parent: c);
            base.Render(builder, inner);
            index++;
        }
    }
}
