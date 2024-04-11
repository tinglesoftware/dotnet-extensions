# Tingle.AspNetCore.JsonPatch

The primary goal of this library is to provide functionalities to perform [Json Patch](https://datatracker.ietf.org/doc/html/rfc6902) and [JSON Merge Patch](https://datatracker.ietf.org/doc/html/rfc7386) operations on documents using `System.Text.Json` library.

## Json Patch

JSON patch support is quite similar to Microsoft's equivalent for [Newtonsoft.Json](https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-8.0). The only difference is how you configure the input and output formatters for all JSON content. This should be done as shown below:

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
                .AddJsonPatch() // would be .AddNewtonsoftJson() in the Newtonsoft.Json equivalent
                .AddJsonOptions(options => {}); // Add and configure JSON formatters

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

## Json Merge Patch

 The library helps to deserialize HTTP requests' and responses' JSON body content for merge patch operation. If the merge patch request contains members that appear as null on the target object, those members are added. If the target object contains the member, the value is replaced. Members with null values in the merge patch requests, are removed from the target object (set to null or default).

For example, the following JSON documents represent a resource, a JSON Merge Patch document for the resource, and the result of applying the Patch operations.

### Resource Example

```json
{
    "id": "1",
    "name": null,
    "phone": "+254722000000",
    "country": "ken"
}
```

### JSON Merge Patch Example

```json
{
    "name": "Fabrikam",
    "phone": "+254722000001",
    "country": null
}
```

### Resource after patch

```json
{
    "id": "1",
    "name": "Fabrikam",
    "phone": "+254722000001",
    "country": null
}
```

`id` property remains unchanged as it is not part of the merge patch request.

### JSON Merge Patch in ASP.NET Core

Define a `Customer` model:

```cs
class Customer
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Country { get; set; }
}
```

Add the following logic in the `Program.cs` file. This same logic can be added to `Startup.cs`.

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
                .AddJsonPatch()
                .AddJsonOptions(options => {}); // Add and configure JSON formatters

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

Use in your controller

```cs
[HttpPatch]
public void Patch([FromBody] JsonMergePatchDocument<Customer> patch)
{
    ...
    patch.ApplyTo(customer, ModelState);
    ...
}
```

In a real app, the code would retrieve the data from a store such as a database and update the database after applying the patch.

The preceding action method example calls an overload of `ApplyTo` that takes model state as one of its parameters. With this option, you can get error messages in responses.
