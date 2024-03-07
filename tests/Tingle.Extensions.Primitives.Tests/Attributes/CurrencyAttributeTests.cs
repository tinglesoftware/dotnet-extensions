using System.ComponentModel.DataAnnotations;

namespace Tingle.Extensions.Primitives.Tests.Attributes;

public class CurrencyAttributeTests
{
    [Theory]
    [InlineData("KES", true)]
    [InlineData("kes", true)]
    [InlineData("UGX", true)]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("A", false)]
    public void Currency_Validation_Works_ForSingle(string? testValue, bool expected)
    {
        var obj = new TestModel1 { Currency = testValue };
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
            Assert.Equal(nameof(TestModel1.Currency), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.EndsWith("must be a valid Currency.", val.ErrorMessage);
        }
    }

    [Theory]
    [InlineData("KES,UGX", true)]
    [InlineData("kes,KES", true)]
    [InlineData("UGX,TZS", true)]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData("A", false)]
    public void Currency_Validation_Works(string? testValues, bool expected)
    {
        var obj = new TestModel2 { Currencies = testValues?.Split(','), };
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
            Assert.Equal(nameof(TestModel2.Currencies), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.EndsWith("must be a valid Currency.", val.ErrorMessage);
        }
    }

    class TestModel1
    {
        [Currency]
        public string? Currency { get; set; }
    }

    class TestModel2
    {
        [Currency]
        public IList<string>? Currencies { get; set; }
    }
}
