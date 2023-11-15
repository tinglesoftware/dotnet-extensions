using Microsoft.AspNetCore.Mvc;
using TokensSample.Models;

namespace TokensSample.Controllers;

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
        last = books.Count != 0 ? books.Last().Created : null;

        if (last is not null)
        {
            var ct = new ContinuationToken<DateTimeOffset>(last.Value);
            return this.Ok(books, ct);
        }

        return Ok(books);
    }
}
