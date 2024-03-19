using Microsoft.AspNetCore.Mvc;

#pragma warning disable IDE0060 // Remove unused parameter

namespace ApplicationInsightsSample.Controllers;

[Route("api/[controller]")]
[ApiController]
[TrackProblems]
public class ValuesController : ControllerBase
{
    // GET: api/Values
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET: api/Values/5
    [HttpGet("{id}", Name = "Get")]
    public string Get(int id)
    {
        return "value";
    }

    // POST: api/Values
    [HttpPost]
    public IActionResult Post([FromBody] string value)
    {
        return Problem(title: "test_error_code", detail: "This is a test error", statusCode: 400);
    }
}
