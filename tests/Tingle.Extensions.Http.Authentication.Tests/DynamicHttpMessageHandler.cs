using System.Net;

namespace Tingle.Extensions.Http.Authentication.Tests;

internal class DynamicHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> processFunc) : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> processFunc = (req, ct) => Task.FromResult(processFunc(req, ct));

    public DynamicHttpMessageHandler(HttpStatusCode statusCode = HttpStatusCode.OK) : this((req, ct) => new HttpResponseMessage(statusCode)) { }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return processFunc(request, cancellationToken);
    }
}
