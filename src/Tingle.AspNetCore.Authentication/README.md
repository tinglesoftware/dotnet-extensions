# Tingle.AspNetCore.Authentication

## SharedKey Authentication

This authentication logic mirrors the one for most Azure services such as Azure storage. Every request provides a different authentication value signed/encrypted with a key shared prioir.
This can often provide better security as compared to bearer tokens in OAuth/OpenId when introspection is not done on every request or when introspection canbe very expensive.

Most common usage scenario is machine-to-machine authentication where the OAuth flow is expensive (introspection and renewing tokens).

A token is generated based on the HTTP method, path, time (if you want to enforce time constraints), content length and content type, then hashed using a pre-shared key (such as the primary or secondary key).
The hashing algorithm used to generate the token is `HMACSHA256`.

A sample request to a resource that does shared key authentication:
`curl -H "Authorization: SharedKey 1/fFAGRNJru1FTz70BzhT3Zg"  https://api.contoso.com/v1/cars?type=tesla`

Add the following logic to Program.cs file

```cs
// Configure authentication
builder.Services.AddAuthentication()
                .AddSharedKey(options =>
                {
                   options.ValidationParameters.KeysResolver = (ctx) =>
                   {
                      var key1 = "my_primary_key_here";
                      var key2 = "my_secondary_key_here";
                      return Task.FromResult((IEnumerable<string>)new[] { key1, key2, /* add as many keys as you wish */ });
                   };
                })

builder.Service.AddAuthorization(options =>
{
   options.AddPolicy("MyTenantAuthorizationPolicy", policy =>
   {
      policy.AddAuthenticationSchemes(SharedKeyDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser();
   });
}

var app = builder.Build();

// Add auth middleware
app.UseAuthentication();
app.UseAuthorization();
```

`options` are of type `SharedKeyOptions`. They contain authentication options used by `SharedKeyTokenHandler`. You can modify some of these configurations to suit your own application needs.

Now, we can use this functionality to authorize access to a controller as shown below:

```cs
[Authorize("MyTenantAuthorizationPolicy")]
public class DummyController : ControllerBase {}
```

## Pass Through Authentication

This authentication scheme results in a successful authentication result by default. You can then use this authentication result to perform authorization to access various endpoints depending on their authorization requirements e.g by restricting the range of IP addresses.

Add the following logic in Program.cs file:

```cs
builder.Services.AddAuthentication()
                .AddPassThrough("my_scheme", null);

builder.Services.AddAuthorization(options =>
{
   options.AddPolicy("myProcessAuthPolicy", policy =>
   {
      policy.RequireAuthenticatedUser()
            .AddAuthenticationSchemes("my_scheme");
   });
});

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
   ...
   // Add auth middleware
   app.UseAuthentication();
   app.UseAuthorization();
   ....
}
```
