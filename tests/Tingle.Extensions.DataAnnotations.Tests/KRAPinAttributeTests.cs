using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Tingle.Extensions.DataAnnotations.Tests;

public class KRAPinAttributeTests
{
    [Theory]
    [InlineData("A123456789P", true)]
    [InlineData("AP123456789", false)]
    [InlineData("a123456789p", true)]
    [InlineData("", true)]
    [InlineData(null, true)]
    [InlineData("A", false)]
    public void KRAPin_Validation_Works(string testPin, bool expected)
    {
        var obj = new TestModel { KRAPinNumber = testPin };
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
            Assert.Equal(nameof(TestModel.KRAPinNumber), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.Contains("must match the regular expression", val.ErrorMessage);
        }
    }

    class TestModel
    {
        [KRAPin]
        public string? KRAPinNumber { get; set; }
    }
}
