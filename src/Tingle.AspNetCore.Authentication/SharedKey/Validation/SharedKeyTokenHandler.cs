using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text;
using Tingle.AspNetCore.Authentication.SharedKey.Validation.Exceptions;

namespace Tingle.AspNetCore.Authentication.SharedKey.Validation;

/// 
public class SharedKeyTokenHandler : ISharedKeyTokenValidator
{
    /// <inheritdoc/>
    public virtual bool CanReadToken(string token) => IsValidBase64(token);

    /// <inheritdoc/>
    public virtual async Task<(ClaimsPrincipal, SharedKeyValidatedToken)> ValidateTokenAsync(string securityToken,
                                                                                             HttpContext httpContext,
                                                                                             SharedKeyOptions options)
    {
        var validationParameters = options.ValidationParameters;
        var httpRequest = httpContext.Request;
        (var timeHeaderName, var timeHeaderValue) = FindDateHeader(httpRequest, validationParameters);

        // ensure we have a value for time
        if (string.IsNullOrWhiteSpace(timeHeaderValue))
        {
            throw SharedKeyNoDateException.Create(validationParameters.DateHeaderNames);
        }

        // check time if allowed
        if (validationParameters.TimeAllowance.HasValue)
        {
            // ensure we can parse
            if (!DateTimeOffset.TryParse(timeHeaderValue, out DateTimeOffset time))
            {
                throw SharedKeyInvalidDateException.Create(timeHeaderValue);
            }

            // calculate the oldest date allowed
            var oldestAllowed = DateTimeOffset.UtcNow.Subtract(validationParameters.TimeAllowance.Value);

            // ensure that the oldest allowed time is not after the provided time
            if (oldestAllowed > time)
            {
                throw SharedKeyTimeWindowExpiredException.Create(time, oldestAllowed);
            }
        }

        // get possible signing keys, throw if there are none
        var keys = await validationParameters.ResolveKeysAsync(httpContext).ConfigureAwait(false);
        if (keys == null || !keys.Any())
        {
            throw new SharedKeyNoKeysException("Keys could not be resolved");
        }

        // ensure the keys resolved are valid
        var invalidKeys = keys.Where(k => !IsValidBase64(k)).ToList();
        if (invalidKeys.Count >= 1)
        {
            throw SharedKeyInvalidSigningKeysException.Create(invalidKeys);
        }

        foreach (var key in keys)
        {
            var bytes = Convert.FromBase64String(key);
            var expectedToken = MakeSignature(httpRequest, validationParameters, bytes, timeHeaderName, timeHeaderValue);
            if (string.Equals(expectedToken, securityToken))
            {
                // at this point, the authentication worked
                var validatedToken = new SharedKeyValidatedToken(securityToken, key);

                var claims = new[]
                {
                        new Claim(ClaimTypes.NameIdentifier, validatedToken.MatchingKey),
                        new Claim(ClaimTypes.Hash, validatedToken.Token),
                    };

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, options.ClaimsIssuer ?? "SharedKey"));
                return (principal, validatedToken);
            }
        }

        // if we get here none of the keys worked so authentication failed
        throw new SharedKeyInvalidSignatureException("The token provided token does not match any of the resolved keys");
    }

    private static string MakeSignature(HttpRequest httpRequest,
                                        SharedKeyTokenValidationParameters validationParameters,
                                        byte[] sharedKeyBytes,
                                        string dateHeaderName,
                                        string dateHeaderValue)
    {
        // collect items
        var method = httpRequest.Method;
        var contentLength = (int)(httpRequest.ContentLength ?? 0);
        var contentType = httpRequest.ContentType;
        dateHeaderValue ??= string.Empty;

        // append path where necessary
        var resource = httpRequest.Path;
        if (!string.IsNullOrWhiteSpace(validationParameters.PathPrefix))
        {
            resource = validationParameters.PathPrefix + resource;
        }

        var stringToHash = string.Join("\n", method, contentLength, contentType, $"{dateHeaderName}:{dateHeaderValue}", resource);
        var bytesToHash = Encoding.ASCII.GetBytes(stringToHash);
        using var sha256 = new System.Security.Cryptography.HMACSHA256(sharedKeyBytes);
        var calculatedHash = sha256.ComputeHash(bytesToHash);
        return Convert.ToBase64String(calculatedHash);
    }

    private static bool IsValidBase64(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return false;

        try
        {
            _ = Convert.FromBase64String(s);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static (string key, string value) FindDateHeader(HttpRequest httpRequest, SharedKeyTokenValidationParameters validationParameters)
    {
        foreach (var possibleHeaderName in validationParameters.DateHeaderNames)
        {
            var v = httpRequest.Headers[possibleHeaderName];
            if (!string.IsNullOrEmpty(v))
            {
                return (key: possibleHeaderName, value: v!);
            }
        }

        return default;
    }
}
