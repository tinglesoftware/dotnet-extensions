# Tingle.AspNetCore.DataProtection.MongoDB

Web applications often need to store security-sensitive data. Windows provides DPAPI for desktop applications, but this is unsuitable for web applications especially when running in a Docker container. The ASP.NET Core data protection stack provide a simple, easy to use cryptographic API a developer can use to protect data, including key management and rotation.

Microsoft has already provided ways to persist data protection keys to [various storage systems](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-10.0).

This package provides the functionality to persist the data protection keys to MongoDB.

## Using Dependency Injection

```cs
public void ConfigureServices(IServicesCollection services)
{
    services.AddSingleton<IMongoClient>(p => new MongoClient("mongodb://localhost:27017/my-database"));
    services.AddDataProtection()
            .PersistKeysToMongo();
}
```

### Sample Usage

```cs
[Route("api/v1/[controller]")]
public class DummyController : ControllerBase
{
    IDataProtector _protector;
    public DummyController(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector(nameof(DummyController));
    }

    [HttpGet]
    public Task<IActionResult> TestAsync([FromQuery] string value)
    {
        string protectedPayload = _protector.Protect(value);
        string unprotectedPayload = _protector.Unprotect(protectedPayload);

        // unprotectedPayload and value should be the same

        return Ok();
    }
}
```

The `PersistKeysToMongo` can be configured to store keys in databases and collections of your choice. By default, data protection keys will be stored in a database and collection named `DataProtection` and `DataProtection-Keys` respectively.
