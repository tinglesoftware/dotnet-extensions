using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Tingle.Extensions.DataAnnotations.Tests;

public class SuffixAttributeTests
{
    [Theory]
    [InlineData("the worLd", true)]
    [InlineData("I dont belong to this worLd", true)]
    [InlineData("who cares about the worLd anyway", false)]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData("A", false)]
    public void Suffix_Validation_Works(string testValue, bool expected)
    {
        var obj = new TestModel { SomeValue = testValue };
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
            Assert.Equal(nameof(TestModel.SomeValue), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.EndsWith("must end with 'worLd'.", val.ErrorMessage);
        }
    }

    class TestModel
    {
        [Suffix("worLd")]
        public string? SomeValue { get; set; }
    }
}
