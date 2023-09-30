namespace Tingle.Extensions.Http.Tests;

public class DynamicHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> processFunc;

    public DynamicHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> processFunc)
    {
        this.processFunc = (req, ct) => Task.FromResult(processFunc(req, ct));
    }

    public DynamicHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> processFunc)
    {
        this.processFunc = processFunc ?? throw new ArgumentNullException(nameof(processFunc));
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return processFunc(request, cancellationToken);
    }
}
