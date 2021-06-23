using System.Text.Json.Serialization;

namespace Tingle.Extensions.JsonPatch.Tests
{
    public class JsonPropertyDTO
    {
        [JsonPropertyName("AnotherName")]
        public string? Name { get; set; }
    }


    public class JsonPropertyWithAnotherNameDTO
    {
        public string? AnotherName { get; set; }
    }
}
