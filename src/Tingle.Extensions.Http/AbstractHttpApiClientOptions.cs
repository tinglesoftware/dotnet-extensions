using System.Text.Json;

namespace Tingle.Extensions.Http;

/// <summary>
/// Abstract service configuration options for <see cref="AbstractHttpApiClient{TOptions}"/>
/// </summary>
public abstract class AbstractHttpApiClientOptions
{
    /// <summary>The options to use for JSON serialization.</summary>
    public virtual JsonSerializerOptions SerializerOptions { get; set; } = new JsonSerializerOptions
    {
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false, // less data used
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
                               | System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,

        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    /// <summary>
    /// Determines if the response headers should be included in the message when creating <see cref="HttpApiResponseException"/>
    /// via <see cref="ResourceResponse{TResource, TProblem}.EnsureSuccess"/>
    /// or <see cref="ResourceResponse{TResource, TProblem}.EnsureHasResource"/>.
    /// Defaults to false.
    /// </summary>
    [Obsolete("Use '" + nameof(IncludeResponseHeadersInExceptionMessage) + "' instead.")]
    public virtual bool IncludeHeadersInExceptionMessage { get => IncludeResponseHeadersInExceptionMessage; set => IncludeResponseHeadersInExceptionMessage = value; }

    /// <summary>
    /// Determines if the response body should be included in the message when creating <see cref="HttpApiResponseException"/>
    /// via <see cref="ResourceResponse{TResource, TProblem}.EnsureSuccess"/>.
    /// Defaults to false.
    /// </summary>
    [Obsolete("Use '" + nameof(IncludeResponseBodyInExceptionMessage) + "' instead.")]
    public virtual bool IncludeRawBodyInExceptionMessage { get => IncludeResponseBodyInExceptionMessage; set => IncludeResponseBodyInExceptionMessage = value; }

    /// <summary>
    /// Determines if the request headers should be included in the message when creating <see cref="HttpApiResponseException"/>
    /// via <see cref="ResourceResponse{TResource, TProblem}.EnsureSuccess"/>
    /// or <see cref="ResourceResponse{TResource, TProblem}.EnsureHasResource"/>.
    /// Defaults to false.
    /// </summary>
    public virtual bool IncludeRequestHeadersInExceptionMessage { get; set; } = false;

    /// <summary>
    /// Determines if the request body should be included in the message when creating <see cref="HttpApiResponseException"/>
    /// via <see cref="ResourceResponse{TResource, TProblem}.EnsureSuccess"/>.
    /// Defaults to false.
    /// </summary>
    public virtual bool IncludeRequestBodyInExceptionMessage { get; set; } = false;

    /// <summary>
    /// Determines if the response headers should be included in the message when creating <see cref="HttpApiResponseException"/>
    /// via <see cref="ResourceResponse{TResource, TProblem}.EnsureSuccess"/>
    /// or <see cref="ResourceResponse{TResource, TProblem}.EnsureHasResource"/>.
    /// Defaults to false.
    /// </summary>
    public virtual bool IncludeResponseHeadersInExceptionMessage { get; set; } = false;

    /// <summary>
    /// Determines if the response body should be included in the message when creating <see cref="HttpApiResponseException"/>
    /// via <see cref="ResourceResponse{TResource, TProblem}.EnsureSuccess"/>.
    /// Defaults to false.
    /// </summary>
    public virtual bool IncludeResponseBodyInExceptionMessage { get; set; } = false;
}
