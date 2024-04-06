using System.Reflection;

namespace Tingle.Extensions.Mustache.Contexts;

/// <summary>
/// 
/// </summary>
public class ProvidedValuesContext : BaseTraversalContext<ProvidedValuesContext>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key">The key for access.</param>
    /// <param name="value">The value associated with this context.</param>
    public ProvidedValuesContext(string key, object value) : base(key)
    {
        Value = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key">The key for access.</param>
    /// <param name="value">The value associated with this context.</param>
    /// <param name="parent">The owning context.</param>
    public ProvidedValuesContext(string key, object value, ProvidedValuesContext parent) : base(key, parent)
    {
        Value = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key">The key for access.</param>
    /// <param name="parent">The owning context.</param>
    public ProvidedValuesContext(string key, ProvidedValuesContext parent) : base(key, parent) { }

    /// <summary>
    /// The value associated with this context.
    /// </summary>
    public object? Value { get; set; }

    /// <summary>Determines if the value of this context exists.</summary>
    public bool Exists() => Value is not null;

    /// <inheritdoc/>
    protected override ProvidedValuesContext GetContextForPath(Queue<string> elements, bool ignoreCase)
    {
        ArgumentNullException.ThrowIfNull(elements);
        if (elements.Count == 0) return this;

        var element = elements.Dequeue();
        if (element.StartsWith(".."))
        {
            if (Parent is not null)
            {
                return Parent.GetContextForPath(elements, ignoreCase);
            }
            else
            {
                // calling "../" too much may be okay in that if we're at root,
                // we may just stop recursion and traverse down the path.
                return GetContextForPath(elements, ignoreCase);
            }
        }
        // TODO: handle array accessor and maybe "special" keys
        else
        {
            // always return the context, even if the value is null
            ProvidedValuesContext? inner = null;
            if (Value is IDictionary<string, object> ctx)
            {
                if (ignoreCase)
                {
                    ctx = new Dictionary<string, object>(ctx, StringComparer.OrdinalIgnoreCase);
                }

                ctx.TryGetValue(element, out var innerV);
                inner = new ProvidedValuesContext(key: element, value: innerV!, parent: this);
            }
            else if (Value is not null)
            {
                var type = Value.GetType();
                var flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
                if (ignoreCase) flags |= BindingFlags.IgnoreCase;
                var prop = type.GetProperty(element, flags);
                if (prop is not null)
                {
                    var innerV = prop.GetValue(Value);
                    inner = new ProvidedValuesContext(key: element, value: innerV!, parent: this);
                }
            }

            //return inner ?? new ProvidedValuesContext(key: element, parent: this);
            inner ??= new ProvidedValuesContext(key: element, parent: this);
            return inner.GetContextForPath(elements, ignoreCase);
        }
    }
}
