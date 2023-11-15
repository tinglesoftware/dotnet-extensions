using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using Tingle.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/> related to authorization
/// </summary>
public static class AuthorizationPolicyBuilderExtensions
{
    /// <summary>
    /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance.
    /// Ensure the necessary Authorization and framework services are added to the same collection
    /// using <c>services.AddApprovedNetworksHandler(...)</c>.
    /// </summary>
    /// <param name="builder">The instance to add to</param>
    /// <param name="networks">The allowed networks</param>
    public static AuthorizationPolicyBuilder RequireApprovedNetworks(this AuthorizationPolicyBuilder builder,
                                                                     IList<IPNetwork> networks)
    {
        // if there are no networks just return
        if (!networks.Any()) return builder;

        // reduce the networks where possible (referred to as supernetting)
#if NET8_0_OR_GREATER
        var reduced = networks;
#else
        var reduced = IPNetwork.Supernet(networks.ToArray());
#endif

        // add the requirement
        return builder.AddRequirements(new ApprovedIPNetworkRequirement(reduced));
    }

    /// <summary>
    /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance.
    /// Ensure  the necessary Authorization and framework services are added to the same collection
    /// using <c>services.AddApprovedNetworksHandler(...)</c>.
    /// </summary>
    /// <param name="builder">The instance to add to</param>
    /// <param name="networks">The allowed networks</param>
    public static AuthorizationPolicyBuilder RequireApprovedNetworks(this AuthorizationPolicyBuilder builder,
                                                                     params string[] networks)
    {
        var parsed = networks.Select(a => IPNetwork.Parse(a)).ToList();
        return builder.RequireApprovedNetworks(parsed);
    }

    /// <summary>
    /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance.
    /// Ensure the necessary Authorization and framework services are added to the same collection
    /// using <c>services.AddApprovedNetworksHandler(...)</c>.
    /// </summary>
    /// <param name="builder">The instance to add to</param>
    /// <param name="networks">The allowed networks</param>
    public static AuthorizationPolicyBuilder RequireApprovedNetworks(this AuthorizationPolicyBuilder builder,
                                                                     params IPNetwork[] networks)
    {
        return builder.RequireApprovedNetworks(networks.ToList());
    }

    /// <summary>
    /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance based on the configuration
    /// section provided. Ensure the necessary Authorization and framework services are added to the same
    /// collection using <c>services.AddApprovedNetworksHandler(...)</c>.
    /// </summary>
    /// <param name="builder">The instance to add to</param>
    /// <param name="section">
    /// The <see cref="IConfiguration"/> containing the values at the root.
    /// It must be bindable to an instance of <c>List&lt;string&gt;</c>
    /// </param>
    /// <returns></returns>
    [RequiresDynamicCode("Binding strongly typed objects to configuration values requires generating dynamic code at runtime, for example instantiating generic types.")]
    [RequiresUnreferencedCode("Cannot statically analyze the type of instance so its members may be trimmed")]
    public static AuthorizationPolicyBuilder RequireApprovedNetworks(this AuthorizationPolicyBuilder builder, IConfiguration section)
    {
        var list = new List<string>();
        section.Bind(list);
        return builder.RequireApprovedNetworks(list.ToArray());
    }

    /// <summary>
    /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance, using Known Azure IPs.
    /// Ensure the necessary Authorization and framework services are added to the same collection
    /// using <c>services.AddApprovedNetworksHandler(...)</c>.
    /// Networks used are retrieved using <see cref="AzureIPNetworks.AzureIPsHelper"/>.
    /// </summary>
    /// <param name="builder">The instance to add to</param>
    /// <param name="cloud">The Azure Cloud which to allow.</param>
    /// <param name="service">
    /// (Optional) The name of the service whose IP ranges to allow.
    /// When not provided(null), IPs from all services are added.
    /// </param>
    /// <param name="region">
    /// (Optional) The name of the region whose IP ranges to allow.
    /// When not provided(null), IPs from all regions are added.
    /// </param>
    public static AuthorizationPolicyBuilder RequireAzureIPNetworks(this AuthorizationPolicyBuilder builder,
                                                                    AzureIPNetworks.AzureCloud cloud = AzureIPNetworks.AzureCloud.Public,
                                                                    string? service = null,
                                                                    string? region = null)
    {
        var networks = AzureIPNetworks.AzureIPsProvider.Local.GetNetworksAsync(cloud, service, region)
                                                             .AsTask()
                                                             .GetAwaiter()
                                                             .GetResult()
                                                             .ToArray();

        // create the requirement and add it to the builder
        return builder.RequireApprovedNetworks(networks);
    }

    /// <summary>
    /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance using IPs resolved via DNS.
    /// Ensure the necessary Authorization and framework services are added to the same collection
    /// using <c>services.AddApprovedNetworksHandler(...)</c>.
    /// </summary>
    /// <param name="builder">The instance to add to</param>
    /// <param name="fqdns">
    /// A list of Fully Qualified Domain Names.
    /// Each of them will be resolved to list of IP addresses using <see cref="Dns.GetHostAddresses(string)"/>
    /// </param>
    public static AuthorizationPolicyBuilder RequireNetworkFromDns(this AuthorizationPolicyBuilder builder,
                                                                   params string[] fqdns)
    {
        var networks = new List<IPNetwork>();

        // work on each FQDN
        foreach (var f in fqdns)
        {
            try
            {
                // resolve the IP address from the hostname
                var ips = Dns.GetHostAddresses(f);

                // parse the IP addresses into IP networks
#if NET8_0_OR_GREATER
                var rawNetworks = ips?.Select(ip => IPNetwork.Parse($"{ip}/32")) ?? Array.Empty<IPNetwork>();
#else
                var rawNetworks = ips?.Select(ip => IPNetwork.Parse(ip.ToString(), CidrGuess.ClassLess)) ?? Array.Empty<IPNetwork>();
#endif

                // add networks into the list if there are any
                if (rawNetworks?.Any() ?? false)
                    networks.AddRange(rawNetworks);

            }
            catch (SocketException se) when (se.SocketErrorCode == SocketError.HostNotFound)
            {
                continue;
            }
        }

        // if there are no networks, return
        if (networks.Count == 0) return builder;

        // create the requirement and add it to the builder
        return builder.RequireApprovedNetworks(networks);
    }
}
