﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;

#nullable disable // aligned with tests for inbuilt authentication

namespace Tingle.AspNetCore.Authentication.Tests;

public static class TestExtensions
{
    public const string CookieAuthenticationScheme = "External";

    public static async Task<Transaction> SendAsync(this TestServer server, string uri, string cookieHeader = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        if (!string.IsNullOrEmpty(cookieHeader))
        {
            request.Headers.Add("Cookie", cookieHeader);
        }
        var transaction = new Transaction
        {
            Request = request,
            Response = await server.CreateClient().SendAsync(request),
        };
        if (transaction.Response.Headers.Contains("Set-Cookie"))
        {
            transaction.SetCookie = [.. transaction.Response.Headers.GetValues("Set-Cookie")];
        }
        transaction.ResponseText = await transaction.Response.Content.ReadAsStringAsync();

        if (transaction.Response.Content != null &&
            transaction.Response.Content.Headers.ContentType != null &&
            transaction.Response.Content.Headers.ContentType.MediaType == "text/xml")
        {
            transaction.ResponseElement = XElement.Parse(transaction.ResponseText);
        }
        return transaction;
    }

    public static Task DescribeAsync(this HttpResponse res, ClaimsPrincipal principal)
    {
        res.StatusCode = 200;
        res.ContentType = "text/xml";
        var xml = new XElement("xml");
        if (principal != null)
        {
            foreach (var identity in principal.Identities)
            {
                xml.Add(identity.Claims.Select(claim =>
                    new XElement("claim", new XAttribute("type", claim.Type),
                    new XAttribute("value", claim.Value),
                    new XAttribute("issuer", claim.Issuer))));
            }
        }
        var xmlBytes = Encoding.UTF8.GetBytes(xml.ToString());
        return res.Body.WriteAsync(xmlBytes, 0, xmlBytes.Length);
    }

    public static Task DescribeAsync(this HttpResponse res, IEnumerable<AuthenticationToken> tokens)
    {
        res.StatusCode = 200;
        res.ContentType = "text/xml";
        var xml = new XElement("xml");
        if (tokens != null)
        {
            foreach (var token in tokens)
            {
                xml.Add(new XElement("token", new XAttribute("name", token.Name),
                    new XAttribute("value", token.Value)));
            }
        }
        var xmlBytes = Encoding.UTF8.GetBytes(xml.ToString());
        return res.Body.WriteAsync(xmlBytes, 0, xmlBytes.Length);
    }

}
