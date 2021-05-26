# Tingle.Extensions.PhoneValidators

This library is used to validate if a phone number is valid. Currently, only Kenyan phone numbers can be validated. It can also be used to convert phone numbers from E.164 format or local format to MSISDN format.
Where needed simply inject `IEnumerable<IPhoneNumberValidator>` to get all validators available.
However, it is advised that you inject the implementation required, for example inject `SafaricomPhoneValidator` to validate Safaricom phone numbers.

## Using attributes

The .NET framework has validation inbuilt by decorating attributes on your members. This library supports that workflow.

```csharp
public class SetPhoneNumberModel
{
    [Required]
    public string UserId { get; set; }

    [Required]
    [Phone] // useful for generating docs in Swagger
    [E164Phone] // ensures a phone in E.164 format
    public string Phone { get; set; }
}
```

Using this in AspNetCore is easy since model validation is inbuilt and enabled by default.

```csharp
[Route("api/v1/[controller]")]
public class DummyController : ControllerBase
{
    [HttpGet]
    public Task<IActionResult> TestAsync([FromBody, Required] SetPhoneNumberModel model)
    {
        // If we get here, the model was already validated
        // Otherwise, problem details were returned.

        return Ok();
    }
}
```

The available attributes include:
|Attribute Name|Description|
|--|--|
|E164PhoneAttribute|Validates that the value is formatted as per the [E.164 standard](https://en.wikipedia.org/wiki/E.164).|
|MsisdnPhoneAttribute|Validates that the value is formatted as per the [MSISDN format](https://en.wikipedia.org/wiki/MSISDN#:~:text=MSISDN%20(pronounced%20as%20%2F'em,a%20mobile%20or%20cellular%20phone.).|
|SafaricomPhoneAttribute|Validates that the value is valid for Safaricom Kenya phone numbers|
|AirtelPhoneAttribute|Validates that the value is valid for Airtel Kenya phone numbers|
|TelkomPhoneAttribute|Validates that the value is valid for Telkom Kenya phone numbers|

## Using Dependency Injection

First add to the services collection before they can be resolved.

```csharp
public void ConfigureServices(IServicesCollection services)
{
    services.AddSafaricomPhoneNumberValidator(); // Safaricom
    services.AddAirtelPhoneNumberValidator(); // Airetl
}
```

### Sample Usage (all possible validators)

```csharp
[Route("api/v1/[controller]")]
public class DummyController : ControllerBase
{
    private readonly IEnumerable<IPhoneNumberValidator> phoneNumberValidators;
    public DummyController(IEnumerable<IPhoneNumberValidator> phoneNumberValidators)
    {
        this.phoneNumberValidators = phoneNumberValidators ?? throw new ArgumentNullException(nameof(phoneNumberValidators));
    }

    [HttpGet]
    public Task<IActionResult> TestAsync([FromQuery] string phoneNumber)
    {
        var result = phoneNumberValidators.Any(pv => pv.IsValid(phoneNumber));
        if (!result) return BadRequest("Invalid Phone number!");

        //Convert to MSISDN format
        var msisdn = phoneNumberValidators.Any(pv => pv.ToMsisdn(phoneNumber));

        return Ok(msisdn);
    }
}
```

### Sample Usage (Safaricom validator)

```csharp
[Route("api/v1/[controller]")]
public class DummyController : ControllerBase
{
    private readonly SafaricomPhoneNumberValidator phoneNumberValidator;
    public DummyController(SafaricomPhoneNumberValidator phoneNumberValidator)
    {
        this.phoneNumberValidator = phoneNumberValidator ?? throw new ArgumentNullException(nameof(phoneNumberValidator));
    }

    [HttpGet]
    public Task<IActionResult> TestAsync([FromQuery] string phoneNumber)
    {
        var result = phoneNumberValidator.IsValid(phoneNumber);
        if (!result) return BadRequest("Invalid Phone number!");

        //Convert to MSISDN format
        var msisdn = phoneNumberValidators.ToMsisdn(phoneNumber);
        return Ok(msisdn);
    }
}
```
