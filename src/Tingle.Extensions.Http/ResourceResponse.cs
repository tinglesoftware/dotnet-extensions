using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using SC = Tingle.Extensions.Http.HttpJsonSerializerContext;

namespace Tingle.Extensions.Http;

/// <summary>Representation of a HTTP response to an API with typed Error and Resource.</summary>
/// <typeparam name="TResource">the type of resource</typeparam>
/// <typeparam name="TProblem">the type of problem</typeparam>
/// <remarks>
/// There is no need to implement <see cref="IDisposable"/> because there are no unmanaged resources in use
/// and there are no resources that the Garbage Collector does not know how to release.
/// The instance of <see cref="HttpResponseMessage"/> referenced by <see cref="Response"/> is automatically disposed
/// once an instance of <see cref="ResourceResponse{TResource, TProblem}"/> is no longer in use.
/// </remarks>
public class ResourceResponse<TResource, TProblem>
{
    private readonly AbstractHttpApiClientOptions? options;

    /// <summary>
    /// Create an instance of <see cref="ResourceResponse{TResource, TError}"/> from another instance.
    /// </summary>
    /// <param name="other">The <see cref="ResourceResponse{TResource, TProblem}"/> to copy from.</param>
    public ResourceResponse(ResourceResponse<TResource, TProblem> other)
    {
        Response = other.Response;
        options = other.options;
        Headers = other.Headers;
        Resource = other.Resource;
        Problem = other.Problem;
    }

    /// <summary>Create an instance of <see cref="ResourceResponse{TResource, TError}"/>.</summary>
    /// <param name="response">the original HTTP response</param>
    /// <param name="options">the client options</param>
    /// <param name="resource">the extracted resource</param>
    /// <param name="problem">the extracted problem description</param>
    public ResourceResponse(HttpResponseMessage response,
                            AbstractHttpApiClientOptions? options = default,
                            TResource? resource = default,
                            TProblem? problem = default)
    {
        Response = response;
        this.options = options;
        Headers = new ResourceResponseHeaders(response);
        Resource = resource;
        Problem = problem;
    }

    /// <summary>The original HTTP response.</summary>
    public HttpResponseMessage Response { get; }

    /// <summary>The response status code gotten from <see cref="Response"/>.</summary>
    public HttpStatusCode StatusCode => Response.StatusCode;

    /// <summary>The response status code gotten from <see cref="Response"/>.</summary>
    public int ResponseCode => (int)Response.StatusCode;

    /// <summary>The list of response headers as extracted from <see cref="Response"/>.</summary>
    public ResourceResponseHeaders Headers { get; }

    /// <summary>
    /// Determines if the request was successful.
    /// Value is true if <see cref="StatusCode"/> is in the 200 to 299 range
    /// </summary>
    public virtual bool IsSuccessful => ((int)StatusCode >= 200) && ((int)StatusCode <= 299);

    /// <summary>The resource extracted from the response body.</summary>
    public TResource? Resource { get; }

    /// <summary>The problem extracted from the response body.</summary>
    public TProblem? Problem { get; }

    /// <summary>Helper method to ensure the response was successful.</summary>
    public virtual void EnsureSuccess()
    {
        // do not bother with successful requests
        if (IsSuccessful) return;

        throw CreateException($"The HTTP request failed with code {ResponseCode} ({StatusCode})",
                              appendHeaders: true,
                              appendRawBody: true);
    }

    /// <summary>Helper method to ensure <see cref="Resource"/> is not null.</summary>
    [MemberNotNull(nameof(Resource))]
    public virtual void EnsureHasResource()
    {
        if (Resource is not null) return;

        throw CreateException("The HTTP response body was either null or empty.",
                              appendHeaders: true,
                              appendRawBody: false);
    }

    /// <summary>
    /// Creates an instance of <see cref="HttpApiResponseException"/>
    /// </summary>
    protected HttpApiResponseException CreateException(string messagePrefix, bool appendHeaders, bool appendRawBody)
    {
        var message = messagePrefix;
        if (appendHeaders) message = AppendHeaders(message);
        if (appendRawBody) message = AppendRawBody(message);

        return new HttpApiResponseException(message: message,
                                            response: Response,
                                            headers: Headers,
                                            resource: Resource,
                                            problem: Problem);
    }

    /// <summary>The token to use to fetch more data.</summary>
    public virtual string? ContinuationToken => Headers.ContinuationToken;

    /// <summary>
    /// Checks if there are more results to retrieve.
    /// The result is null when <typeparamref name="TResource"/> is not assignable from <see cref="IEnumerable"/>.
    /// Otherwise, true when <see cref="ContinuationToken"/> has a value or false when it doesn't have a value.
    /// </summary>
    public virtual bool? HasMoreResults => typeof(IEnumerable).IsAssignableFrom(typeof(TResource)) ? ContinuationToken != null : null;

    /// <summary>Append the response headers to an error message.</summary>
    /// <param name="message">The message to append to.</param>
    /// <returns>The appended message.</returns>
    protected string AppendHeaders(string message)
    {
        string serialize() => System.Text.Json.JsonSerializer.Serialize(Headers, SC.Default.ResourceResponseHeaders);
        return AppendIf(message, o => o.IncludeHeadersInExceptionMessage, serialize, "Headers:\n{0}");
    }

    /// <summary>Append the raw body if present to an error message.</summary>
    /// <param name="message">The message to append to.</param>
    /// <returns>The appended message.</returns>
    protected string AppendRawBody(string message)
    {
        string serialize() => Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        return AppendIf(message, o => o.IncludeRawBodyInExceptionMessage, serialize, "Body:\n{0}");
    }

    private string AppendIf(string message, Func<AbstractHttpApiClientOptions, bool> evaluator, Func<string> serialize, string format)
    {
        if (evaluator is null) throw new ArgumentNullException(nameof(evaluator));

        return options is not null && evaluator(options) ? message + $"\n\n{string.Format(format, serialize())}" : message;
    }
}

/// <summary>Model of a HTTP response to an API with typed Resource.</summary>
/// <typeparam name="TResource">the type of resource</typeparam>
public class ResourceResponse<TResource> : ResourceResponse<TResource, HttpApiResponseProblem>
{
    /// <summary>
    /// Create an instance of <see cref="ResourceResponse{TResource}"/> from another instance.
    /// </summary>
    /// <param name="other">The <see cref="ResourceResponse{TResource}"/> to copy from.</param>
    public ResourceResponse(ResourceResponse<TResource> other) : base(other) { }

    /// <summary>Create an instance of <see cref="ResourceResponse{TResource}"/>.</summary>
    /// <param name="response">the original HTTP response</param>
    /// <param name="options">the client options</param>
    /// <param name="resource">the extracted resource</param>
    /// <param name="problem">the extracted problem description</param>
    public ResourceResponse(HttpResponseMessage response,
                            AbstractHttpApiClientOptions? options = default,
                            TResource? resource = default,
                            HttpApiResponseProblem? problem = null)
        : base(response: response, options: options, resource: resource, problem: problem)
    {
    }
}
