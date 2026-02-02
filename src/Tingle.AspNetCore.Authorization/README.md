# Tingle.AspNetCore.Authorization

Authorization refers to the process that determines what a user is able to do. For example, an administrative user is allowed to create a document library, add documents, edit documents, and delete them. A non-administrative user working with the library is only authorized to read the documents.

Authorization is orthogonal and independent of authentication. However, authorization requires an authentication mechanism. Authentication is the process of ascertaining who a user is. Authentication may create one or more identities for the current user.

Below are some of the functionalities that the library provides to aid with authorization work flows.

## IP Address Based Authorization

### User Defined IPs

It is a common scenario whereby we may require to only allow HTTP requests from certain IPs.

In `appsettings.json`

```json
{
    "AllowedNetworks": [
      "::1/128",
      "127.0.0.1/32"
    ]
}
```

In `Program.cs`

```cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("my_auth_policy", policy =>
    {
        policy.AddAuthenticationSchemes("my_auth_scheme")
              .RequireAuthenticatedUser()
              .RequireApprovedNetworks(Configuration.GetSection("AllowedNetworks"));
    });
});

// add accessor for HttpContext i.e. implementation of IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// add IAuthorizationHandler for approved networks
builder.Services.AddApprovedNetworksHandler();
```

Details of the implementation of `my_auth_scheme` authentication scheme have been omitted here since it is beyond the scope of this discussion. More details on how to handle authentication in ASP.NET Core can be found [here](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-10.0).

The above code section defines `my_auth_policy` authorization policy which ensures the user who has been authenticated via the `my_auth_scheme` has access to the resource they're trying to gain access to. Using `RequireApprovedNetworks` extension method on the `AuthorizationPolicyBuilder` we can then add a comma separated list of IP networks that are approved to access the resource from.

We also have added a call to the `services.AddHttpContextAccessor()` extension method in order to allow us to gain access to the `HttpContext` which contains the details of the IP address that the request is originating from.

Finally, we have a call to the `services.AddApprovedNetworksHandler()` which adds an instance of the `ApprovedIPNetworkHandler`. This authorization handler then makes a decision if authorization is allowed by checking if the request IP is among the networks provided in the authorization policy.

Now, we can use this functionality to authorize access to a controller as shown below:

```cs
[Authorize("my_auth_policy")]
public class DummyController : ControllerBase
{
    // ..
}
```

Is that it?... Wait there's more!

### Fully Qualified Domain Names

Alternatively, you can provide a list of fully qualified domain names and each of them will be resolved to the list of IP addresses. Let us see how to do this with an example:

In `appsettings.json`

```json
{
  "AllowedDomains": ["contoso.com", "northwind.com"]
}
```

In `Program.cs`

```cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("my_auth_policy", policy =>
    {
        policy.AddAuthenticationSchemes("my_auth_scheme")
              .RequireAuthenticatedUser()
              .RequireNetworkFromDns(Configuration.GetSection("AllowedDomains"));
    });
});

// add accessor for HttpContext i.e. implementation of IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// add IAuthorizationHandler for approved networks
builder.Services.AddApprovedNetworksHandler();
```

### Azure IPs

For developers who are working with Microsoft Azure, and they'd wish to allow all their IP addresses they can do that easily as demonstrated below:

In Program.cs

```cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("my_auth_policy", policy =>
    {
        policy.AddAuthenticationSchemes("my_auth_scheme")
              .RequireAuthenticatedUser()
              .RequireAzureIPNetworks();
    });
});

// add accessor for HttpContext i.e. implementation of IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// add IAuthorizationHandler for approved networks
builder.Services.AddApprovedNetworksHandler();
```

If you however do not wish to allow the entire range of Azure IPs in a given cloud, you can provide `service` and `region` parameters to `RequireAzureIPNetworks` to scope the range of IPs based on the Azure service and/or region. For example:

```cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("my_auth_policy", policy =>
    {
        policy.AddAuthenticationSchemes("my_auth_scheme")
              .RequireAuthenticatedUser()
              .RequireAzureIPNetworks(cloud: AzureCloud.Public, service: "AzureAppService", region: "westeurope");
    });
});

// add accessor for HttpContext i.e. implementation of IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// add IAuthorizationHandler for approved networks
builder.Services.AddApprovedNetworksHandler();
```
