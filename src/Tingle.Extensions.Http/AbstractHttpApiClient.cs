using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;
using SC = Tingle.Extensions.Http.HttpJsonSerializerContext;

namespace Tingle.Extensions.Http;

/// <summary>An abstraction of a HTTP client for accessing HTTP REST APIs.</summary>
public abstract class AbstractHttpApiClient<TOptions> where TOptions : AbstractHttpApiClientOptions, new()
{
    /// <summary>Creates an instance of <see cref="AbstractHttpApiClient{TOptions}"/>.</summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> for making requests.</param>
    /// <param name="optionsAccessor">The accessor for the configuration options.</param>
    protected AbstractHttpApiClient(HttpClient httpClient, IOptionsSnapshot<TOptions> optionsAccessor)
    {
        BackChannel = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
    }

    /// <summary>The client for making HTTP requests.</summary>
    protected HttpClient BackChannel { get; }

    /// <summary>The options configured for this client.</summary>
    protected TOptions Options { get; private set; }

    /// <summary>Send a request and download the response to a stream.</summary>
    /// <param name="request">The request to make.</param>
    /// <param name="destination">The stream to copy the response to.</param>
    /// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero. The default size is 81920.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns></returns>
    protected virtual Task<ResourceResponse<object, HttpApiResponseProblem>> DownloadToStreamAsync(HttpRequestMessage request,
                                                                                                   Stream destination,
                                                                                                   int bufferSize = 81920,
                                                                                                   CancellationToken cancellationToken = default)
        => DownloadToStreamAsync(request, destination, SC.Default.HttpApiResponseProblem, bufferSize, cancellationToken);

    /// <summary>Send a request and download the response to a stream.</summary>
    /// <typeparam name="TProblem">The type of problem to be extracted.</typeparam>
    /// <param name="request">The request to make.</param>
    /// <param name="destination">The stream to copy the response to.</param>
    /// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero. The default size is 81920.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns></returns>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    protected virtual async Task<ResourceResponse<object, TProblem>> DownloadToStreamAsync<TProblem>(HttpRequestMessage request,
                                                                                                     Stream destination,
                                                                                                     int bufferSize = 81920,
                                                                                                     CancellationToken cancellationToken = default)
    {
        var response = await BackChannel.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var problem = default(TProblem);

        // if the request succeeded write to the supplied stream
        if (response.IsSuccessStatusCode)
        {
            // get a stream reference for the content and copy its contents to the destination
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            await stream.CopyToAsync(destination, bufferSize: bufferSize, cancellationToken).ConfigureAwait(false);
            stream.Seek(0, SeekOrigin.Begin);
        }
        // if the request did not succeed and we can deserialize the contents, do so
        else
        {
            problem = await DeserializeAsync<TProblem>(response.Content, cancellationToken).ConfigureAwait(false);
        }

        return new ResourceResponse<object, TProblem>(response: response, options: Options, resource: null, problem: problem);
    }

    /// <summary>Send a request and download the response to a stream.</summary>
    /// <typeparam name="TProblem">The type of problem to be extracted.</typeparam>
    /// <param name="request">The request to make.</param>
    /// <param name="destination">The stream to copy the response to.</param>
    /// <param name="problemJsonTypeInfo">Metadata about the <typeparamref name="TProblem"/> to convert.</param>
    /// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero. The default size is 81920.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns></returns>
    protected virtual async Task<ResourceResponse<object, TProblem>> DownloadToStreamAsync<TProblem>(HttpRequestMessage request,
                                                                                                     Stream destination,
                                                                                                     JsonTypeInfo<TProblem> problemJsonTypeInfo,
                                                                                                     int bufferSize = 81920,
                                                                                                     CancellationToken cancellationToken = default)
    {
        var response = await BackChannel.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var problem = default(TProblem);

        // if the request succeeded write to the supplied stream
        if (response.IsSuccessStatusCode)
        {
            // get a stream reference for the content and copy its contents to the destination
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            await stream.CopyToAsync(destination, bufferSize: bufferSize, cancellationToken).ConfigureAwait(false);
            stream.Seek(0, SeekOrigin.Begin);
        }
        // if the request did not succeed and we can deserialize the contents, do so
        else
        {
            problem = await DeserializeAsync(response.Content, problemJsonTypeInfo, cancellationToken).ConfigureAwait(false);
        }

        return new ResourceResponse<object, TProblem>(response: response, options: Options, resource: null, problem: problem);
    }

    /// <summary>Send a request and extract the response.</summary>
    /// <typeparam name="TResource">The type or resource to be extracted.</typeparam>
    /// <param name="request">The request to make.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>A <see cref="ResourceResponse{TResource}"/> instance.</returns>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    protected virtual async Task<ResourceResponse<TResource>> SendAsync<TResource>(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var response = await BackChannel.SendAsync(request, cancellationToken).ConfigureAwait(false);
        (var resource, var problem) = await ExtractResponseAsync<TResource, HttpApiResponseProblem>(response, cancellationToken).ConfigureAwait(false);
        return new ResourceResponse<TResource>(options: Options, response: response, resource: resource, problem: problem);
    }

    /// <summary>Send a request and extract the response.</summary>
    /// <typeparam name="TResource">The type or resource to be extracted.</typeparam>
    /// <param name="request">The request to make.</param>
    /// <param name="jsonTypeInfo">Metadata about the <typeparamref name="TResource"/> to convert.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>A <see cref="ResourceResponse{TResource}"/> instance.</returns>
    protected virtual async Task<ResourceResponse<TResource>> SendAsync<TResource>(HttpRequestMessage request,
                                                                                   JsonTypeInfo<TResource> jsonTypeInfo,
                                                                                   CancellationToken cancellationToken = default)
    {
        var response = await BackChannel.SendAsync(request, cancellationToken).ConfigureAwait(false);
        (var resource, var problem) = await ExtractResponseAsync(response, jsonTypeInfo, SC.Default.HttpApiResponseProblem, cancellationToken).ConfigureAwait(false);
        return new ResourceResponse<TResource>(options: Options, response: response, resource: resource, problem: problem);
    }

    /// <summary>Send a request and extract the response.</summary>
    /// <typeparam name="TResource">The type or resource to be extracted.</typeparam>
    /// <typeparam name="TProblem">The type of problem to be extracted.</typeparam>
    /// <param name="request">The request to make.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>A <see cref="ResourceResponse{TResource, TProblem}"/> instance.</returns>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    protected virtual async Task<ResourceResponse<TResource, TProblem>> SendAsync<TResource, TProblem>(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var response = await BackChannel.SendAsync(request, cancellationToken).ConfigureAwait(false);
        (var resource, var problem) = await ExtractResponseAsync<TResource, TProblem>(response, cancellationToken).ConfigureAwait(false);
        return new ResourceResponse<TResource, TProblem>(response: response, options: Options, resource: resource, problem: problem);
    }

    /// <summary>Send a request and extract the response.</summary>
    /// <typeparam name="TResource">The type or resource to be extracted.</typeparam>
    /// <typeparam name="TProblem">The type of problem to be extracted.</typeparam>
    /// <param name="request">The request to make.</param>
    /// <param name="resourceJsonTypeInfo">Metadata about the <typeparamref name="TResource"/> to convert.</param>
    /// <param name="problemJsonTypeInfo">Metadata about the <typeparamref name="TProblem"/> to convert.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>A <see cref="ResourceResponse{TResource, TProblem}"/> instance.</returns>
    protected virtual async Task<ResourceResponse<TResource, TProblem>> SendAsync<TResource, TProblem>(HttpRequestMessage request,
                                                                                                       JsonTypeInfo<TResource> resourceJsonTypeInfo,
                                                                                                       JsonTypeInfo<TProblem> problemJsonTypeInfo,
                                                                                                       CancellationToken cancellationToken = default)
    {
        var response = await BackChannel.SendAsync(request, cancellationToken).ConfigureAwait(false);
        (var resource, var problem) = await ExtractResponseAsync(response, resourceJsonTypeInfo, problemJsonTypeInfo, cancellationToken).ConfigureAwait(false);
        return new ResourceResponse<TResource, TProblem>(response: response, options: Options, resource: resource, problem: problem);
    }

    /// <summary>
    /// Extracts the resource and problem from the response message. The resource is only extracted for successful requests according
    /// to <see cref="HttpResponseMessage.IsSuccessStatusCode"/>, otherwise the problem is extracted.
    /// </summary>
    /// <typeparam name="TResource">The type or resource to be extracted.</typeparam>
    /// <typeparam name="TProblem">The type of problem to be extracted.</typeparam>
    /// <param name="response">The response message to be used for extraction</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>
    /// A nullable <typeparamref name="TResource"/> and <typeparamref name="TProblem"/> depending on the response status.
    /// <br/>
    /// The <typeparamref name="TResource"/> instance is only populated for successful responses
    /// while the <typeparamref name="TProblem"/> instance is only populated in all other scenarios.
    /// </returns>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    protected virtual async Task<(TResource?, TProblem?)> ExtractResponseAsync<TResource, TProblem>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var resource = default(TResource);
        var problem = default(TProblem);

        var content = response.Content;

        // if the response was a success then deserialize the body as TResource otherwise TProblem
        if (response.IsSuccessStatusCode)
        {
            resource = await DeserializeAsync<TResource>(content, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            problem = await DeserializeAsync<TProblem>(content, cancellationToken).ConfigureAwait(false);
        }

        return (resource, problem);
    }

    /// <summary>
    /// Extracts the resource and problem from the response message. The resource is only extracted for successful requests according
    /// to <see cref="HttpResponseMessage.IsSuccessStatusCode"/>, otherwise the problem is extracted.
    /// </summary>
    /// <typeparam name="TResource">The type or resource to be extracted.</typeparam>
    /// <typeparam name="TProblem">The type of problem to be extracted.</typeparam>
    /// <param name="response">The response message to be used for extraction</param>
    /// <param name="resourceJsonTypeInfo">Metadata about the <typeparamref name="TResource"/> to convert.</param>
    /// <param name="problemJsonTypeInfo">Metadata about the <typeparamref name="TProblem"/> to convert.</param>
    /// <param name="cancellationToken">The token to cancel the request.</param>
    /// <returns>
    /// A nullable <typeparamref name="TResource"/> and <typeparamref name="TProblem"/> depending on the response status.
    /// <br/>
    /// The <typeparamref name="TResource"/> instance is only populated for successful responses
    /// while the <typeparamref name="TProblem"/> instance is only populated in all other scenarios.
    /// </returns>
    protected virtual async Task<(TResource?, TProblem?)> ExtractResponseAsync<TResource, TProblem>(HttpResponseMessage response,
                                                                                                    JsonTypeInfo<TResource> resourceJsonTypeInfo,
                                                                                                    JsonTypeInfo<TProblem> problemJsonTypeInfo,
                                                                                                    CancellationToken cancellationToken)
    {
        var resource = default(TResource);
        var problem = default(TProblem);

        var content = response.Content;

        // if the response was a success then deserialize the body as TResource otherwise TProblem
        if (response.IsSuccessStatusCode)
        {
            resource = await DeserializeAsync(content, resourceJsonTypeInfo, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            problem = await DeserializeAsync(content, problemJsonTypeInfo, cancellationToken).ConfigureAwait(false);
        }

        return (resource, problem);
    }

    /// <summary>Create <see cref="HttpContent"/> with JSON content from the provided <paramref name="value"/>.</summary>
    /// <typeparam name="TValue">The type of the value to serialize.</typeparam>
    /// <param name="value">The object to to write</param>
    /// <param name="mediaType">The media type to use for the content.</param>
    /// <returns>A <see cref="JsonContent"/> instance.</returns>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    protected virtual HttpContent MakeJsonContent<TValue>(TValue value, MediaTypeHeaderValue? mediaType = null) => JsonContent.Create(value, mediaType, options: Options.SerializerOptions);

    /// <summary>Create <see cref="HttpContent"/> with JSON content from the provided <paramref name="value"/>.</summary>
    /// <param name="value">The object to to write</param>
    /// <param name="valueType">The type of the value to serialize.</param>
    /// <param name="mediaType">The media type to use for the content.</param>
    /// <returns>A <see cref="JsonContent"/> instance.</returns>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    protected virtual HttpContent MakeJsonContent(object value, Type valueType, MediaTypeHeaderValue? mediaType = null) => JsonContent.Create(value, valueType, mediaType, options: Options.SerializerOptions);

    /// <summary>Create <see cref="HttpContent"/> with JSON content from the provided <paramref name="value"/>.</summary>
    /// <typeparam name="TValue">The type of the value to serialize.</typeparam>
    /// <param name="value">The object to to write</param>
    /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
    /// <param name="mediaType">The media type to use for the content.</param>
    /// <returns>A <see cref="JsonContent"/> instance.</returns>
    protected virtual HttpContent MakeJsonContent<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo, MediaTypeHeaderValue? mediaType = null) => JsonContent.Create(value, jsonTypeInfo, mediaType);

    /// <summary>Reads the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.</summary>
    /// <typeparam name="TValue">The type to deserialize the JSON value into.</typeparam>
    /// <param name="content">The <see cref="HttpContent"/> from the response.</param>
    /// <param name="cancellationToken">The token to cancel the read operation.</param>
    /// <returns>A <typeparamref name="TValue"/> representation of the JSON value.</returns>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    protected virtual Task<TValue?> DeserializeAsync<TValue>(HttpContent content, CancellationToken cancellationToken)
    {
        return content.Headers.ContentLength == 0
            ? Task.FromResult<TValue?>(default)
            : content.ReadFromJsonAsync<TValue>(Options.SerializerOptions, cancellationToken);
    }

    /// <summary>Reads the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.</summary>
    /// <typeparam name="TValue">The type to deserialize the JSON value into.</typeparam>
    /// <param name="content">The <see cref="HttpContent"/> from the response.</param>
    /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
    /// <param name="cancellationToken">The token to cancel the read operation.</param>
    /// <returns>A <typeparamref name="TValue"/> representation of the JSON value.</returns>
    protected virtual Task<TValue?> DeserializeAsync<TValue>(HttpContent content, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken)
    {
        return content.Headers.ContentLength == 0
            ? Task.FromResult<TValue?>(default)
            : content.ReadFromJsonAsync(jsonTypeInfo, cancellationToken);
    }
}
