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
    public static AuthorizationPolicyBuilder RequireApprovedNetworks(this AuthorizationPolicyBuilder builder, IList<IPNetwork> networks)
    {
        // if there are no networks just return
        if (!networks.Any()) return builder;

        // add the requirement
        return builder.AddRequirements(new ApprovedIPNetworkRequirement(networks));
    }

    /// <summary>
    /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance.
    /// Ensure  the necessary Authorization and framework services are added to the same collection
    /// using <c>services.AddApprovedNetworksHandler(...)</c>.
    /// </summary>
    /// <param name="builder">The instance to add to</param>
    /// <param name="networks">The allowed networks</param>
    public static AuthorizationPolicyBuilder RequireApprovedNetworks(this AuthorizationPolicyBuilder builder, params string[] networks)
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
    public static AuthorizationPolicyBuilder RequireApprovedNetworks(this AuthorizationPolicyBuilder builder, params IPNetwork[] networks)
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
    /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance, using known Azure IPs that are cached locally.
    /// Ensure the necessary Authorization and framework services are added to the same collection
    /// using <c>services.AddApprovedNetworksHandler(...)</c>.
    /// Networks used are retrieved using <see cref="AzureIPNetworks.AzureIPsProvider.Local"/>.
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
        => builder.RequireAzureIPNetworks(AzureIPNetworks.AzureIPsProvider.Local, cloud, service, region);

    /// <summary>
    /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance, using known Azure IPs from an instance of <see cref="AzureIPNetworks.AzureIPsProvider"/>.
    /// Ensure the necessary Authorization and framework services are added to the same collection
    /// using <c>services.AddApprovedNetworksHandler(...)</c>.
    /// </summary>
    /// <param name="builder">The instance to add to</param>
    /// <param name="provider">The <see cref="AzureIPNetworks.AzureIPsProvider"/> to use for retrieval.</param>
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
                                                                    AzureIPNetworks.AzureIPsProvider provider,
                                                                    AzureIPNetworks.AzureCloud cloud = AzureIPNetworks.AzureCloud.Public,
                                                                    string? service = null,
                                                                    string? region = null)
    {
        var networks = provider.GetNetworksAsync(cloud, service, region)
                               .AsTask()
                               .GetAwaiter()
                               .GetResult()
                               .ToArray();

        // create the requirement and add it to the builder
        return builder.RequireApprovedNetworks(networks);
    }

    /// <summary>
    /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance, using known Azure IPs that are cached locally.
    /// Ensure the necessary Authorization and framework services are added to the same collection
    /// using <c>services.AddApprovedNetworksHandler(...)</c>.
    /// Networks used are retrieved using <see cref="AzureIPNetworks.AzureIPsProvider"/>.
    /// </summary>
    /// <param name="builder">The instance to add to</param>
    /// <param name="cloud">The Azure Cloud which to allow.</param>
    /// <param name="services">
    /// (Optional) The name of the services whose IP ranges to allow.
    /// When not provided(null), IPs from all services are added.
    /// </param>
    /// <param name="regions">
    /// (Optional) The name of the regions whose IP ranges to allow.
    /// When not provided(null), IPs from all regions are added.
    /// </param>
    public static AuthorizationPolicyBuilder RequireAzureIPNetworks(this AuthorizationPolicyBuilder builder,
                                                                    AzureIPNetworks.AzureCloud cloud,
                                                                    IReadOnlyList<string> services,
                                                                    IReadOnlyList<string> regions)
        => builder.RequireAzureIPNetworks(AzureIPNetworks.AzureIPsProvider.Local, cloud, services, regions);

    /// <summary>
    /// Adds an <see cref="ApprovedIPNetworkRequirement"/> to the current instance, using known Azure IPs from an instance of <see cref="AzureIPNetworks.AzureIPsProvider"/>.
    /// Ensure the necessary Authorization and framework services are added to the same collection
    /// using <c>services.AddApprovedNetworksHandler(...)</c>.
    /// </summary>
    /// <param name="builder">The instance to add to</param>
    /// <param name="provider">The <see cref="AzureIPNetworks.AzureIPsProvider"/> to use for retrieval.</param>
    /// <param name="cloud">The Azure Cloud which to allow.</param>
    /// <param name="services">
    /// (Optional) The name of the services whose IP ranges to allow.
    /// When not provided(null), IPs from all services are added.
    /// </param>
    /// <param name="regions">
    /// (Optional) The name of the regions whose IP ranges to allow.
    /// When not provided(null), IPs from all regions are added.
    /// </param>
    public static AuthorizationPolicyBuilder RequireAzureIPNetworks(this AuthorizationPolicyBuilder builder,
                                                                    AzureIPNetworks.AzureIPsProvider provider,
                                                                    AzureIPNetworks.AzureCloud cloud,
                                                                    IReadOnlyList<string> services,
                                                                    IReadOnlyList<string> regions)
    {
        var networks = provider.GetNetworksAsync(cloud, services, regions)
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
    public static AuthorizationPolicyBuilder RequireNetworkFromDns(this AuthorizationPolicyBuilder builder, params string[] fqdns)
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
                var rawNetworks = ips?.Select(ip => new IPNetwork(ip, (byte)(ip.AddressFamily is AddressFamily.InterNetwork ? 32 : 128)));

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
