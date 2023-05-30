using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Tingle.AspNetCore.Authentication.SharedKey;

/// <summary>
/// Context used with <see cref="SharedKeyEvents.OnMessageReceived"/>
/// </summary>
public class MessageReceivedContext : ResultContext<SharedKeyOptions>
{
    /// <summary>
    /// Creates an instance of <see cref="MessageReceivedContext"/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="scheme"></param>
    /// <param name="options"></param>
    public MessageReceivedContext(HttpContext context, AuthenticationScheme scheme, SharedKeyOptions options)
        : base(context, scheme, options)
    {
    }

    /// <summary>
    /// The provided Token. This will give application an opportunity to retrieve token from an alternation location.
    /// </summary>
    public string? Token { get; set; }
}
