using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TokensSample.Models;

namespace TokensSample.Controllers;

[ApiController]
[Route("/invites")]
[ProducesErrorResponseType(typeof(ValidationProblemDetails))]
public class InvitesController : ControllerBase
{
    [HttpPost("accept")]
    public IActionResult Accept([FromQuery, Required] ContinuationToken<InvitationLinkToken> token)
    {
        // ensure the invite exists
        var model = token.GetValue();
        var invitationId = model.Id;
        // do the database magic here

        return Ok();
    }

    [HttpPost("reject")]
    public IActionResult Reject([FromQuery, Required] ContinuationToken<InvitationLinkToken> token)
    {
        // ensure the invite exists
        var model = token.GetValue();
        var invitationId = model.Id;
        // do the database magic here

        return Ok();
    }
}
