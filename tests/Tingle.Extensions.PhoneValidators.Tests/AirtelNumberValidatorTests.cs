using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tingle.Extensions.PhoneValidators.Airtel;
using Xunit;

namespace Tingle.Extensions.PhoneValidators.Tests
{
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
        [InlineData("0733491887", true)]
        [InlineData("+254733491887", true)]
        [InlineData("+254101491887", true)]
        [InlineData("254733491887", true)]
        [InlineData("254100491887", true)]
        [InlineData("733491887", true)]
        [InlineData("102491887", true)]
        [InlineData("00733491887", false)]
        [InlineData("256733491887", false)]
        [InlineData("25473349188", false)]
        [InlineData("25410049188", false)]
        [InlineData("0728837078", false)]
        public void IsValid_Works(string phoneNumber, bool expected)
        {
            var actual = validator.IsValid(phoneNumber);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("0733491887", "254733491887")]
        [InlineData("0102491887", "254102491887")]
        [InlineData("+254733491887", "254733491887")]
        [InlineData("+254102491887", "254102491887")]
        [InlineData("254733491887", "254733491887")]
        [InlineData("254100491887", "254100491887")]
        [InlineData("733491887", "254733491887")]
        [InlineData("101491887", "254101491887")]
        [InlineData("256733491887", null)]
        [InlineData("0728837078", null)]
        public void ToMsisdn_Works(string phoneNumber, string expectedResponse)
        {
            var actualResult = validator.ToMsisdn(phoneNumber);
            Assert.Equal(expectedResponse, actualResult);
        }

        [Theory]
        [InlineData("254733491887", new string[] { "254733491887", "0733491887", "733491887" })]
        [InlineData("254100491887", new string[] { "254100491887", "0100491887", "100491887" })]
        [InlineData("733491887", new string[] { "254733491887", "0733491887", "733491887" })]
        [InlineData("102491887", new string[] { "254102491887", "0102491887", "102491887" })]
        [InlineData("0722759406", new string[] { })]
        public void MakePossibleValues_Works(string phoneNumber, string[] expectedValues)
        {
            var actualValues = validator.MakePossibleValues(phoneNumber);
            Assert.Equal(expectedValues, actualValues);
        }

        [Theory]
        [InlineData("0733491887", "+254733491887")]
        [InlineData("0102491887", "+254102491887")]
        [InlineData("+254733491887", "+254733491887")]
        [InlineData("+254102491887", "+254102491887")]
        [InlineData("254733491887", "+254733491887")]
        [InlineData("254100491887", "+254100491887")]
        [InlineData("733491887", "+254733491887")]
        [InlineData("101491887", "+254101491887")]
        [InlineData("256733491887", null)]
        [InlineData("0728837078", null)]
        public void ToE164_Works(string phoneNumber, string expectedResponse)
        {
            var actualResult = validator.ToE164(phoneNumber);
            Assert.Equal(expectedResponse, actualResult);
        }

        [Theory]
        [InlineData("0733491887", true)]
        [InlineData("254733491887", true)]
        [InlineData("+254733491887", true)]
        [InlineData("102491887", true)]
        [InlineData("733491887", true)]
        [InlineData("0722759406", false)]
        [InlineData("256733491887", false)]
        [InlineData("", true)]
        [InlineData(null, true)]
        [InlineData("A", false)]
        public void AirtelPhoneNumber_Validation_Works(string testPin, bool expected)
        {
            var obj = new TestModel { PhoneNumber = testPin };
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
                Assert.Equal(nameof(TestModel.PhoneNumber), memeberName);
                Assert.NotNull(val.ErrorMessage);
                Assert.NotEmpty(val.ErrorMessage);
                Assert.Contains("must be a valid Airtel phone number.", val.ErrorMessage);
            }
        }

        class TestModel
        {
            [AirtelPhoneNumber]
            public string PhoneNumber { get; set; }
        }
    }
}
