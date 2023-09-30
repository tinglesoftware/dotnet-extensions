using System.Net;
using System.Runtime.Serialization;

namespace Tingle.Extensions.Http;

/// <summary>
/// An exception thrown when an API request does not succeed.
/// </summary>
[Serializable]
public class HttpApiResponseException : Exception
{
    /// <summary>
    /// Creates an instance of <see cref="HttpApiResponseException"/>
    /// </summary>
    public HttpApiResponseException() { }

    /// <summary>
    /// Creates an instance of <see cref="HttpApiResponseException"/>
    /// </summary>
    public HttpApiResponseException(string message) : base(message) { }

    /// <summary>
    /// Creates an instance of <see cref="HttpApiResponseException"/>
    /// </summary>
    public HttpApiResponseException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// Creates an instance of <see cref="HttpApiResponseException"/>
    /// </summary>
    protected HttpApiResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    /// <summary>
    /// Creates an instance of <see cref="HttpApiResponseException"/>
    /// </summary>
    public HttpApiResponseException(string message,
                                    HttpResponseMessage response,
                                    object? resource,
                                    object? problem,
                                    IReadOnlyDictionary<string, IEnumerable<string>> headers) : base(message)
    {
        Response = response;
        StatusCode = response.StatusCode;
        ResponseCode = (int)response.StatusCode;
        Resource = resource;
        Problem = problem;
        Headers = headers;
    }

    /// <summary>The actual HTTP response.</summary>
    public HttpResponseMessage? Response { get; }

    /// <summary>
    /// The response status code gotten from the Response
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// The response status code gotten from the Response
    /// </summary>
    public int ResponseCode { get; }

    /// <summary>
    /// The resource extracted from the response body, if any.
    /// </summary>
    public object? Resource { get; }

    /// <summary>
    /// The problem extracted from the response body, if any.
    /// </summary>
    public object? Problem { get; }

    /// <summary>
    /// The list of response headers as extracted from the Response
    /// </summary>
    public IReadOnlyDictionary<string, IEnumerable<string>>? Headers { get; }
}
