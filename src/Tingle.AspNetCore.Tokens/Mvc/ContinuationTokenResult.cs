using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Tingle.AspNetCore.Tokens;
using Tingle.AspNetCore.Tokens.Protection;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// An <see cref="OkObjectResult"/> that supports writing the continuation token to a header before writing the response body
/// </summary>
/// <typeparam name="T">The type of data contained</typeparam>
/// <param name="value">Contains the errors to be returned to the client.</param>
/// <param name="token">the token containing the value</param>
/// <param name="headerName">the name of the header to write the protected token</param>
public class ContinuationTokenResult<T>([ActionResultObjectValue] object value,
                                        ContinuationToken<T> token,
                                        string headerName = TokenDefaults.ContinuationTokenHeaderName) : OkObjectResult(value)
{
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

            // generate a new protected value based on the type of token
            string protected_val;
            if (token is TimedContinuationToken<T> timed)
            {
                protected_val = protector.Protect(timed.GetValue(), timed.GetExpiration());
            }
            else
            {
                protected_val = protector.Protect(token.GetValue());
            }

            // set the header if the protected value is not null
            if (!string.IsNullOrWhiteSpace(protected_val))
                context.HttpContext.Response.Headers[headerName] = protected_val;
        }
    }
}
