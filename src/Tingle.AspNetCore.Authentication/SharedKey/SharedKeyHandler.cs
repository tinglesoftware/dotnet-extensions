using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Tingle.AspNetCore.Authentication.SharedKey.Validation;
using Tingle.AspNetCore.Authentication.SharedKey.Validation.Exceptions;

namespace Tingle.AspNetCore.Authentication.SharedKey;

/// <summary>
/// Shared key authentication handler 
/// </summary>
public class SharedKeyHandler : AuthenticationHandler<SharedKeyOptions>
{
#if NET8_0_OR_GREATER
    /// <summary>
    /// Create an instance of <see cref="SharedKeyHandler"/>
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    /// <param name="encoder"></param>
    public SharedKeyHandler(IOptionsMonitor<SharedKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) { }

#else

    /// <summary>
    /// Create an instance of <see cref="SharedKeyHandler"/>
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    /// <param name="encoder"></param>
    /// <param name="clock"></param>
    [Obsolete("ISystemClock is obsolete, use TimeProvider on AuthenticationSchemeOptions instead.")]
    public SharedKeyHandler(IOptionsMonitor<SharedKeyOptions> options,
                            ILoggerFactory logger,
                            UrlEncoder encoder,
                            ISystemClock clock) : base(options, logger, encoder, clock) { }
#endif

    /// <summary>
    /// The handler calls methods on the events which give the application control at certain points where processing is occurring. 
    /// If it is not provided a default instance is supplied which does nothing when the methods are called.
    /// </summary>
    protected new SharedKeyEvents Events
    {
        get { return (SharedKeyEvents)base.Events!; }
        set { base.Events = value; }
    }

    /// <inheritdoc/>
    protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new SharedKeyEvents());

    /// <inheritdoc/>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            // Give application opportunity to find from a different location, adjust, or reject token
            var messageReceivedContext = new MessageReceivedContext(Context, Scheme, Options);

            // event can set the token
            await Events.MessageReceived(messageReceivedContext).ConfigureAwait(false);
            if (messageReceivedContext.Result != null)
            {
                return messageReceivedContext.Result;
            }

            // If application retrieved token from somewhere else, use that.
            var token = messageReceivedContext.Token;

            if (string.IsNullOrEmpty(token))
            {
                string authorization = Request.Headers.Authorization.ToString();

                // If no authorization header found nothing to process further
                if (string.IsNullOrEmpty(authorization))
                {
                    return AuthenticateResult.NoResult();
                }

                // if we have a header and it starts with the prefix then extract the token
                var prefix = $"{Options.HeaderValuePrefix} ";
                if (authorization.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    token = authorization[prefix.Length..].Trim();
                }

                // If no token found, no further work possible
                if (string.IsNullOrEmpty(token))
                {
                    return AuthenticateResult.NoResult();
                }
            }

            var validationParameters = Options.ValidationParameters;

            var validationFailures = new List<Exception>();
            SharedKeyValidatedToken? validatedToken = null;
            foreach (var validator in Options.TokenValidators)
            {
                if (validator.CanReadToken(token))
                {
                    ClaimsPrincipal principal;
                    try
                    {
                        (principal, validatedToken) = await validator.ValidateTokenAsync(token, Context, Options).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Logger.TokenValidationFailed(ex);
                        validationFailures.Add(ex);
                        continue;
                    }

                    Logger.TokenValidationSucceeded();

                    var tokenValidatedContext = new TokenValidatedContext(Context, Scheme, Options)
                    {
                        Principal = principal,
                        ValidationResponse = validatedToken,
                    };

                    await Events.TokenValidated(tokenValidatedContext).ConfigureAwait(false);
                    if (tokenValidatedContext.Result != null)
                    {
                        return tokenValidatedContext.Result;
                    }

                    if (Options.SaveToken)
                    {
                        tokenValidatedContext.Properties.StoreTokens(new[]
                        {
                                new AuthenticationToken { Name = "access_token", Value = token }
                            });
                    }

                    tokenValidatedContext.Success();
                    return tokenValidatedContext.Result!;
                }
            }

            if (validationFailures.Count != 0)
            {
                var authenticationFailedContext = new AuthenticationFailedContext(Context, Scheme, Options)
                {
                    Exception = (validationFailures.Count == 1) ? validationFailures[0] : new AggregateException(validationFailures)
                };

                await Events.AuthenticationFailed(authenticationFailedContext).ConfigureAwait(false);
                if (authenticationFailedContext.Result != null)
                {
                    return authenticationFailedContext.Result;
                }

                return AuthenticateResult.Fail(authenticationFailedContext.Exception);
            }

            return AuthenticateResult.Fail("No SharedKeyTokenValidator available for token: " + token ?? "[null]");
        }
        catch (Exception ex)
        {
            Logger.ErrorProcessingMessage(ex);

            var authenticationFailedContext = new AuthenticationFailedContext(Context, Scheme, Options)
            {
                Exception = ex
            };

            await Events.AuthenticationFailed(authenticationFailedContext).ConfigureAwait(false);
            if (authenticationFailedContext.Result != null)
            {
                return authenticationFailedContext.Result;
            }

            throw;
        }
    }

    /// <inheritdoc/>
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var authResult = await HandleAuthenticateOnceSafeAsync().ConfigureAwait(false);
        var eventContext = new SharedKeyChallengeContext(Context, Scheme, Options, properties)
        {
            AuthenticateFailure = authResult?.Failure
        };

        // Avoid returning error=invalid_token if the error is not caused by an authentication failure (e.g missing token).
        if (Options.IncludeErrorDetails && eventContext.AuthenticateFailure != null)
        {
            eventContext.Error = "invalid_token";
            eventContext.ErrorDescription = CreateErrorDescription(eventContext.AuthenticateFailure);
        }

        await Events.Challenge(eventContext).ConfigureAwait(false);
        if (eventContext.Handled)
        {
            return;
        }

        Response.StatusCode = 401;

        if (string.IsNullOrEmpty(eventContext.Error) &&
            string.IsNullOrEmpty(eventContext.ErrorDescription) &&
            string.IsNullOrEmpty(eventContext.ErrorUri))
        {
            Response.Headers.Append(HeaderNames.WWWAuthenticate, Options.Challenge);
        }
        else
        {
            // https://tools.ietf.org/html/rfc6750#section-3.1
            // WWW-Authenticate: Bearer realm="example", error="invalid_token", error_description="The access token expired"
            var builder = new StringBuilder(Options.Challenge);
            if (Options.Challenge.IndexOf(' ') > 0)
            {
                // Only add a comma after the first param, if any
                builder.Append(',');
            }
            if (!string.IsNullOrEmpty(eventContext.Error))
            {
                builder.Append(" error=\"");
                builder.Append(eventContext.Error);
                builder.Append('"');
            }
            if (!string.IsNullOrEmpty(eventContext.ErrorDescription))
            {
                if (!string.IsNullOrEmpty(eventContext.Error))
                {
                    builder.Append(',');
                }

                builder.Append(" error_description=\"");
                builder.Append(eventContext.ErrorDescription);
                builder.Append('\"');
            }
            if (!string.IsNullOrEmpty(eventContext.ErrorUri))
            {
                if (!string.IsNullOrEmpty(eventContext.Error) ||
                    !string.IsNullOrEmpty(eventContext.ErrorDescription))
                {
                    builder.Append(',');
                }

                builder.Append(" error_uri=\"");
                builder.Append(eventContext.ErrorUri);
                builder.Append('\"');
            }

            Response.Headers.Append(HeaderNames.WWWAuthenticate, builder.ToString());
        }
    }

    /// <inheritdoc/>
    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        var forbiddenContext = new ForbiddenContext(Context, Scheme, Options);
        Response.StatusCode = 403;
        return Events.Forbidden(forbiddenContext);
    }

    private static string CreateErrorDescription(Exception authFailure)
    {
        IEnumerable<Exception> exceptions;
        if (authFailure is AggregateException agEx)
        {
            exceptions = agEx.InnerExceptions;
        }
        else
        {
            exceptions = [authFailure];
        }

        var messages = new List<string>();

        foreach (var ex in exceptions)
        {
            // Order sensitive, some of these exceptions derive from others
            // and we want to display the most specific message possible.
            switch (ex)
            {
                case SharedKeyInvalidDateException skid:
                    messages.Add($"The supplied date is invalid; Supplied: '{skid.Value ?? "(null)"}'");
                    break;
                case SharedKeyTimeWindowExpiredException sktwe:
                    messages.Add($"The supplied date is invalid; NotOlderThan: '{sktwe.OldestAllowed.ToString(CultureInfo.InvariantCulture)}'"
                        + $", Supplied: '{sktwe.SuppliedTime.ToString(CultureInfo.InvariantCulture)}'");
                    break;
                case SharedKeyInvalidSignatureException _:
                    messages.Add("The signature is invalid");
                    break;
                case SharedKeyNoDateException sknd:
                    messages.Add($"Date header must be supplied in any of these headers ({sknd.HeaderNamesJoined})");
                    break;
                case SharedKeyNoKeysException _:
                    messages.Add("Unable to resolve signing keys");
                    break;
                case SharedKeyInvalidSigningKeysException _:
                    messages.Add("Invalid signing keys");
                    break;
            }
        }

        return string.Join("; ", messages);
    }
}
