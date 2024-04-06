namespace Tingle.Extensions.Mustache.Contexts;

///
public class InferredValuesContext : BaseTraversalContext<InferredValuesContext>
{
    ///
    public InferredValuesContext(string key) : base(key) { }

    ///
    public InferredValuesContext(string key, InferredValuesContext parent) : base(key, parent) { }

    /// <summary>Usages for this context.</summary>
    public ICollection<InferredUsage> Usages { get; } = new HashSet<InferredUsage>();

    /// <summary>Children owned by this context.</summary>
    private Dictionary<string, InferredValuesContext> Children { get; } = [];

    /// <inheritdoc/>
    protected override InferredValuesContext GetContextForPath(Queue<string> elements, bool ignoreCase)
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
            if (!Children.TryGetValue(element, out var inner))
            {
                inner = new InferredValuesContext(key: element, parent: this);
                Children[element] = inner;
            }
            return inner.GetContextForPath(elements, ignoreCase);
        }
    }

    ///
    public InferredValuesContext GetInferredContextForPath(string path, InferredUsage accessType, bool ignoreCase)
    {
        var context = GetContextForPath(path, ignoreCase);
        context.Usages.Add(accessType);
        return context;
    }

    /// <summary>
    /// Returns an <see cref="object"/> containing the current inferred model representation.
    /// </summary>
    /// <returns></returns>
    public object ToModel()
    {
        object result;
        if (Usages.Count == 0)
        {
            result = Children.ToDictionary(k => k.Key, v => v.Value.ToModel());
        }
        else if (Usages.Contains(InferredUsage.Scalar) && Usages.Count == 1)
        {
            result = Key + "_Value";
        }
        else
        {
            if (Usages.Contains(InferredUsage.Collection))
            {
                if (Children.Count != 0)
                {
                    result = new[] { Children.ToDictionary(k => k.Key, v => v.Value.ToModel()) };
                }
                else
                {
                    result = Enumerable.Range(1, 3).Select(k => Key + "_" + k).ToArray();
                }
            }
            else
            {
                result = Children.ToDictionary(k => k.Key, v => v.Value.ToModel());
            }
        }

        return result;
    }
}
