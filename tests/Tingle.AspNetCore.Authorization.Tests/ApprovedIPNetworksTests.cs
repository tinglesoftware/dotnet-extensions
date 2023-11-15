using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Tingle.AspNetCore.Authorization.Tests;

public class ApprovedIPNetworksTests
{
    [Theory]
    [InlineData(true, "127.0.0.1", "127.0.0.1/32,::1/128")]
    [InlineData(false, "127.0.1.1", "127.0.0.1/32,::1/128")]
    [InlineData(false, "127.0.1.1", "192.201.214.0/24,::1/128")]
    [InlineData(false, "30.0.0.21", "192.201.214.0/24")]
    [InlineData(true, "207.154.225.144", "207.154.225.144/32")]
    [InlineData(true, "196.201.214.94", "196.201.214.0/24")]
    [InlineData(true, "196.201.214.94", "196.201.214.0/24,30.0.0.0/27")]
    [InlineData(true, "30.0.0.21", "196.201.214.0/24,30.0.0.0/27")]
    [InlineData(true, "::ffff:196.201.214.127", "196.201.214.0/24")] // IPv4 mapped to IPv6
    // TODO: remove this once the inbuilt implementation support IPv6
#if !NET8_0_OR_GREATER
    [InlineData(false, "2001:0000:0000:1234:abcd:ffff:c0a8:0101", "2002::1234:abcd:ffff:c0a8:101/64")]
    [InlineData(true, "2002:0000:0000:1234:abcd:ffff:c0a8:0101", "2002::1234:abcd:ffff:c0a8:101/64")]
#endif
    public void IsApproved_Works(bool expected, string test, string networks)
    {
        var builder = new AuthorizationPolicyBuilder();
        builder.RequireApprovedNetworks(networks.Split(','));
        var r = Assert.Single(builder.Requirements);
        var requirement = Assert.IsAssignableFrom<ApprovedIPNetworkRequirement>(r);
        var actual = requirement.IsApproved(IPAddress.Parse(test));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true, "127.0.0.1", "127.0.0.1/32", "::1/128", "192.201.214.0/24")]
    [InlineData(false, "30.0.0.21", "127.0.0.1/32", "::1/128", "192.201.214.0/24")]
    public void RequireApprovedNetworks_Configuration_Works(bool expected, string test, params string[] networks)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(networks);
        json = $"{{\"ApprovedNetworks\":{json}}}";
        var configuration = new ConfigurationBuilder()
            .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            .Build();
        var builder = new AuthorizationPolicyBuilder();
        builder.RequireApprovedNetworks(configuration.GetSection("ApprovedNetworks"));
        var r = Assert.Single(builder.Requirements);
        var requirement = Assert.IsAssignableFrom<ApprovedIPNetworkRequirement>(r);
        var actual = requirement.IsApproved(IPAddress.Parse(test));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RequireNetworkFromDns_Skips_UnknownHost()
    {
        var builder = new AuthorizationPolicyBuilder();
        builder.RequireNetworkFromDns("cakes.maxwellweru.com");
        Assert.Empty(builder.Requirements);
    }

    [Fact]
    public void RequireNetworkFromDns_Works()
    {
        var builder = new AuthorizationPolicyBuilder();
        builder.RequireNetworkFromDns("maxwellweru.com");
        var r = Assert.Single(builder.Requirements);
        _ = Assert.IsAssignableFrom<ApprovedIPNetworkRequirement>(r);
    }

    [Fact]
    public void RequireAzureIPNetworks_Works()
    {
        var builder = new AuthorizationPolicyBuilder();
        builder.RequireAzureIPNetworks();
        var r = Assert.Single(builder.Requirements);
        _ = Assert.IsAssignableFrom<ApprovedIPNetworkRequirement>(r);

        // now filter by known region
        builder = new AuthorizationPolicyBuilder();
        builder.RequireAzureIPNetworks(region: "westeurope");
        r = Assert.Single(builder.Requirements);
        _ = Assert.IsAssignableFrom<ApprovedIPNetworkRequirement>(r);

        // now filter by unknown region
        builder = new AuthorizationPolicyBuilder();
        builder.RequireAzureIPNetworks(region: "mine");
        Assert.Empty(builder.Requirements);

        // now filter by known service tag
        builder = new AuthorizationPolicyBuilder();
        builder.RequireAzureIPNetworks(service: "AzureAppService");
        r = Assert.Single(builder.Requirements);
        _ = Assert.IsAssignableFrom<ApprovedIPNetworkRequirement>(r);

        // now filter by unknown service tag
        builder = new AuthorizationPolicyBuilder();
        builder.RequireAzureIPNetworks(service: "MyService");
        Assert.Empty(builder.Requirements);
    }
}
