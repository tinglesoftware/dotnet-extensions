using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Tingle.AspNetCore.Authorization;

/// <summary>
/// An <see cref="IAuthorizationRequirement"/> that contains IP networks
/// </summary>
/// <param name="networks">the networks allowed</param>
#if NET8_0_OR_GREATER
public sealed class ApprovedIPNetworkRequirement(IList<IPNetwork> networks) : IAuthorizationRequirement
#else
public sealed class ApprovedIPNetworkRequirement(IList<IPNetwork2> networks) : IAuthorizationRequirement
#endif
{
    /// <summary>
    /// Checks is an instance of <see cref="IPAddress"/> is approved
    /// </summary>
    /// <param name="address">The address to check.</param>
    /// <returns></returns>
    public bool IsApproved(IPAddress address)
    {
        // if the IP is an IPv4 mapped to IPv6, remap it
        var addr = address;
        if (addr.IsIPv4MappedToIPv6)
        {
            addr = addr.MapToIPv4();
        }

        return networks.Any(n => n.Contains(addr));
    }
}
