using System.Text.RegularExpressions;

namespace Tingle.Extensions.Mustache.Contexts;

/// <param name="key">The key for access.</param>
public abstract partial class BaseTraversalContext<T>(string key) : ITraversalContext where T : ITraversalContext
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key">The key for access.</param>
    /// <param name="parent">The owning context.</param>
    public BaseTraversalContext(string key, T parent) : this(key)
    {
        Parent = parent;
    }

    /// <inheritdoc/>
    public string Key { get; } = key;

    /// <summary>
    /// The owning context.
    /// </summary>
    public T? Parent { get; }

    ///
    protected abstract T GetContextForPath(Queue<string> elements, bool ignoreCase);

    ///
    protected internal virtual T GetContextForPath(string path, bool ignoreCase)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
        }

        var elements = new Queue<string>();
        var matches = GetPathFinderFormat().Matches(path).OfType<Match>().ToList();
        foreach (var m in matches)
        {
            elements.Enqueue(m.Value);
        }

        return GetContextForPath(elements, ignoreCase);
    }

    [GeneratedRegex("(\\.\\.[\\\\/]{1})|([^.]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline, "en-KE")]
    private static partial Regex GetPathFinderFormat();
}
