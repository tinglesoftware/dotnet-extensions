using System.Text.Json.Serialization;

namespace Tingle.Extensions.Http.Tests.Serialization;

public class ToDoActivity
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    public int TaskNum { get; set; }
    public double Cost { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
}
