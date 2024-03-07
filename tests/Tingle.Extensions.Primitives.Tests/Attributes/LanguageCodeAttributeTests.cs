using System.ComponentModel.DataAnnotations;

namespace Tingle.Extensions.Primitives.Tests.Attributes;

public class LanguageCodeAttributeTests
{
    [Theory]
    [InlineData("eng", true)]
    [InlineData("swa", true)]
    [InlineData("sw", true)]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("A", false)]
    public void LanguageCode_Validation_Works_ForSingle(string? testCode, bool expected)
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
            Assert.EndsWith("must be a valid Language.", val.ErrorMessage);
        }
    }

    [Theory]
    [InlineData("eng,swa", true)]
    [InlineData("swa,sw", true)]
    [InlineData("sw", true)]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData("A", false)]
    public void LanguageCode_Validation_Works(string? testCodes, bool expected)
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
            Assert.EndsWith("must be a valid Language.", val.ErrorMessage);
        }
    }

    class TestModel1
    {
        [LanguageCode]
        public string? SomeCode { get; set; }
    }

    class TestModel2
    {
        [LanguageCode]
        public IList<string>? SomeCodes { get; set; }
    }
}
