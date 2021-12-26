using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using Tingle.Extensions.PhoneValidators.Airtel;

namespace Tingle.Extensions.PhoneValidators.Tests;

public class AirtelNumberValidatorTests
{
    private readonly AirtelPhoneNumberValidator validator;

    public AirtelNumberValidatorTests()
    {
        var services = new ServiceCollection()
            .AddAirtelPhoneNumberValidator();

        var provider = services.BuildServiceProvider();

        validator = provider.GetRequiredService<AirtelPhoneNumberValidator>();
    }

    [Theory]
    [InlineData("0733000000", true)]
    [InlineData("+254733000000", true)]
    [InlineData("+254101000000", true)]
    [InlineData("254733000000", true)]
    [InlineData("254100000000", true)]
    [InlineData("733000000", true)]
    [InlineData("102000000", true)]
    [InlineData("00733000000", false)]
    [InlineData("256733000000", false)]
    [InlineData("25473349188", false)]
    [InlineData("25410049188", false)]
    [InlineData("0722000000", false)]
    public void IsValid_Works(string phoneNumber, bool expected)
    {
        var actual = validator.IsValid(phoneNumber);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("0733000000", "254733000000")]
    [InlineData("0102000000", "254102000000")]
    [InlineData("+254733000000", "254733000000")]
    [InlineData("+254102000000", "254102000000")]
    [InlineData("254733000000", "254733000000")]
    [InlineData("254100000000", "254100000000")]
    [InlineData("733000000", "254733000000")]
    [InlineData("101000000", "254101000000")]
    [InlineData("256733000000", null)]
    [InlineData("0722000000", null)]
    public void ToMsisdn_Works(string phoneNumber, string expectedResponse)
    {
        var actualResult = validator.ToMsisdn(phoneNumber);
        Assert.Equal(expectedResponse, actualResult);
    }

    [Theory]
    [InlineData("254733000000", new string[] { "254733000000", "0733000000", "733000000" })]
    [InlineData("254100000000", new string[] { "254100000000", "0100000000", "100000000" })]
    [InlineData("733000000", new string[] { "254733000000", "0733000000", "733000000" })]
    [InlineData("102000000", new string[] { "254102000000", "0102000000", "102000000" })]
    [InlineData("0722759406", new string[] { })]
    public void MakePossibleValues_Works(string phoneNumber, string[] expectedValues)
    {
        var actualValues = validator.MakePossibleValues(phoneNumber);
        Assert.Equal(expectedValues, actualValues);
    }

    [Theory]
    [InlineData("0733000000", "+254733000000")]
    [InlineData("0102000000", "+254102000000")]
    [InlineData("+254733000000", "+254733000000")]
    [InlineData("+254102000000", "+254102000000")]
    [InlineData("254733000000", "+254733000000")]
    [InlineData("254100000000", "+254100000000")]
    [InlineData("733000000", "+254733000000")]
    [InlineData("101000000", "+254101000000")]
    [InlineData("256733000000", null)]
    [InlineData("0722000000", null)]
    public void ToE164_Works(string phoneNumber, string expectedResponse)
    {
        var actualResult = validator.ToE164(phoneNumber);
        Assert.Equal(expectedResponse, actualResult);
    }

    [Theory]
    [InlineData("0733000000", true)]
    [InlineData("254733000000", true)]
    [InlineData("+254733000000", true)]
    [InlineData("102000000", true)]
    [InlineData("733000000", true)]
    [InlineData("0722759406", false)]
    [InlineData("256733000000", false)]
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
            Assert.Contains("must be a valid Airtel phone number.", val.ErrorMessage);
        }
    }

    [Theory]
    [InlineData("0733000000,254733000000", true)]
    [InlineData("+254733000000,102000000", true)]
    [InlineData("733000000", true)]
    [InlineData("+254733000000,0722759406", false)]
    [InlineData("+254733000000,256733000000", false)]
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
            Assert.Contains("must be a valid Airtel phone number.", val.ErrorMessage);
        }
    }

    class TestModel1
    {
        [AirtelPhoneNumber]
        public string? PhoneNumber { get; set; }
    }

    class TestModel2
    {
        [AirtelPhoneNumber]
        public IList<string>? PhoneNumbers { get; set; }
    }
}
