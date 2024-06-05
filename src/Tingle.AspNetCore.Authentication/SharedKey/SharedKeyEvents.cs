namespace Tingle.AspNetCore.Authentication.SharedKey;

/// <summary>
/// Specifies events which the <see cref="SharedKeyHandler"/> invokes to enable developer control over the authentication process.
/// </summary>
public class SharedKeyEvents
{
    /// <summary>
    /// Invoked if exceptions are thrown during request processing. The exceptions will be re-thrown after this event unless suppressed.
    /// </summary>
    public Func<AuthenticationFailedContext, Task> OnAuthenticationFailed { get; set; } = context => Task.CompletedTask;

    /// <summary>
    /// Invoked if Authorization fails and results in a Forbidden response
    /// </summary>
    public Func<ForbiddenContext, Task> OnForbidden { get; set; } = context => Task.CompletedTask;

    /// <summary>
    /// Invoked when a protocol message is first received.
    /// </summary>
    public Func<MessageReceivedContext, Task> OnMessageReceived { get; set; } = context => Task.CompletedTask;

    /// <summary>
    /// Invoked after the security token has passed validation and a ClaimsIdentity has been generated.
    /// </summary>
    public Func<TokenValidatedContext, Task> OnTokenValidated { get; set; } = context => Task.CompletedTask;

    /// <summary>
    /// Invoked before a challenge is sent back to the caller.
    /// </summary>
    public Func<SharedKeyChallengeContext, Task> OnChallenge { get; set; } = context => Task.CompletedTask;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public virtual Task AuthenticationFailed(AuthenticationFailedContext context) => OnAuthenticationFailed(context);

    public virtual Task Forbidden(ForbiddenContext context) => OnForbidden(context);

    public virtual Task MessageReceived(MessageReceivedContext context) => OnMessageReceived(context);

    public virtual Task TokenValidated(TokenValidatedContext context) => OnTokenValidated(context);

    public virtual Task Challenge(SharedKeyChallengeContext context) => OnChallenge(context);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
