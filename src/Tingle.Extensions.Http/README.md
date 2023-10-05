# Tingle.Extensions.Http

This provides a lightweight abstraction around `HttpClient` which can be used to build custom client with response wrapping semantics.

The default serialization is done using JSON (`System.Text.Json`) but can be overridden to handle XML, SOAP, or any other formats including just changing the serializer to `Newtonsoft.Json`.

Below we'll go through some examples of how the `AbstractHttpApiClient`.

```cs
public class Account
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

public class MyServiceClient : AbstractApiClient<MyServiceClientOptions>
{
   public MyServiceClient(HttpClient client, IOptions<MyServiceClientOptions> optionsAccessor) : base(client, optionsAccessor){}

   public async Task<ResourceResponse<Account>> GetAccountAsync(string id, CancellationToken cancellationToken = default)
   {
       var uri = new Uri(BaseAddress, $"/v1/accounts/{id}");
       var request = new HttpRequestMessage(HttpMethod.Get, uri);
       return await SendAsync<Account>(request, cancellationToken);
   }
   // ...
}

public class MyServiceClientOptions : AbstractHttpApiClientOptions { }
```

Adding to services collection

In `Program.cs` add the following code snippet:

```cs
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpApiClient<MyServiceClient, MyServiceClientOptions>();

var host = builder.Build();
using var scope = host.Services.CreateScope();
var client = scope.ServiceProvider.GetRequiredService<MyServiceClient>();
var response = await client.GetAccountAsync("123456789");
response.EnsureSuccess(); // throws if not successful
response.EnsureHasResource(); // throws if the response body was empty (null resource)
```
