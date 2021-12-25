using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Tingle.Extensions.DataAnnotations.Tests;

public class Base64AttributeTests
{
    [Theory]
    [InlineData("oHKa/1iJ0kONZfhtCBDKcQ==", true)]
    [InlineData("oHKa/1iJ0kONZfhtCBDKcQ=", false)]
    [InlineData("oHKa1iJ0kONZfhtCBDKcQ==", false)]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("A", false)]
    public void Base64_Validation_Works(string testKey, bool expected)
    {
        var obj = new TestModel { SomeKey = testKey };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var actual = Validator.TryValidateObject(obj, context, results, true);
        Assert.Equal(expected, actual);

        // if expected it to pass, the results should be empty
        if (expected) Assert.Empty(results);
        else
        {
            var val = Assert.Single(results);
            var memeberName = Assert.Single(val.MemberNames);
            Assert.Equal(nameof(TestModel.SomeKey), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.EndsWith("must be a valid base64 string.", val.ErrorMessage);
        }
    }

    class TestModel
    {
        [Base64]
        public string? SomeKey { get; set; }
    }
}
