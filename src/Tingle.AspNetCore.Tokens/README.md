# Tingle.AspNetCore.Tokens

This library adds support for generation of continuation tokens in ASP.NET Core with optional expiry. This is particularly useful for pagination, user invite tokens, expiring operation tokens, etc.
The functionality is availed through the `ContinuationToken<T>` and `TimedContinuationToken<T>` types. These are backed using the [DataProtection sub-system in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-7.0).

See [sample](./samples/TokensSample).

First step is to register the required services.

```cs
var builder = WebApplication.CreateBuilder(args);

// see https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-7.0
builder.Services.AddDataProtection();

builder.Services.AddControllers()
                .AddTokens();

var app = builder.Build();

app.MapControllers();

app.Run();
```

## Pagination

Pagination is best served using `ContinuationToken<T>` with `DateTimeOffset` or an incrementing identifier in your database.

```cs
[ApiController]
[Route("/books")]
[ProducesErrorResponseType(typeof(ValidationProblemDetails))]
public class BooksController : ControllerBase
{
    static readonly List<Book> Books = new();

    [HttpGet]
    public IActionResult List([FromQuery] ContinuationToken<DateTimeOffset>? token)
    {
        var last = token?.GetValue();
        var query = last is not null ? Books.Where(b => b.Created > last) : Books;
        query = query.Take(10); // limit the number of items to pull from the database

        var books = query.ToList(); // pull from the database
        last = books.Any() ? books.Last().Created : null;

        if (last is not null)
        {
            var ct = new ContinuationToken<DateTimeOffset>(last.Value);
            return this.Ok(books, ct);
        }

        return Ok(books);
    }
}
```

## User invitation and account transfer tokens

User invitation tokens that do not expire are best served using `ContinuationToken<T>` with a custom model inside that can carry extra information with it. The same can be done for other scenarios such as account transfer and email validation.

```cs
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
```

## Expiration

For certain scenarios, expiration is desired. Such as confirmation of monetary disbursement. In such, you should use `TimedContinuationToken<T>` when receiving the token and pass an absolute/relative expiration when protecting the data.

```cs
[ApiController]
[Route("/disbursement")]
[ProducesErrorResponseType(typeof(ValidationProblemDetails))]
public class DisbursementController : ControllerBase
{
    private readonly ITokenProtector<DisbursementToken> tokenProtector;

    public DisbursementController(ITokenProtector<DisbursementToken> tokenProtector)
    {
        this.tokenProtector = tokenProtector ?? throw new ArgumentNullException(nameof(tokenProtector));
    }

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
```
