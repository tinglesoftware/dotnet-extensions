using System;
using System.Text.Json;
using Xunit;

namespace Tingle.Extensions.Json.Tests
{
    public class VersionConverterTests
    {
        [Fact]
        public void VersionConverter_Works()
        {
            var src_json = "{\"deployed\":\"1.13.4\"}";
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }.AddConverterForVersion();
            var model = JsonSerializer.Deserialize<TestModel>(src_json, options);
            var dst_json = JsonSerializer.Serialize(model, options);
            Assert.Equal(src_json, dst_json);

            // not test with it null
            src_json = "{\"deployed\":null}";
            model = JsonSerializer.Deserialize<TestModel>(src_json, options);
            dst_json = JsonSerializer.Serialize(model, options);
            Assert.Equal(src_json, dst_json);
        }

        class TestModel
        {
            public Version Deployed { get; set; }
        }
    }
}
