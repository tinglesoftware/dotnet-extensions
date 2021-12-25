using System.ComponentModel.DataAnnotations;

namespace Tingle.Extensions.DataAnnotations.Tests;

public class PrefixAttributeTests
{
    [Theory]
    [InlineData("test.com", true)]
    [InlineData("test-com", true)]
    [InlineData("test the chicken", true)]
    [InlineData("who cares anyway", false)]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData("A", false)]
    public void Prefix_Validation_Works(string testPin, bool expected)
    {
        var obj = new TestModel { SomeValue = testPin };
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
            Assert.EndsWith("must start with 'test'.", val.ErrorMessage);
        }
    }

    class TestModel
    {
        [Prefix("test")]
        public string? SomeValue { get; set; }
    }
}
