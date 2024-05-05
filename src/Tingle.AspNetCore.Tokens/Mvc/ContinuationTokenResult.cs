using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Tingle.AspNetCore.Tokens;
using Tingle.AspNetCore.Tokens.Protection;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// An <see cref="OkObjectResult"/> that supports writing the continuation token to a header before writing the response body
/// </summary>
/// <typeparam name="T">The type of data contained</typeparam>
public class ContinuationTokenResult<T> : OkObjectResult
{
    private readonly ContinuationToken<T> token;
    private readonly string headerName;
    private readonly JsonSerializerOptions? serializerOptions;
    private readonly JsonTypeInfo<T>? jsonTypeInfo;

    /// <param name="value">Contains the errors to be returned to the client.</param>
    /// <param name="token">the token containing the value</param>
    /// <param name="serializerOptions">Options to control the behavior during parsing.</param>
    /// <param name="headerName">the name of the header to write the protected token</param>
    [RequiresUnreferencedCode(MessageStrings.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(MessageStrings.SerializationRequiresDynamicCodeMessage)]
    public ContinuationTokenResult([ActionResultObjectValue] object value,
                                   ContinuationToken<T> token,
                                   JsonSerializerOptions? serializerOptions = null,
                                   string headerName = TokenDefaults.ContinuationTokenHeaderName) : base(value)
    {
        this.token = token ?? throw new ArgumentNullException(nameof(token));
        this.serializerOptions = serializerOptions;
        this.headerName = headerName ?? throw new ArgumentNullException(nameof(headerName));
    }

    /// <param name="value">Contains the errors to be returned to the client.</param>
    /// <param name="token">the token containing the value</param>
    /// <param name="jsonTypeInfo">Metadata about the type to convert.</param>
    /// <param name="headerName">the name of the header to write the protected token</param>
    public ContinuationTokenResult([ActionResultObjectValue] object value,
                                   ContinuationToken<T> token,
                                   JsonTypeInfo<T> jsonTypeInfo,
                                   string headerName = TokenDefaults.ContinuationTokenHeaderName) : base(value)
    {
        this.token = token ?? throw new ArgumentNullException(nameof(token));
        this.jsonTypeInfo = jsonTypeInfo ?? throw new ArgumentNullException(nameof(jsonTypeInfo));
        this.headerName = headerName ?? throw new ArgumentNullException(nameof(headerName));
    }

    /// <inheritdoc/>
    public override void OnFormatting(ActionContext context)
    {
        base.OnFormatting(context); // required so that it can write the statusCode

        // we can only set the header if 
        // 1) the provided token instance is not null
        // 2) the underlying value is not null
        // 3) the protected value is not null or empty
        if (token is not null && token.GetValue() != null)
        {
            // get an instance of the protector
            var protector = context.HttpContext.RequestServices.GetRequiredService<ITokenProtector<T>>();

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            // generate a new protected value based on the type of token
            string protected_val;
            var value = token.GetValue();
            if (token is TimedContinuationToken<T> timed)
            {
                var expiration = timed.GetExpiration();
                protected_val = jsonTypeInfo is null
                              ? protector.Protect(value, expiration, serializerOptions)
                              : protector.Protect(value, expiration, jsonTypeInfo);
            }
            else
            {
                protected_val = jsonTypeInfo is null
                              ? protector.Protect(value)
                              : protector.Protect(value);
            }

            // set the header if the protected value is not null
            if (!string.IsNullOrWhiteSpace(protected_val))
                context.HttpContext.Response.Headers[headerName] = protected_val;
        }
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    }
}
