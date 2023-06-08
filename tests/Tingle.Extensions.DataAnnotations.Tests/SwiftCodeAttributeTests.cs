using System.ComponentModel.DataAnnotations;

namespace Tingle.Extensions.DataAnnotations.Tests;

public class SwiftCodeAttributeTests
{
    [Theory]
    [InlineData("KCBLKENXXXX", true)]
    [InlineData("IMBLKENAXXX", true)]
    [InlineData("PMFAUS66HKG", true)]
    [InlineData("IMB2KENAXXX", false)]
    [InlineData("kcblkenxxxx", true)]
    [InlineData("", true)]
    [InlineData(null, true)]
    [InlineData("A", false)]
    public void SwiftCode_Validation_Works(string testPin, bool expected)
    {
        var obj = new TestModel(testPin);
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
            Assert.Equal(nameof(TestModel.TransferTo), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.Contains("must match the regular expression", val.ErrorMessage);
        }
    }

    record TestModel([property: SwiftCode] string? TransferTo);
}
