using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Tingle.AspNetCore.Authentication.Tests;

public class ClaimsPrincipalExtensionsTests
{
    const string ValidIssuer = "issuer.contoso.com";
    const string ValidAudience = "audience.contoso.com";

    [Theory]
    // resembles AzureAd token for user
    [InlineData(null,
                false,
                "mburumaxwell@tingle.software",
                "993143b6-3214-4bcb-8fd1-7782764d7858",
                "J0-RjswRJ8bAIDSW99adHLN-keZUivIgCOMQcUstc8Y",
                "Maxwell Weru",
                null,
                "80aeb978-f48b-4a62-a6d5-54b3d2a299d5",
                null,
                false)]
    // resembles AzureAd token for Guest user
    [InlineData("mburumaxwell@tingle.software",
                false,
                null,
                "17c70c67-7c6f-43f0-8e72-b0dd23e0b7e9",
                "tvmexzQHw_vNSWITn8iHlHnKySIl8mpplPph8zyjHIQ",
                "Maxwell Weru",
                null,
                "b6a92e38-bf38-40ac-9704-779a9d9f6d18",
                null,
                false)]
    // resembles Firebase token
    [InlineData("mburumaxwell@gmail.com",
                true,
                null,
                null,
                "UjFwZHjtrdTTVXwMNG0N7m9KXah2",
                "Maxwell Weru",
                "mburumaxwell@gmail.com",
                null,
                "+254728837078",
                true)]
    public void PrincipalExtensionsWork(string expectedEmail,
                                        bool expectedEmailVerified,
                                        string expectedUpn,
                                        string expectedObjectId,
                                        string expectedNameId,
                                        string expectedName,
                                        string expectedPreferredUsername,
                                        string expectedTenantId,
                                        string expectedPhoneNumber,
                                        bool expectedPhoneNumberVerified)
    {
        (var token, var key) = CreateStandardTokenAndKey(email: expectedEmail,
                                                         email_verified: expectedEmailVerified,
                                                         upn: expectedUpn,
                                                         objectId: expectedObjectId,
                                                         nameId: expectedNameId,
                                                         name: expectedName,
                                                         preferred_username: expectedPreferredUsername,
                                                         tenantId: expectedTenantId,
                                                         phone_number: expectedPhoneNumber,
                                                         phone_number_verified: expectedPhoneNumberVerified);

        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = ValidIssuer,
            ValidAudience = ValidAudience,
            IssuerSigningKey = key,
        };

        var validator = new JwtSecurityTokenHandler();
        var principal = validator.ValidateToken(token, validationParameters, out _);
        Assert.NotNull(principal);
        Assert.Equal(expectedEmail, principal.GetEmail());
        Assert.Equal(expectedEmailVerified, principal.GetEmailVerified());
        Assert.Equal(expectedUpn, principal.GetUpn());
        Assert.Equal(expectedObjectId, principal.GetObjectId());
        Assert.Equal(expectedNameId, principal.GetNameId());
        Assert.Equal(expectedName, principal.GetName());
        Assert.Equal(expectedPreferredUsername, principal.GetPreferredUsername());
        Assert.Equal(expectedTenantId, principal.GetTenantId());
        Assert.Equal(expectedPhoneNumber, principal.GetPhoneNumber());
        Assert.Equal(expectedPhoneNumberVerified, principal.GetPhoneNumberVerified());

        // assert computed properties
        Assert.Equal(expectedEmail ?? expectedUpn, principal.GetEmailOrUpn());
        Assert.Equal(expectedObjectId ?? expectedNameId, principal.GetUserId());
    }


    private static (string token, SymmetricSecurityKey key) CreateStandardTokenAndKey(string email,
                                                                                      bool email_verified,
                                                                                      string upn,
                                                                                      string objectId,
                                                                                      string nameId,
                                                                                      string name,
                                                                                      string preferred_username,
                                                                                      string tenantId,
                                                                                      string phone_number,
                                                                                      bool phone_number_verified)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(new string('a', 128)));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, nameId),
            new Claim("name", name),
        };

        if (!string.IsNullOrEmpty(preferred_username))
        {
            claims.Add(new Claim("preferred_username", preferred_username));
        }

        if (!string.IsNullOrEmpty(email))
        {
            claims.Add(new Claim(ClaimTypes.Email, email));
        }

        claims.Add(new Claim("email_verified", email_verified.ToString()));

        if (!string.IsNullOrEmpty(upn))
        {
            claims.Add(new Claim(ClaimTypes.Upn, upn));
        }

        if (!string.IsNullOrEmpty(objectId))
        {
            claims.Add(new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", objectId));
        }

        if (!string.IsNullOrEmpty(tenantId))
        {
            claims.Add(new Claim("http://schemas.microsoft.com/identity/claims/tenantid", tenantId));
        }

        if (!string.IsNullOrEmpty(phone_number))
        {
            claims.Add(new Claim("phone_number", phone_number));
        }

        claims.Add(new Claim("phone_number_verified", phone_number_verified.ToString()));

        var token = new JwtSecurityToken(issuer: ValidIssuer,
                                         audience: ValidAudience,
                                         claims: claims,
                                         expires: DateTime.Now.AddMinutes(30),
                                         signingCredentials: creds);

        var tokenText = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenText, key);
    }

    [Fact]
    public void GetAddressDecoded_Works()
    {
        var addr_claim_value = "{\"street_address\": \"P.O.Box 20769\",\"postal_code\": \"00100\",\"locality\": \"Nairobi\",\"region\": null,\"country\": \"Kenya\"}";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(new string('a', 128)));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1234567890"),
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "1234567890"),
            new Claim("address", addr_claim_value),
        };


        var token = new JwtSecurityToken(issuer: ValidIssuer,
                                         audience: ValidAudience,
                                         claims: claims,
                                         expires: DateTime.Now.AddMinutes(30),
                                         signingCredentials: creds);

        var tokenText = new JwtSecurityTokenHandler().WriteToken(token);

        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = ValidIssuer,
            ValidAudience = ValidAudience,
            IssuerSigningKey = key,
        };

        var validator = new JwtSecurityTokenHandler();
        var principal = validator.ValidateToken(tokenText, validationParameters, out _);
        Assert.NotNull(principal);
        Assert.Equal("1234567890", principal.GetNameId());
        Assert.Equal("1234567890", principal.GetObjectId());
        Assert.Equal(addr_claim_value, principal.GetAddress());
        var addr = principal.GetAddressDecoded();
        Assert.NotNull(addr);
        Assert.Equal("P.O.Box 20769", addr?.StreetAddress);
        Assert.Equal("Nairobi", addr?.Locality);
        Assert.Null(addr?.Region);
        Assert.Equal("00100", addr?.PostalCode);
        Assert.Equal("Kenya", addr?.Country);
    }

    [Fact]
    public void GetAddressDecoded_IgnoresNull()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(new string('a', 128)));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1234567890"),
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "1234567890"),
        };


        var token = new JwtSecurityToken(issuer: ValidIssuer,
                                         audience: ValidAudience,
                                         claims: claims,
                                         expires: DateTime.Now.AddMinutes(30),
                                         signingCredentials: creds);

        var tokenText = new JwtSecurityTokenHandler().WriteToken(token);

        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = ValidIssuer,
            ValidAudience = ValidAudience,
            IssuerSigningKey = key,
        };

        var validator = new JwtSecurityTokenHandler();
        var principal = validator.ValidateToken(tokenText, validationParameters, out _);
        Assert.NotNull(principal);
        Assert.Equal("1234567890", principal.GetNameId());
        Assert.Equal("1234567890", principal.GetObjectId());
        Assert.Null(principal.GetAddress());
        var addr = principal.GetAddressDecoded();
        Assert.Null(addr);
    }

    [Fact]
    public void GetAddressDecoded_IgnoresWhitespace()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(new string('a', 128)));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1234567890"),
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "1234567890"),
            new Claim("address", ""),
        };


        var token = new JwtSecurityToken(issuer: ValidIssuer,
                                         audience: ValidAudience,
                                         claims: claims,
                                         expires: DateTime.Now.AddMinutes(30),
                                         signingCredentials: creds);

        var tokenText = new JwtSecurityTokenHandler().WriteToken(token);

        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = ValidIssuer,
            ValidAudience = ValidAudience,
            IssuerSigningKey = key,
        };

        var validator = new JwtSecurityTokenHandler();
        var principal = validator.ValidateToken(tokenText, validationParameters, out _);
        Assert.NotNull(principal);
        Assert.Equal("1234567890", principal.GetNameId());
        Assert.Equal("1234567890", principal.GetObjectId());
        var addr_s = principal.GetAddress();
        Assert.NotNull(addr_s);
        Assert.Empty(addr_s);
        var addr = principal.GetAddressDecoded();
        Assert.Null(addr);
    }
}
