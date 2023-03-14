using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Tingle.AspNetCore.Tokens.Protection;
using TokensSample.Models;

namespace TokensSample.Controllers;

[ApiController]
[Route("/users")]
[ProducesErrorResponseType(typeof(ValidationProblemDetails))]
public class UsersController : ControllerBase
{
    private readonly ITokenProtector<InvitationLinkToken> tokenProtector;

    public UsersController(ITokenProtector<InvitationLinkToken> tokenProtector)
    {
        this.tokenProtector = tokenProtector ?? throw new ArgumentNullException(nameof(tokenProtector));
    }

    [HttpPost]
    public IActionResult Invite([FromBody] UserCreateModel model)
    {
        // create the invitation in the database
        var invitationId = Guid.NewGuid().ToString();

        // send invite email
        var token = tokenProtector.Protect(new InvitationLinkToken { Id = invitationId, });

        // send email using the token

        return Ok();
    }

    [HttpPost("{id}/resend")]
    public IActionResult Resend([FromRoute, Required] string id)
    {
        // find the invitation from the database
        var invitationId = Guid.NewGuid().ToString();

        // resend invite email
        var token = tokenProtector.Protect(new InvitationLinkToken { Id = invitationId, });

        // send email using the token

        return Ok();
    }
}
