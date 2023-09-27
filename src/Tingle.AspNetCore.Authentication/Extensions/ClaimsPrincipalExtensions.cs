using Tingle.AspNetCore.Authentication;
using SC = Tingle.AspNetCore.Authentication.AuthenticationJsonSerializerContext;

namespace System.Security.Claims;

/// <summary>
/// Extension methods for <see cref="ClaimsPrincipal"/>
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Get the user's email using the <c>email</c> or <see cref="ClaimTypes.Email"/> type.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindFirstValue("email") ?? principal.FindFirstValue(ClaimTypes.Email);
    }

    /// <summary>
    /// Get if the user's email is verified using the '<c>email_verified</c>' type.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static bool? GetEmailVerified(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        var value = principal.FindFirstValue("email_verified");
        if (bool.TryParse(value, out var result)) return result;
        return null;
    }

    /// <summary>
    /// Get the User Principal Name (UPN) using the <see cref="ClaimTypes.Upn"/> type.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? GetUpn(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindFirstValue(ClaimTypes.Upn);
    }

    /// <summary>
    /// Get the user's email or the UPN (User Principal Name) when the email is not found.
    /// This uses the <see cref="ClaimTypes.Email"/> or <see cref="ClaimTypes.Upn"/> type.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? GetEmailOrUpn(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.GetEmail() ?? principal.GetUpn();
    }

    /// <summary>
    /// Get the user's preferred username using the '<c>preferred_username</c>' type.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? GetPreferredUsername(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindFirstValue("preferred_username");
    }

    /// <summary>
    /// Get the user's name using the '<c>name</c>' or <see cref="ClaimTypes.Name"/> type.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? GetName(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindFirstValue("name") ?? principal.FindFirstValue(ClaimTypes.Name);
    }

    /// <summary>
    /// Get the entity's name identifier using the <see cref="ClaimTypes.NameIdentifier"/> type.
    /// </summary>
    /// <remarks>
    /// The principal about which the token asserts information, such as the user of an app. This value is
    /// immutable and cannot be reassigned or reused. The subject is a pairwise identifier - it is unique to a
    /// particular application ID. If a single user signs into two different apps using two different client IDs,
    /// those apps will receive two different values for the subject claim. This may or may not be wanted
    /// depending on your architecture and privacy requirements.
    /// </remarks>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? GetNameId(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    /// <summary>
    /// Get the entity's identifier using the <c>sub</c> type.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? GetSub(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindFirstValue("sub");
    }

    /// <summary>
    /// Get the entity's object identifier using the <see cref="Constants.ObjectIdentifierClaimType"/> type.
    /// </summary>
    /// <remarks>
    /// The immutable identifier for an object in the Microsoft identity system, in this case, a user account.
    /// This ID uniquely identifies the user across applications - two different applications signing in the
    /// same user will receive the same value in the oid claim. The Microsoft Graph will return this ID as the
    /// id property for a given user account. Because the oid allows multiple apps to correlate users, the profile
    /// scope is required to receive this claim. Note that if a single user exists in multiple tenants, the user
    /// will contain a different object ID in each tenant - they're considered different accounts, even though the
    /// user logs into each account with the same credentials. The oid claim is a GUID and cannot be reused.
    /// </remarks>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? GetObjectId(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindFirstValue(Constants.ObjectIdentifierClaimType);
    }

    /// <summary>
    /// Get the user's unique identifier.
    /// This uses the <see cref="Constants.ObjectIdentifierClaimType"/> or <see cref="ClaimTypes.NameIdentifier"/> type.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.GetObjectId() ?? principal.GetNameId() ?? principal.GetSub();
    }

    /// <summary>
    /// Get the identifier of the tenant for the user using the <see cref="Constants.TenantIdClaimType"/> type.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? GetTenantId(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindFirstValue(Constants.TenantIdClaimType);
    }

    /// <summary>
    /// Get the user's phone number using the '<c>phone_number</c>' type.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? GetPhoneNumber(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindFirstValue("phone_number");
    }

    /// <summary>
    /// Get if the user's phone number is verified using the '<c>phone_number_verified</c>' type.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static bool? GetPhoneNumberVerified(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        var value = principal.FindFirstValue("phone_number_verified");
        if (bool.TryParse(value, out var result)) return result;
        return null;
    }

    /// <summary>
    /// Get the user's address using the '<c>address</c>' type.
    /// This is usually a JSON object.
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static string? GetAddress(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.FindFirstValue("address");
    }

    /// <summary>
    /// Get the user's address using the '<c>address</c>' type.
    /// This is usually a JSON object that is then decoded into an object
    /// </summary>
    /// <param name="principal"></param>
    /// <returns></returns>
    public static AddressClaim? GetAddressDecoded(this ClaimsPrincipal principal)
    {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        var json = principal.GetAddress();
        if (string.IsNullOrWhiteSpace(json)) return default;
        return Text.Json.JsonSerializer.Deserialize(json, SC.Default.AddressClaim);
    }
}
