# Tingle.Extensions.PhoneValidators

This library is used to validate if a phone number is valid. Currently, only Kenyan phone numbers can be validated. It can also be used to convert phone numbers from E.164 format or local format to MSISDN format.
Where needed simply inject `IEnumerable<IPhoneNumberValidator>` to get all validators available.
However, it is advised that you inject the implementation required, for example inject `SafaricomPhoneValidator` to validate Safaricom phone numbers.

## Adding To Services Collection

```csharp
public void ConfigureServices(IServicesCollection services) {
    services.AddSafaricomPhoneNumberValidator(); // Safaricom
    services.AddAirtelPhoneNumberValidator(); // Airetl
}
```

## Sample Usage (all validators)

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

## Sample Usage (Safaricom validators)

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
