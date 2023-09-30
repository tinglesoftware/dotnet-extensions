using System.Net;

namespace Tingle.Extensions.Http.Tests;

public class ResourceResponseTests
{
    [Fact]
    public void EnsureSuccess_Works()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        response.Headers.Date = DateTimeOffset.UtcNow;
        response.Content = new StringContent(string.Empty);

        var rr = new ResourceResponse<object>(response);
        Assert.Equal(response, rr.Response);

        Assert.Equal(HttpStatusCode.NotFound, rr.StatusCode);
        Assert.False(rr.IsSuccessful);

        // change to code to test IsSuccessful
        response.StatusCode = HttpStatusCode.Accepted;
        Assert.Equal(HttpStatusCode.Accepted, rr.StatusCode);
        Assert.True(rr.IsSuccessful);

        // change to code to test IsUnauthorized
        response.StatusCode = HttpStatusCode.Unauthorized;
        Assert.Equal(HttpStatusCode.Unauthorized, rr.StatusCode);
        Assert.False(rr.IsSuccessful);

        // change to code to test IsUnavailable
        response.StatusCode = HttpStatusCode.ServiceUnavailable;
        Assert.Equal(HttpStatusCode.ServiceUnavailable, rr.StatusCode);
        Assert.False(rr.IsSuccessful);
    }

    [Fact]
    public void EnsureSuccess_DoesNot_Throw_Exception()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Accepted);
        response.Headers.Date = DateTimeOffset.UtcNow;
        response.Content = new StringContent(string.Empty);

        var rr = new ResourceResponse<object>(response);
        rr.EnsureSuccess();
    }

    [Fact]
    public void EnsureSuccess_Throws_Exception()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        response.Headers.Date = new DateTimeOffset(DateTimeOffset.UtcNow.Date, TimeSpan.Zero);
        response.Content = new StringContent(string.Empty);

        var options = new DummyHttpApiClientOptions
        {
            IncludeHeadersInExceptionMessage = true,
            IncludeRawBodyInExceptionMessage = true,
        };

        var rr = new ResourceResponse<object>(response, options);
        Assert.Equal(HttpStatusCode.NotFound, rr.StatusCode);

        var message = "The HTTP request failed with code 404 (NotFound)\n"
                   + $"\nHeaders:\n{{\"Date\":[\"{response.Headers.Date:r}\"],\"Content-Type\":[\"text/plain; charset=utf-8\"]}}\n"
                    + "\nBody:\n";
        var ex = Assert.Throws<HttpApiResponseException>(rr.EnsureSuccess);
        Assert.Equal(message, ex.Message);

        Assert.NotNull(ex.Response);
        Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
        Assert.Equal(404, ex.ResponseCode);
        Assert.Null(ex.Resource);
        Assert.Null(ex.Problem);
        Assert.NotNull(ex.Headers);
        Assert.NotEmpty(ex.Headers);
        var h1 = ex.Headers!.First();
        Assert.Equal("Date", h1.Key);
        var date_str = Assert.Single(h1.Value);
        var date = DateTimeOffset.Parse(date_str);
        Assert.Equal(response.Headers.Date, date);
    }

    [Fact]
    public void EnsureHasResource_DoesNot_Throw_Exception()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Accepted);
        response.Headers.Date = DateTimeOffset.UtcNow;
        response.Content = new StringContent(string.Empty);

        var rr = new ResourceResponse<object>(response, resource: new());
        rr.EnsureHasResource();
    }

    [Fact]
    public void EnsureHasResource_Throws_Exception()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Accepted);
        response.Headers.Date = new DateTimeOffset(DateTimeOffset.UtcNow.Date, TimeSpan.Zero);
        response.Content = new StringContent(string.Empty);

        var rr = new ResourceResponse<object>(response);

        var ex = Assert.Throws<HttpApiResponseException>(rr.EnsureHasResource);
        Assert.Equal("The HTTP response body was either null or empty.", ex.Message);

        Assert.NotNull(ex.Response);
        Assert.Equal(HttpStatusCode.Accepted, ex.StatusCode);
        Assert.Equal(202, ex.ResponseCode);
        Assert.Null(ex.Resource);
        Assert.Null(ex.Problem);
        Assert.NotNull(ex.Headers);
        Assert.NotEmpty(ex.Headers);
        var h1 = ex.Headers!.First();
        Assert.Equal("Date", h1.Key);
        var date_str = Assert.Single(h1.Value);
        var date = DateTimeOffset.Parse(date_str);
        Assert.Equal(response.Headers.Date, date);
    }

    [Fact]
    public void ContinuationToken_MustNotBe_Null()
    {
        var ct = Guid.NewGuid().ToString();
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        response.Headers.Date = new DateTimeOffset(DateTimeOffset.UtcNow.Date, TimeSpan.Zero);
        response.Headers.TryAddWithoutValidation("x-continuation-token", ct);
        response.Content = new StringContent(string.Empty);

        var rr = new ResourceResponse<object>(response);
        Assert.Equal(ct, rr.ContinuationToken);
    }

    [Fact]
    public void ContinuationToken_MustBe_Null()
    {
        // first try when the header is not there
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        response.Headers.Date = new DateTimeOffset(DateTimeOffset.UtcNow.Date, TimeSpan.Zero);
        response.Content = new StringContent(string.Empty);

        var rr = new ResourceResponse<object>(response);
        Assert.Null(rr.ContinuationToken);


        // now try with wrongly spelled headers
        var ct = Guid.NewGuid().ToString();
        response.Headers.TryAddWithoutValidation("x-ms-continuation", ct); // for Microsoft
        response.Headers.TryAddWithoutValidation("x-ts-continuation2", ct); // mumbled

        rr = new ResourceResponse<object>(response);
        Assert.Null(rr.ContinuationToken);
    }

    [Fact]
    public void HasMoreResults_MustBe_Null()
    {
        // first try when the header is not there
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        response.Headers.Date = new DateTimeOffset(DateTimeOffset.UtcNow.Date, TimeSpan.Zero);
        response.Content = new StringContent(string.Empty);

        var rr = new ResourceResponse<object>(response);
        Assert.Null(rr.HasMoreResults);
    }

    [Fact]
    public void HasMoreResults_MustNotBe_Null()
    {
        // first try when the header is not there
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        response.Headers.Date = new DateTimeOffset(DateTimeOffset.UtcNow.Date, TimeSpan.Zero);
        response.Content = new StringContent(string.Empty);

        var rr = new ResourceResponse<IEnumerable<string>>(response);
        Assert.NotNull(rr.HasMoreResults);
    }

    [Fact]
    public void HasMoreResults_Returns_False()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        response.Headers.Date = new DateTimeOffset(DateTimeOffset.UtcNow.Date, TimeSpan.Zero);
        response.Content = new StringContent(string.Empty);

        var rr = new ResourceResponse<IEnumerable<string>>(response);
        Assert.False(rr.HasMoreResults);
    }

    [Fact]
    public void HasMoreResults_Returns_True()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        response.Headers.Date = new DateTimeOffset(DateTimeOffset.UtcNow.Date, TimeSpan.Zero);
        response.Headers.TryAddWithoutValidation("x-continuation-token", Guid.NewGuid().ToString());
        response.Content = new StringContent(string.Empty);

        var rr1 = new ResourceResponse<IEnumerable<string>>(response);
        Assert.True(rr1.HasMoreResults);

        var rr2 = new ResourceResponse<List<string>>(response);
        Assert.True(rr2.HasMoreResults);

        var rr3 = new ResourceResponse<Dictionary<string, object>>(response);
        Assert.True(rr3.HasMoreResults);
    }

    private class DummyHttpApiClientOptions : AbstractHttpApiClientOptions { }
}
