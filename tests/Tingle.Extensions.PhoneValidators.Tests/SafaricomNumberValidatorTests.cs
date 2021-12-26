using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using Tingle.Extensions.PhoneValidators.Safaricom;

namespace Tingle.Extensions.PhoneValidators.Tests;

public class SafaricomNumberValidatorTests
{
    private readonly SafaricomPhoneNumberValidator validator;

    public SafaricomNumberValidatorTests()
    {
        var services = new ServiceCollection()
            .AddSafaricomPhoneNumberValidator();

        var provider = services.BuildServiceProvider();

        validator = provider.GetRequiredService<SafaricomPhoneNumberValidator>();
    }

    [Theory]
    [InlineData("0722000000", true)]
    [InlineData("+254722000000", true)]
    [InlineData("+254110000000", true)]
    [InlineData("254722000000", true)]
    [InlineData("254719666721", true)]
    [InlineData("254791135160", true)]
    [InlineData("722000000", true)]
    [InlineData("798126409", true)]
    [InlineData("00722000000", false)]
    [InlineData("256722000000", false)]
    [InlineData("25472883707", false)]
    [InlineData("254709677872", true)]
    [InlineData("254111677872", true)]
    [InlineData("0733000000", false)]
    [InlineData("0757000000", true)]
    [InlineData("0755000000", false)]
    [InlineData("0769123123", true)]
    [InlineData("0768123123", true)]
    [InlineData("0111000000", true)]
    [InlineData("0110000000", true)]
    [InlineData("0115278316", true)]
    public void IsValid_Works(string phoneNumber, bool expected)
    {
        var actual = validator.IsValid(phoneNumber);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("0722759406", "254722759406")]
    [InlineData("0722000000", "254722000000")]
    [InlineData("+254722000000", "254722000000")]
    [InlineData("254722000000", "254722000000")]
    [InlineData("722000000", "254722000000")]
    [InlineData("256722000000", null)]
    [InlineData("0733000000", null)]
    [InlineData("798126409", "254798126409")]
    [InlineData("111126409", "254111126409")]
    [InlineData("110126409", "254110126409")]
    [InlineData("115278316", "254115278316")]
    public void ToMsisdn_Works(string phoneNumber, string expectedResponse)
    {
        var actualResult = validator.ToMsisdn(phoneNumber);
        Assert.Equal(expectedResponse, actualResult);
    }

    [Theory]
    [InlineData("0722759406", new string[] { "254722759406", "0722759406", "722759406" })]
    [InlineData("254722000000", new string[] { "254722000000", "0722000000", "722000000" })]
    [InlineData("722000000", new string[] { "254722000000", "0722000000", "722000000" })]
    [InlineData("798126409", new string[] { "254798126409", "0798126409", "798126409" })]
    [InlineData("111126409", new string[] { "254111126409", "0111126409", "111126409" })]
    [InlineData("110126409", new string[] { "254110126409", "0110126409", "110126409" })]
    [InlineData("115278316", new string[] { "254115278316", "0115278316", "115278316" })]
    [InlineData("0733000000", new string[] { })]
    public void MakePossibleValues_Works(string phoneNumber, string[] expectedValues)
    {
        var actualValues = validator.MakePossibleValues(phoneNumber);
        Assert.Equal(expectedValues, actualValues);
    }

    [Theory]
    [InlineData("0722759406", "+254722759406")]
    [InlineData("0722000000", "+254722000000")]
    [InlineData("+254722000000", "+254722000000")]
    [InlineData("254722000000", "+254722000000")]
    [InlineData("722000000", "+254722000000")]
    [InlineData("256722000000", null)]
    [InlineData("0733000000", null)]
    [InlineData("798126409", "+254798126409")]
    [InlineData("111126409", "+254111126409")]
    [InlineData("110126409", "+254110126409")]
    [InlineData("115278316", "+254115278316")]
    public void ToE164_Works(string phoneNumber, string expectedResponse)
    {
        var actualResult = validator.ToE164(phoneNumber);
        Assert.Equal(expectedResponse, actualResult);
    }

    [Theory]
    [InlineData("0722759406", true)]
    [InlineData("254722759406", true)]
    [InlineData("+254722759406", true)]
    [InlineData("722759406", true)]
    [InlineData("111126409", true)]
    [InlineData("0733000000", false)]
    [InlineData("256722000000", false)]
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
            Assert.Contains("must be a valid Safaricom phone number.", val.ErrorMessage);
        }
    }

    [Theory]
    [InlineData("0722759406,254722759406", true)]
    [InlineData("+254722759406,722759406", true)]
    [InlineData("111126409", true)]
    [InlineData("+254722759406,0733000000", false)]
    [InlineData("+254722759406,256722000000", false)]
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
            Assert.Contains("must be a valid Safaricom phone number.", val.ErrorMessage);
        }
    }

    class TestModel1
    {
        [SafaricomPhoneNumber]
        public string? PhoneNumber { get; set; }
    }

    class TestModel2
    {
        [SafaricomPhoneNumber]
        public IList<string>? PhoneNumbers { get; set; }
    }
}
