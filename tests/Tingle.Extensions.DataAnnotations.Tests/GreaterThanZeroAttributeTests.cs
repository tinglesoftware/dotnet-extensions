using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Tingle.Extensions.DataAnnotations.Tests
{
    public class GreaterThanZeroAttributeTests
    {
        [Theory]
        [InlineData(12, true)]
        [InlineData(0, false)]
        [InlineData(-1, false)]
        public void GreaterThanZero_Validation_Works(int testPin, bool expected)
        {
            var obj = new TestModel { MyValue = testPin };
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
                Assert.Equal(nameof(TestModel.MyValue), memeberName);
                Assert.NotNull(val.ErrorMessage);
                Assert.NotEmpty(val.ErrorMessage);
                Assert.Contains($"must be between 1 and {int.MaxValue}", val.ErrorMessage);
            }
        }

        class TestModel
        {
            [GreaterThanZero]
            public int MyValue { get; set; }
        }
    }
}
