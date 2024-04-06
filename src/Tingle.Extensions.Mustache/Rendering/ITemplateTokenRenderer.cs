using System.Text;
using Tingle.Extensions.Mustache.Contexts;
using Tingle.Extensions.Mustache.Parsing;

namespace Tingle.Extensions.Mustache.Rendering;

/// <summary>
/// An abstraction for rendering a custom <see cref="TemplateToken"/> following tokens.
/// </summary>
public interface ITemplateTokenRenderer
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/> in which to render.</param>
    /// <param name="context">Context containing the provided value(s).</param>
    void Render(StringBuilder builder, ProvidedValuesContext context);
}
