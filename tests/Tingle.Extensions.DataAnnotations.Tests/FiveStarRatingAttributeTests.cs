using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Tingle.Extensions.DataAnnotations.Tests;

public class FiveStarRatingAttributeTests
{
    [Theory]
    [InlineData(5.02, false)]
    [InlineData(12, false)]
    [InlineData(6, false)]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(1.2, true)]
    [InlineData(0.234, true)]
    public void FiveStarRating_Validation_Works(float testPin, bool expected)
    {
        var obj = new TestModel { SomeRating = testPin };
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
            Assert.Equal(nameof(TestModel.SomeRating), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.Contains("must be between 0 and 5", val.ErrorMessage);
        }
    }

    class TestModel
    {
        [FiveStarRating]
        public float SomeRating { get; set; }
    }
}
