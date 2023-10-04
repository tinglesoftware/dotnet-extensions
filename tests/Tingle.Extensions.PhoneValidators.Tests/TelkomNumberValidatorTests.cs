using System.ComponentModel.DataAnnotations;
using Tingle.Extensions.PhoneValidators.Telkom;

namespace Tingle.Extensions.PhoneValidators.Tests;

public class TelkomNumberValidatorTests
{
    private readonly TelkomPhoneNumberValidator validator = new();

    [Theory]
    [InlineData("0772313817", true)]
    [InlineData("+254772313817", true)]
    [InlineData("254772313817", true)]
    [InlineData("772313817", true)]
    [InlineData("00772313817", false)]
    [InlineData("256772313817", false)]
    [InlineData("25477231381", false)]
    [InlineData("0722000000", false)]
    public void IsValid_Works(string phoneNumber, bool expected)
    {
        var actual = validator.IsValid(phoneNumber);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("0772313817", "254772313817")]
    [InlineData("+254772313817", "254772313817")]
    [InlineData("254772313817", "254772313817")]
    [InlineData("772313817", "254772313817")]
    [InlineData("256772313817", null)]
    [InlineData("0722000000", null)]
    public void ToMsisdn_Works(string phoneNumber, string expectedResponse)
    {
        var actualResult = validator.ToMsisdn(phoneNumber);
        Assert.Equal(expectedResponse, actualResult);
    }

    [Theory]
    [InlineData("254772313817", new string[] { "254772313817", "0772313817", "772313817" })]
    [InlineData("772313817", new string[] { "254772313817", "0772313817", "772313817" })]
    [InlineData("0722759406", new string[] { })]
    public void MakePossibleValues_Works(string phoneNumber, string[] expectedValues)
    {
        var actualValues = validator.MakePossibleValues(phoneNumber);
        Assert.Equal(expectedValues, actualValues);
    }

    [Theory]
    [InlineData("0772313817", "+254772313817")]
    [InlineData("+254772313817", "+254772313817")]
    [InlineData("254772313817", "+254772313817")]
    [InlineData("772313817", "+254772313817")]
    [InlineData("256772313817", null)]
    [InlineData("0722000000", null)]
    public void ToE164_Works(string phoneNumber, string expectedResponse)
    {
        var actualResult = validator.ToE164(phoneNumber);
        Assert.Equal(expectedResponse, actualResult);
    }

    [Theory]
    [InlineData("0772313817", true)]
    [InlineData("254772313817", true)]
    [InlineData("+254772313817", true)]
    [InlineData("772313817", true)]
    [InlineData("0722759406", false)]
    [InlineData("256772313817", false)]
    [InlineData("", true)]
    [InlineData(null, true)]
    [InlineData("A", false)]
    public void Attribute_Validation_Works_ForSingle(string testPhoneNumber, bool expected)
    {
        var obj = new TestModel1 { PhoneNumber = testPhoneNumber };
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
            Assert.Equal(nameof(TestModel1.PhoneNumber), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.Contains("must be a valid Telkom phone number.", val.ErrorMessage);
        }
    }

    [Theory]
    [InlineData("0772313817,254772313817", true)]
    [InlineData("+254772313817,772313817", true)]
    [InlineData("+254772313817,0722759406", false)]
    [InlineData("+254772313817,256772313817", false)]
    [InlineData("", false)]
    [InlineData(null, true)]
    [InlineData("A", false)]
    public void Attribute_Validation_Works_ForList(string testPhoneNumbers, bool expected)
    {
        var obj = new TestModel2 { PhoneNumbers = testPhoneNumbers?.Split(',') };
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
            Assert.Equal(nameof(TestModel2.PhoneNumbers), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.Contains("must be a valid Telkom phone number.", val.ErrorMessage);
        }
    }

    class TestModel1
    {
        [TelkomPhoneNumber]
        public string? PhoneNumber { get; set; }
    }

    class TestModel2
    {
        [TelkomPhoneNumber]
        public IList<string>? PhoneNumbers { get; set; }
    }
}
