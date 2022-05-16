namespace Tingle.Extensions.Http.Authentication;

/// <summary>
/// Authentication provider for OAuth Client Credentials flow (see the OAuth 2.0 spec for more details) in Azure Active Directory
/// </summary>
public class AzureAdB2BHandler : OAuthClientCredentialHandler
{
    /// <summary>
    /// Creates a new instance of the <see cref="AzureAdB2BHandler"/> class.
    /// </summary>
    public AzureAdB2BHandler() { }

    /// <summary>
    /// Creates a new instance of the <see cref="AzureAdB2BHandler"/> class with a specific inner handler.
    /// </summary>
    /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
    public AzureAdB2BHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    /// <summary>
    /// The tenant identifier of the Azure Active Directory Tenant. This can either be in GUID format or as a domain e.g. contoso.onmicrosoft.com
    /// </summary>
    public virtual string? TenantId { get; set; }

    /// <summary>
    /// The authentication endpoint to be used to request a token
    /// </summary>
    public override string? AuthenticationEndpoint => $"https://login.windows.net/{TenantId}/oauth2/token";
}
