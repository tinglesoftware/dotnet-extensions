# Tingle.Extensions.Http.Authentication

This library adds support for custom authentication via the Authorization header when using `HttpClient`. In some cases, setting the header once is not sufficient. Instead you may want to refresh the token only after it expires.
This functionality builds upon the `DelegatingHandler` and hence can be used via DI using `IHttpClientBuilder` or without by wrapping inner handlers.

The supported authentication patterns:

## API Key in the `Authorization` header

Example:

```cs
builder.Services.AddHttpClient<MyCustomClient>()
                .AddApiKeyHeaderAuthenticationHandler("my-api-key-here", scheme: "Bearer");
```

This will add an Authorization here -> `Authorization: Bearer my-api-key-here`

## API Key in the query string

Example:

```cs
builder.Services.AddHttpClient<MyCustomClient>()
                .AddApiKeyQueryAuthenticationHandler("my-api-key-here", queryParameterName: "key");
```

This will append to the query string of the request before the request is send out. E.g. `https://contoso.com/?key=my-api-key-here`

## Pre-Shared Key (PSK) in the `Authorization` header

This behaves similar to Microsoft's shared key authentication which you can also use in your own projects. Every requests ends up with a different authorization value and can be safer in some situations compared to using OAuth.

```cs
builder.Services.AddHttpClient($"{nameof(Worker)}4")
                .AddSharedKeyAuthenticationHandler("my-base-64-encoded-key", scheme: "Bearer");
```

This will add an Authorization here -> `Authorization: Bearer {base64-request-hash-will-be-set-here}`

## OAuth Client Credentials

This follows the OAuth 2.0 `client_credentials` flow and optionally caches access tokens using `IMemoryCache` or `IDistributedCache` for the duration it is valid.

Example:

```cs
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpClient<MyCustomClient>()
                .AddAuthenticationHandler(provider => new OAuthClientCredentialHandler()
                {
                    Scheme = "Bearer",

                    // set OAuth values to match your scenario
                    AuthenticationEndpoint = "https://oauth-1.contoso.com",
                    Resource = "https://api.contoso.com",
                    ClientId = "awesome-app-id",
                    ClientSecret = "super-secret",

                    Logger = provider.GetRequiredService<ILogger<Program>>(), // optional, useful for debugging

                    // caching can be disabled by setting either CacheKey or Cache to null
                    CacheKey = $"{nameof(MyCustomClient)}:auth-token",
                    // either IMemoryCache or IDistributedCache
                    Cache = new(provider.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>()),
                    //Cache = new(provider.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>()),
                });
```

## Azure B2B via OAuth Client Credentials

Works like [OAuth Client Credentials](#oauth-client-credentials) with a slight customization for Azure AD by setting the `AuthorizationEndpoint` based on the value supplied for `TenantId`.

Example:

```cs
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpClient<MyCustomClient>()
                .AddAuthenticationHandler(provider => new AzureAdB2BHandler
                {
                    Scheme = "Bearer",

                    // set OAuth values to match your scenario
                    TenantId = "00000000-0000-1111-0001-000000000000",
                    Resource = "https://api.contoso.com",
                    ClientId = "awesome-app-id",
                    ClientSecret = "super-secret",

                    Logger = provider.GetRequiredService<ILogger<Program>>(), // optional, useful for debugging

                    // caching can be disabled by setting either CacheKey or Cache to null
                    CacheKey = $"{nameof(MyCustomClient)}:auth-token",
                    // either IMemoryCache or IDistributedCache
                    Cache = new(provider.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>()),
                    //Cache = new(provider.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>()),
                });
```
