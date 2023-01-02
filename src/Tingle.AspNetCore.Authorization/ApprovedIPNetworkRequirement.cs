using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Tingle.AspNetCore.Authorization;

/// <summary>
/// An <see cref="IAuthorizationRequirement"/> that contains IP networks
/// </summary>
public sealed class ApprovedIPNetworkRequirement : IAuthorizationRequirement
{
    private readonly IList<IPNetwork> networks;

    /// <summary>
    /// Creates and instance of <see cref="ApprovedIPNetworkRequirement"/>
    /// </summary>
    /// <param name="networks">the networks allowed</param>
    public ApprovedIPNetworkRequirement(IList<IPNetwork> networks)
    {
        this.networks = networks ?? throw new ArgumentNullException(nameof(networks));
    }

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

        return networks.Contains(addr);
    }
}
