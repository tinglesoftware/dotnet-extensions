using System;
using System.Text.Json;
using Xunit;

namespace Tingle.Extensions.Json.Tests
{
    public class TimeSpanConverterTests
    {
        [Fact]
        public void TimeSpanConverter_Works()
        {
            var src_json = "{\"duration\":\"00:00:00.2880000\"}";
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }.AddConverterForTimeSpan();
            var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
            var dst_json = JsonSerializer.Serialize(model, options);
            Assert.Equal(src_json, dst_json);

            // not test with it null
            src_json = "{\"duration\":null}";
            model = JsonSerializer.Deserialize<TestModel>(src_json, options);
            dst_json = JsonSerializer.Serialize(model, options);
            Assert.Equal(src_json, dst_json);
        }

        class TestModel
        {
            public TimeSpan? Duration { get; set; }
        }
    }
}
