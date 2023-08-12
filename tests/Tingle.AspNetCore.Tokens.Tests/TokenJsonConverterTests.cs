using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Tingle.AspNetCore.Tokens.Tests;

public class TokenJsonConverterTests
{
    [Fact]
    public void Converter_Works()
    {
        var src_json = @"{""token1"":""YyBpPyhOgEGAKQAkqvNFMg=="",""token2"":""GkTK64SntEWRw28wsnYQ5g==""}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var tdm = new TestDataClass { };
        var model = new TestModel
        {
            Token1 = new ContinuationToken<TestDataClass>(tdm, "YyBpPyhOgEGAKQAkqvNFMg=="),
            Token2 = new TimedContinuationToken<TestDataClass>(tdm, "GkTK64SntEWRw28wsnYQ5g==", DateTimeOffset.UtcNow)
        };
        var dst_json = JsonSerializer.Serialize(model, options);
        Assert.Equal(src_json, dst_json);
    }

    [Fact]
    public void Converter_Deserialization_Throws_NotSupportedException()
    {
        var src_json = @"{""token1"":""YyBpPyhOgEGAKQAkqvNFMg=="",""token2"":""YqlhEFTPL0qFHJmQoyX40w=="",""token3"":""GkTK64SntEWRw28wsnYQ5g==""}";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var ex = Assert.Throws<NotSupportedException>(() => JsonSerializer.Deserialize<TestModel>(src_json, options));
        Assert.StartsWith("Tokens cannot be deserialized because they are protected (obscure) data."
                        + " Use model binding instead.", ex.Message);
    }

    class TestModel
    {
        public ContinuationToken<TestDataClass>? Token1 { get; set; }
        public TimedContinuationToken<TestDataClass>? Token2 { get; set; }
    }
}
