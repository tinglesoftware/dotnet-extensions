using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Tingle.AspNetCore.Authentication.SharedKey.Validation;

/// <summary>
/// Validator for tokens generated with a shared key
/// </summary>
public interface ISharedKeyTokenValidator
{
    /// <summary>
    /// Returns true if the token can be read, false otherwise.
    /// </summary>
    bool CanReadToken(string token);

    /// <summary>
    /// Validates a token passed as a string using <see cref="SharedKeyTokenValidationParameters"/>
    /// </summary>
    Task<(ClaimsPrincipal, SharedKeyValidatedToken)> ValidateTokenAsync(string securityToken,
                                                                        HttpContext httpContext,
                                                                        SharedKeyOptions options);
}
