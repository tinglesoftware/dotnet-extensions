using System.ComponentModel.DataAnnotations;

namespace Tingle.Extensions.DataAnnotations.Tests;

public class TimeZoneAttributeTests
{
    [Theory]
    [InlineData("Etc/UTC", true)] // IANA
    [InlineData("UTC", true)]
    [InlineData("Asia/Tokyo", true)] // IANA
    [InlineData("Tokyo Standard Time", true)]
    [InlineData("Africa/Nairobi", true)] // IANA
    [InlineData("E. Africa Standard Time", true)]
    [InlineData("Tokyo", false)]
    [InlineData("Nairobi", false)]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("A", false)]
    public void TimeZone_Validation_Works(string testValue, bool expected)
    {
        var obj = new TestModel(testValue);
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
            Assert.Equal(nameof(TestModel.SomeValue), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.EndsWith("must be a valid Windows or IANA TimeZone identifier.", val.ErrorMessage);
        }
    }

    record TestModel([property: TimeZone] string? SomeValue);
}
