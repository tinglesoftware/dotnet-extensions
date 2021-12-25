using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Tingle.Extensions.PhoneValidators.Tests;

public class E164PhoneAttributeTests
{
    [Theory]
    [InlineData("+44 117 496 0123", true)]
    [InlineData("+441174960123", true)]
    [InlineData("+254728837078", true)]
    [InlineData("+254 728 837078", true)]
    [InlineData("+254 72 883 7078", true)]
    [InlineData("0728837078", false)]
    [InlineData("728837078", false)]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("A", false)]
    public void E164Phone_Validation_Works_WithoutRegion(string testPin, bool expected)
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
            Assert.EndsWith("must be a valid E.164 phone number.", val.ErrorMessage);
        }
    }

    [Theory]
    [InlineData("+44 117 496 0123", true)]
    [InlineData("+441174960123", true)]
    [InlineData("+254728837078", true)]
    [InlineData("+254 728 837078", true)]
    [InlineData("+254 72 883 7078", true)]
    [InlineData("0728837078", true)]
    [InlineData("728837078", true)]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("A", false)]
    public void E164Phone_Validation_Works_WithRegion(string testPin, bool expected)
    {
        var obj = new TestModel2 { SomePhoneNumber = testPin };
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
            Assert.Equal(nameof(TestModel2.SomePhoneNumber), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.EndsWith("must be a valid E.164 phone number.", val.ErrorMessage);
        }
    }

    class TestModel1
    {
        [E164Phone]
        public string? SomePhoneNumber { get; set; }
    }

    class TestModel2
    {
        [E164Phone("KE")]
        public string? SomePhoneNumber { get; set; }
    }
}
