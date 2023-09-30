using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Tingle.Extensions.Http.Tests;

public class ResourceResponseHeadersTests
{
    [Fact]
    public void HeadersParsedFromResponse()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NoContent);
        response.Headers.Date = DateTimeOffset.UtcNow;
        response.Headers.Server.Add(new ProductInfoHeaderValue("ServerX", "1.0"));
        response.Headers.TryAddWithoutValidation("x-trace-id", Guid.NewGuid().ToString());
        response.Headers.TryAddWithoutValidation("x-continuation-token", Guid.NewGuid().ToString());
        response.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("me")));
        response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain;charset=utf-8");
        response.Content.LoadIntoBufferAsync(); // required to populate Content-Length header

        var rr = new ResourceResponse<object>(response);
        Assert.NotNull(rr.Headers);

        Assert.NotNull(rr.Headers.ContentLength);
        Assert.NotNull(rr.Headers.ContentType);
        Assert.NotNull(rr.Headers.ContinuationToken);
        Assert.NotNull(rr.Headers.TraceId);
        Assert.Null(rr.Headers.SessionToken);
    }
}
