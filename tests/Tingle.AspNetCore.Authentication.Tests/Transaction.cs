using System.Xml.Linq;

#nullable disable // aligned with tests for inbuilt authentication

namespace Tingle.AspNetCore.Authentication.Tests;

public class Transaction
{
    public HttpRequestMessage Request { get; set; }
    public HttpResponseMessage Response { get; set; }

    public IList<string> SetCookie { get; set; }

    public string ResponseText { get; set; }
    public XElement ResponseElement { get; set; }

    public string AuthenticationCookieValue
    {
        get
        {
            if (SetCookie != null && SetCookie.Count > 0)
            {
                var authCookie = SetCookie.SingleOrDefault(c => c.Contains(".AspNetCore." + TestExtensions.CookieAuthenticationScheme + "="));
                if (authCookie != null)
                {
                    return authCookie.Substring(0, authCookie.IndexOf(';'));
                }
            }

            return null;
        }
    }

    public string FindClaimValue(string claimType, string issuer = null)
    {
        var claim = ResponseElement.Elements("claim")
            .SingleOrDefault(elt => elt.Attribute("type").Value == claimType &&
                (issuer == null || elt.Attribute("issuer").Value == issuer));
        if (claim == null)
        {
            return null;
        }
        return claim.Attribute("value").Value;
    }

    public string FindTokenValue(string name)
    {
        var claim = ResponseElement.Elements("token")
            .SingleOrDefault(elt => elt.Attribute("name").Value == name);
        if (claim == null)
        {
            return null;
        }
        return claim.Attribute("value").Value;
    }

}
