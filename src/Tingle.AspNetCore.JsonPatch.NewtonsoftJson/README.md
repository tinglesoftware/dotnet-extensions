# Tingle.AspNetCore.JsonPatch.NewtonsoftJson

When performing patch operations, it is very likely that some updates to the model may try to modify or remove immutable types and thus there is a
need to ensure that this is not the case before committing the changes to the persistence layer such as a database.

A sample usage of this functionality is shown below:

```cs
public class TestClass
{
    [Key]
    public string Id { get; set; }

    [Range(18, 65)]
    public int Age { get; set; }
}
```

```cs
public class TestController : ControllerBase
{
    [HttpPost("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute, Required] string id, [FromBody] JsonPatchDocument<TestClass> model)
    {
        // Fetch the object that needs to be patched from your persistence layer e.g. database
        var test = await GetObjectFromDbAsync(id);

        // update the template
        model.ApplyToSafely(template, ModelState, new List<string> { "Id" });
        if (!ModelState.IsValid) return ValidationProblem();

        // validate the resulting model
        if (!TryValidateModel(test)) return ValidationProblem();

        // other code omitted for brevity
    }
}
```
