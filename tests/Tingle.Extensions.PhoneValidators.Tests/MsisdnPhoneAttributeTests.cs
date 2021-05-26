using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Tingle.Extensions.PhoneValidators.Tests
{
    public class MsisdnPhoneAttributeTests
    {
        [Theory]
        [InlineData("254728837078", true)]
        [InlineData("254733491887", true)]
        [InlineData("254203893501", true)]
        [InlineData("+44 117 496 0123", false)]
        [InlineData("+441174960123", false)]
        [InlineData("+254728837078", false)]
        [InlineData("+254 728 837078", false)]
        [InlineData("+254 72 883 7078", false)]
        [InlineData("0728837078", false)]
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
            public string SomePhoneNumber { get; set; }
        }
    }
}
