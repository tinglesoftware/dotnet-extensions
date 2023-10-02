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
        string? GetHeaderValue(string headerName) => TryGetValue(headerName, out var values) ? values.FirstOrDefault() : default;

        ContentLength = GetHeaderValue("Content-Length");
        ContentType = GetHeaderValue("Content-Type");
        TraceId = GetHeaderValue("Trace-Id") ?? GetHeaderValue("X-Trace-Id");
        RequestId = GetHeaderValue("Request-Id") ?? GetHeaderValue("X-Request-Id");
        ContinuationToken = GetHeaderValue("Continuation-Token") ?? GetHeaderValue("X-Continuation-Token");
        SessionToken = GetHeaderValue("Session-Token") ?? GetHeaderValue("X-Session-Token");
    }

    /// <summary>Value for <c>Content-Length</c> header.</summary>
    public virtual string? ContentLength { get; }

    /// <summary>Value for <c>Content-Type</c> header.</summary>
    public virtual string? ContentType { get; }

    /// <summary>Value for <c>Trace-Id</c> or <c>X-Trace-Id</c> header.</summary>
    public virtual string? TraceId { get; }

    /// <summary>Value for <c>Request-Id</c> or <c>X-Request-Id</c> header.</summary>
    public virtual string? RequestId { get; }

    /// <summary>Value for <c>Continuation-Token</c> or <c>X-Continuation-Token</c> header.</summary>
    public virtual string? ContinuationToken { get; }

    /// <summary>Value for <c>Session-Token</c> or <c>X-Session-Token</c> header.</summary>
    public virtual string? SessionToken { get; }
}
