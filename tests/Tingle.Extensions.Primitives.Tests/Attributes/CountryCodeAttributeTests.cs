using System.ComponentModel.DataAnnotations;

namespace Tingle.Extensions.Primitives.Tests.Attributes;

public class CountryCodeAttributeTests
{
    [Theory]
    [InlineData("KE", true)]
    [InlineData("KEN", true)]
    [InlineData("UGA", true)]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("A", false)]
    public void CountryCode_Validation_Works_ForSingle(string? testCode, bool expected)
    {
        var obj = new TestModel1 { SomeCode = testCode };
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
            Assert.Equal(nameof(TestModel1.SomeCode), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.EndsWith("must be a valid Country Code.", val.ErrorMessage);
        }
    }

    [Theory]
    [InlineData("KE,KEN", true)]
    [InlineData("KEN,UGA", true)]
    [InlineData("UGA", true)]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData("A", false)]
    public void CountryCode_Validation_Works_ForList(string? testCodes, bool expected)
    {
        var obj = new TestModel2 { SomeCodes = testCodes?.Split(','), };
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
            Assert.Equal(nameof(TestModel2.SomeCodes), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.EndsWith("must be a valid Country Code.", val.ErrorMessage);
        }
    }

    class TestModel1
    {
        [CountryCode]
        public string? SomeCode { get; set; }
    }

    class TestModel2
    {
        [CountryCode]
        public IList<string>? SomeCodes { get; set; }
    }
}
