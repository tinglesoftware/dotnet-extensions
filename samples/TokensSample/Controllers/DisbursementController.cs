using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Tingle.AspNetCore.Tokens.Protection;
using TokensSample.Models;

namespace TokensSample.Controllers;

[ApiController]
[Route("/disbursement")]
[ProducesErrorResponseType(typeof(ValidationProblemDetails))]
public class DisbursementController(ITokenProtector<DisbursementToken> tokenProtector) : ControllerBase
{
    [HttpPost("initiate")]
    public IActionResult Initiate([FromBody] DisbursementInitiateModel model)
    {
        // do lots of time consuming checks here such as limit per day, forex rules, etc.

        // do light weight checks e.g. enough funds in the account

        // generate token and send it back in the response
        var ttl = TimeSpan.FromMinutes(1);
        var token = tokenProtector.Protect(new DisbursementToken { Amount = model.Amount, Iban = model.Iban, }, ttl);

        // to confirm, the user will call /confirmed?token={token}
        return Ok(new DisbursementResponseModel { Token = token, });
    }

    [HttpPost("confirmed")]
    public IActionResult Confirmed([FromQuery, Required] TimedContinuationToken<DisbursementToken> token)
    {
        // do light weight checks that may have changed e.g. enough funds in the account

        // do the disbursement here (if expired, we never get here)
        var model = token.GetValue();
        var amount = model.Amount;
        var iban = model.Iban;

        return Ok();
    }
}
