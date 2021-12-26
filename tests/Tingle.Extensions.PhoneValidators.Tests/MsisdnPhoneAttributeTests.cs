using System.ComponentModel.DataAnnotations;

namespace Tingle.Extensions.PhoneValidators.Tests;

public class MsisdnPhoneAttributeTests
{
    [Theory]
    [InlineData("254722000000", true)]
    [InlineData("254733000000", true)]
    [InlineData("254203893501", true)]
    [InlineData("+44 117 000 0000", false)]
    [InlineData("+441170000000", false)]
    [InlineData("+254722000000", false)]
    [InlineData("+254 722 000000", false)]
    [InlineData("+254 72 200 0000", false)]
    [InlineData("0722000000", false)]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("A", false)]
    public void MsisdnPhone_Validation_Works_WithoutRegion(string testPin, bool expected)
    {
        var obj = new TestModel1 { SomePhoneNumber = testPin };
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
            Assert.Equal(nameof(TestModel1.SomePhoneNumber), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.EndsWith("must be a valid MSISDN phone number.", val.ErrorMessage);
        }
    }

    class TestModel1
    {
        [MsisdnPhone]
        public string? SomePhoneNumber { get; set; }
    }
}
