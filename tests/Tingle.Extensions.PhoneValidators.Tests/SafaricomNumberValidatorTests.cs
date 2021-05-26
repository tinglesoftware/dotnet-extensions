using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tingle.Extensions.PhoneValidators.Safaricom;
using Xunit;

namespace Tingle.Extensions.PhoneValidators.Tests
{
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
        [InlineData("0728837078", true)]
        [InlineData("+254728837078", true)]
        [InlineData("+254110837078", true)]
        [InlineData("254728837078", true)]
        [InlineData("254719666721", true)]
        [InlineData("254791135160", true)]
        [InlineData("728837078", true)]
        [InlineData("798126409", true)]
        [InlineData("00728837078", false)]
        [InlineData("256728837078", false)]
        [InlineData("25472883707", false)]
        [InlineData("254709677872", true)]
        [InlineData("254111677872", true)]
        [InlineData("0733491887", false)]
        [InlineData("0757491887", true)]
        [InlineData("0755491887", false)]
        [InlineData("0769123123", true)]
        [InlineData("0768123123", true)]
        [InlineData("0111837078", true)]
        [InlineData("0110837078", true)]
        [InlineData("0115278316", true)]
        public void IsValid_Works(string phoneNumber, bool expected)
        {
            var actual = validator.IsValid(phoneNumber);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("0722759406", "254722759406")]
        [InlineData("0728837078", "254728837078")]
        [InlineData("+254728837078", "254728837078")]
        [InlineData("254728837078", "254728837078")]
        [InlineData("728837078", "254728837078")]
        [InlineData("256728837078", null)]
        [InlineData("0733491887", null)]
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
        [InlineData("254728837078", new string[] { "254728837078", "0728837078", "728837078" })]
        [InlineData("728837078", new string[] { "254728837078", "0728837078", "728837078" })]
        [InlineData("798126409", new string[] { "254798126409", "0798126409", "798126409" })]
        [InlineData("111126409", new string[] { "254111126409", "0111126409", "111126409" })]
        [InlineData("110126409", new string[] { "254110126409", "0110126409", "110126409" })]
        [InlineData("115278316", new string[] { "254115278316", "0115278316", "115278316"})]
        [InlineData("0733491887", new string[] { })]
        public void MakePossibleValues_Works(string phoneNumber, string[] expectedValues)
        {
            var actualValues = validator.MakePossibleValues(phoneNumber);
            Assert.Equal(expectedValues, actualValues);
        }

        [Theory]
        [InlineData("0722759406", "+254722759406")]
        [InlineData("0728837078", "+254728837078")]
        [InlineData("+254728837078", "+254728837078")]
        [InlineData("254728837078", "+254728837078")]
        [InlineData("728837078", "+254728837078")]
        [InlineData("256728837078", null)]
        [InlineData("0733491887", null)]
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
        [InlineData("0733491887", false)]
        [InlineData("256728837078", false)]
        [InlineData("", true)]
        [InlineData(null, true)]
        [InlineData("A", false)]
        public void SafaricomPhoneNumber_Validation_Works(string testPin, bool expected)
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
                Assert.Contains("must be a valid Safaricom phone number.", val.ErrorMessage);
            }
        }

        class TestModel
        {
            [SafaricomPhoneNumber]
            public string PhoneNumber { get; set; }
        }
    }
}
