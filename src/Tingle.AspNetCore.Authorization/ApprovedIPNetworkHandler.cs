using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Tingle.AspNetCore.Authorization;

/// <summary>
/// An <see cref="IAuthorizationHandler"/> that need is called to validate an instance of <see cref="ApprovedIPNetworkRequirement"/>
/// </summary>
/// <param name="httpContextAccessor">the accessor that provides the current HTTP context</param>
/// <param name="logger"></param>
public class ApprovedIPNetworkHandler(IHttpContextAccessor httpContextAccessor, ILogger<ApprovedIPNetworkHandler> logger) : AuthorizationHandler<ApprovedIPNetworkRequirement>
{
    /// <summary>
    /// Makes a decision if authorization is allowed based on a specific requirement.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The requirement to evaluate.</param>
    /// <returns></returns>
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApprovedIPNetworkRequirement requirement)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var address = httpContext?.Connection.RemoteIpAddress;
        logger.LogTrace("Checking approval for IP: '{IPAddress}'", address);
        if (address != null && requirement.IsApproved(address))
        {
            context.Succeed(requirement);
        }
        else
        {
            logger.LogWarning("Approval for IP: '{IPAddress}' failed", address);
        }

        return Task.CompletedTask;
    }
}
