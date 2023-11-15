using System.Net;

namespace Tingle.Extensions.Http;

/// <summary>
/// An exception thrown when an API request does not succeed.
/// </summary>
[Serializable]
public class HttpApiResponseException : Exception
{
    /// <summary>Creates an instance of <see cref="HttpApiResponseException"/>.</summary>
    public HttpApiResponseException() { }

    /// <summary>Creates an instance of <see cref="HttpApiResponseException"/>.</summary>
    public HttpApiResponseException(string message) : base(message) { }

    /// <summary>Creates an instance of <see cref="HttpApiResponseException"/>.</summary>
    public HttpApiResponseException(string message, Exception inner) : base(message, inner) { }

    /// <summary>Creates an instance of <see cref="HttpApiResponseException"/>.</summary>
    public HttpApiResponseException(string message,
                                    HttpResponseMessage response,
                                    IReadOnlyDictionary<string, IEnumerable<string>> headers,
                                    object? resource,
                                    object? problem) : base(message)
    {
        Response = response;
        StatusCode = response.StatusCode;
        ResponseCode = (int)response.StatusCode;
        Headers = headers;
        Resource = resource;
        Problem = problem;
    }

    /// <summary>The actual HTTP response.</summary>
    public HttpResponseMessage? Response { get; }

    /// <summary>The response status code gotten from the <see cref="Response"/>.</summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>The response status code gotten from the <see cref="Response"/>.</summary>
    public int ResponseCode { get; }

    /// <summary>
    /// The list of response headers extracted from <see cref="Response"/>
    /// and its content (<see cref="HttpResponseMessage.Content"/>).
    /// </summary>
    public IReadOnlyDictionary<string, IEnumerable<string>>? Headers { get; }

    /// <summary>The resource extracted from the response body, if any.</summary>
    public object? Resource { get; }

    /// <summary>The problem extracted from the response body, if any.</summary>
    public object? Problem { get; }
}
