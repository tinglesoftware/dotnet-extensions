using System.Net;

namespace Tingle.Extensions.Http.Authentication.Tests;

internal class DynamicHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> processFunc;

    public DynamicHttpMessageHandler(HttpStatusCode statusCode = HttpStatusCode.OK) : this((req, ct) => new HttpResponseMessage(statusCode)) { }

    public DynamicHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> processFunc)
    {
        this.processFunc = (req, ct) => Task.FromResult(processFunc(req, ct));
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return processFunc(request, cancellationToken);
    }
}
