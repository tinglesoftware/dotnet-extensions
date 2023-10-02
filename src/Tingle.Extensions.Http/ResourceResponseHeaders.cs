using System.Reflection;

namespace Tingle.Extensions.Http;

/// <summary>
/// HTTP Response headers parsed from a <see cref="HttpResponseMessage"/>
/// </summary>
public class ResourceResponseHeaders : Dictionary<string, IEnumerable<string>>
{
    /// <summary>Creates an instance of <see cref="ResourceResponseHeaders"/>.</summary>
    /// <param name="response">The original HTTP response.</param>
    public ResourceResponseHeaders(HttpResponseMessage response) : this(response.Headers.Concat(response.Content.Headers)) { }

    /// <summary>Creates an instance of <see cref="ResourceResponseHeaders"/>.</summary>
    /// <param name="data">The combined headers.</param>
    public ResourceResponseHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> data)
        : this(data.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) { }

    /// <summary>Creates an instance of <see cref="ResourceResponseHeaders"/>.</summary>
    /// <param name="data">The headers.</param>
    public ResourceResponseHeaders(IDictionary<string, IEnumerable<string>> data) : base(data, StringComparer.OrdinalIgnoreCase)
    {
        PopulateKnownHeaders(this);
    }

    /// <summary>Value for <c>X-Continuation-Token</c> header.</summary>
    [KnownHeader("X-Continuation-Token")]
    public virtual string? ContinuationToken { get; private set; }

    /// <summary>Value for <c>X-Trace-Id</c> header.</summary>
    [KnownHeader("X-Trace-Id")]
    public virtual string? TraceId { get; private set; }

    /// <summary>Value for <c>Content-Length</c> header.</summary>
    [KnownHeader("Content-Length")]
    public virtual string? ContentLength { get; private set; }

    /// <summary>Value for <c>Content-Type</c> header.</summary>
    [KnownHeader("Content-Type")]
    public virtual string? ContentType { get; private set; }

    /// <summary>Value for <c></c> header.</summary>
    [KnownHeader("X-Session-Token")]
    public virtual string? SessionToken { get; private set; }

    internal static void PopulateKnownHeaders(ResourceResponseHeaders instance)
    {
        // get the properties
        var type = instance.GetType();
        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var properties = type.GetProperties(flags);

        // work on each property
        foreach (var prop in properties)
        {
            // check if the property is annotated
            var attr = prop.GetCustomAttribute<KnownHeaderAttribute>(false);
            if (attr is not null)
            {
                // set the property value if the header is present
                var headerName = attr.Name;
                if (instance.TryGetValue(headerName, out var values))
                {
                    prop.SetValue(instance, values.FirstOrDefault());
                }
            }
        }
    }
}
