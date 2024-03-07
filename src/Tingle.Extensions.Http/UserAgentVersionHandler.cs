using System.Net.Http.Headers;

namespace Tingle.Extensions.Http;

internal class UserAgentVersionHandler : DelegatingHandler
{
    private readonly string name;
    private readonly string version;
    private readonly bool clear;

    public UserAgentVersionHandler(string name, string version, bool clear)
    {
        if (string.IsNullOrWhiteSpace(this.name = name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(this.version = version))
        {
            throw new ArgumentException($"'{nameof(version)}' cannot be null or whitespace.", nameof(version));
        }

        this.clear = clear;
    }

    /// <inheritdoc/>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (clear) request.Headers.UserAgent.Clear();

        // populate the User-Agent header
        var userAgent = new ProductInfoHeaderValue(name, version);
        request.Headers.UserAgent.Add(userAgent);

        // execute the request
        return base.SendAsync(request, cancellationToken);
    }
}
