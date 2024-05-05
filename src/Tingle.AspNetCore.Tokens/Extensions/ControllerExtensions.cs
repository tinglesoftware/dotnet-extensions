using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Tingle.AspNetCore.Tokens;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// Extension methods for <see cref="ControllerBase"/>
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Creates an <see cref="OkObjectResult"/> that supports writing an instance of <see cref="ContinuationToken{T}"/>
    /// to a header before writing the response body
    /// </summary>
    /// <param name="controller">the controller to extend</param>
    /// <param name="value">Contains the errors to be returned to the client.</param>
    /// <param name="token">the token containing the value</param>
    /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
    /// <param name="headerName">the name of the header to write the protected token</param>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    public static ContinuationTokenResult<T> Ok<T>(this ControllerBase controller,
                                                   [ActionResultObjectValue] object value,
                                                   ContinuationToken<T> token,
                                                   JsonSerializerOptions? serializerOptions = null,
                                                   string headerName = TokenDefaults.ContinuationTokenHeaderName)
    {
        return new ContinuationTokenResult<T>(value, token, serializerOptions, headerName);
    }

    /// <summary>
    /// Creates an <see cref="OkObjectResult"/> that supports writing a token's value wrapped in an instance of
    /// <see cref="ContinuationToken{T}"/> to a header before writing the response body
    /// </summary>
    /// <param name="controller">the controller to extend</param>
    /// <param name="value">Contains the errors to be returned to the client.</param>
    /// <param name="tokenValue">the token's value</param>
    /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
    /// <param name="headerName">the name of the header to write the protected token</param>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    public static ContinuationTokenResult<T> Ok<T>(this ControllerBase controller,
                                                   [ActionResultObjectValue] object value,
                                                   T tokenValue,
                                                   JsonSerializerOptions? serializerOptions = null,
                                                   string headerName = TokenDefaults.ContinuationTokenHeaderName)
    {
        return Ok(controller: controller,
                  value: value,
                  token: new ContinuationToken<T>(tokenValue),
                  serializerOptions: serializerOptions,
                  headerName: headerName);
    }


    /// <summary>
    /// Creates an <see cref="OkObjectResult"/> that supports writing a token's value wrapped in an instance of
    /// <see cref="ContinuationTokenResult{T}"/> to a header before writing the response body
    /// </summary>
    /// <param name="controller">the controller to extend</param>
    /// <param name="value">Contains the errors to be returned to the client.</param>
    /// <param name="tokenValue">the token's value</param>
    /// <param name="expiration"></param>
    /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
    /// <param name="headerName">the name of the header to write the protected token</param>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    public static ContinuationTokenResult<T> Ok<T>(this ControllerBase controller,
                                                   [ActionResultObjectValue] object value,
                                                   T tokenValue,
                                                   DateTimeOffset expiration,
                                                   JsonSerializerOptions? serializerOptions = null,
                                                   string headerName = TokenDefaults.ContinuationTokenHeaderName)
    {
        return Ok(controller: controller,
                  value: value,
                  token: new TimedContinuationToken<T>(tokenValue, expiration),
                  serializerOptions: serializerOptions,
                  headerName: headerName);
    }

    /// <summary>
    /// Creates an <see cref="OkObjectResult"/> that supports writing an instance of <see cref="ContinuationToken{T}"/>
    /// to a header before writing the response body
    /// </summary>
    /// <param name="controller">the controller to extend</param>
    /// <param name="value">Contains the errors to be returned to the client.</param>
    /// <param name="token">the token containing the value</param>
    /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
    /// <param name="headerName">the name of the header to write the protected token</param>
    public static ContinuationTokenResult<T> Ok<T>(this ControllerBase controller,
                                                   [ActionResultObjectValue] object value,
                                                   ContinuationToken<T> token,
                                                   JsonTypeInfo<T> jsonTypeInfo,
                                                   string headerName = TokenDefaults.ContinuationTokenHeaderName)
    {
        return new ContinuationTokenResult<T>(value, token, jsonTypeInfo, headerName);
    }

    /// <summary>
    /// Creates an <see cref="OkObjectResult"/> that supports writing a token's value wrapped in an instance of
    /// <see cref="ContinuationToken{T}"/> to a header before writing the response body
    /// </summary>
    /// <param name="controller">the controller to extend</param>
    /// <param name="value">Contains the errors to be returned to the client.</param>
    /// <param name="tokenValue">the token's value</param>
    /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
    /// <param name="headerName">the name of the header to write the protected token</param>
    public static ContinuationTokenResult<T> Ok<T>(this ControllerBase controller,
                                                   [ActionResultObjectValue] object value,
                                                   T tokenValue,
                                                   JsonTypeInfo<T> jsonTypeInfo,
                                                   string headerName = TokenDefaults.ContinuationTokenHeaderName)
    {
        return Ok(controller: controller,
                  value: value,
                  token: new ContinuationToken<T>(tokenValue),
                  jsonTypeInfo: jsonTypeInfo,
                  headerName: headerName);
    }


    /// <summary>
    /// Creates an <see cref="OkObjectResult"/> that supports writing a token's value wrapped in an instance of
    /// <see cref="ContinuationTokenResult{T}"/> to a header before writing the response body
    /// </summary>
    /// <param name="controller">the controller to extend</param>
    /// <param name="value">Contains the errors to be returned to the client.</param>
    /// <param name="tokenValue">the token's value</param>
    /// <param name="expiration"></param>
    /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
    /// <param name="headerName">the name of the header to write the protected token</param>
    public static ContinuationTokenResult<T> Ok<T>(this ControllerBase controller,
                                                   [ActionResultObjectValue] object value,
                                                   T tokenValue,
                                                   DateTimeOffset expiration,
                                                   JsonTypeInfo<T> jsonTypeInfo,
                                                   string headerName = TokenDefaults.ContinuationTokenHeaderName)
    {
        return Ok(controller: controller,
                  value: value,
                  token: new TimedContinuationToken<T>(tokenValue, expiration),
                  jsonTypeInfo: jsonTypeInfo,
                  headerName: headerName);
    }
}
