namespace Tingle.AspNetCore.Authentication;

/// <summary></summary>
internal static class Constants
{
    /// <summary>
    /// The URI for a claim that specifies the object identifier of an entity, http://schemas.microsoft.com/identity/claims/objectidentifier.
    /// </summary>
    public const string ObjectIdentifierClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

    /// <summary>
    /// The URI for a claim that specifies the tenant identifier of an entity, http://schemas.microsoft.com/identity/claims/tenantid.
    /// </summary>
    public const string TenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";
}
