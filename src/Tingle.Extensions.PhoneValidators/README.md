# Tingle.Extensions.PhoneValidators

This library is used to validate if a phone number is valid. It can also be used to convert Kenyan phone numbers between E.164, local, and MSISDN formats.

## Using attributes

The .NET framework has validation inbuilt by decorating attributes on your members. This library supports that workflow.

```csharp
public class SetPhoneNumberModel
{
    [Required]
    public string UserId { get; set; }

    [Required]
    [E164Phone] // ensures value is in E.164 format
    public string Primary { get; set; }

    [E164Phone] // ensures all values are in E.164 format
    [DataType(DataType.PhoneNumber)] // useful for generating docs in OpenAPI
    public IList<string> Alternatives { get; set; }
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
|MsisdnPhoneAttribute|Validates that the value is formatted as per the [MSISDN format](<https://en.wikipedia.org/wiki/MSISDN#:~:text=MSISDN%20(pronounced%20as%20%2F'em,a%20mobile%20or%20cellular%20phone.)>).|
|SafaricomPhoneAttribute|Validates that the value is valid for Safaricom Kenya phone numbers|
|AirtelPhoneAttribute|Validates that the value is valid for Airtel Kenya phone numbers|
|TelkomPhoneAttribute|Validates that the value is valid for Telkom Kenya phone numbers|

## Using Dependency Injection

First add to the services collection before they can be resolved.

### Sample Usage (Safaricom validator)

```csharp
[Route("api/v1/[controller]")]
public class DummyController : ControllerBase
{
    private static readonly SafaricomPhoneNumberValidator phoneNumberValidator = new();

    [HttpGet]
    public Task<IActionResult> TestAsync([FromQuery] string phoneNumber)
    {
        var result = phoneNumberValidator.IsValid(phoneNumber);
        if (!result) return BadRequest("Invalid Phone number!");

        // Convert to MSISDN format
        var msisdn = phoneNumberValidators.ToMsisdn(phoneNumber);
        return Ok(msisdn);
    }
}
```
