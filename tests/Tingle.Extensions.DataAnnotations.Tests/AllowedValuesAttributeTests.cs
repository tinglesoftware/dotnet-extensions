using System.ComponentModel.DataAnnotations;

namespace Tingle.Extensions.DataAnnotations.Tests;

public class AllowedValuesAttributeTests
{
    [Theory]
    [InlineData(1, true)]
    [InlineData(4, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    public void Works_For_Int(int value, bool expected)
    {
        var obj = new TestModel1(value);
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
            Assert.Equal(nameof(TestModel1.SomeValue), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.Equal($"The field {memeberName} only permits: 1,2,3,4,5.", val.ErrorMessage);
        }
    }

    [Theory]
    [InlineData(1.1f, true)]
    [InlineData(4.4f, true)]
    [InlineData(0.2f, false)]
    [InlineData(-1.1f, false)]
    [InlineData(1.101f, false)]
    public void Works_For_Float(float value, bool expected)
    {
        var obj = new TestModel2(value);
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
            Assert.Equal(nameof(TestModel2.SomeValue), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.Equal($"The field {memeberName} only permits: 1.1,2.2,3.3,4.4,5.5.", val.ErrorMessage);
        }
    }

    [Theory]
    [InlineData("green", true)]
    [InlineData("blue", true)]
    [InlineData("yellow", true)]
    [InlineData("yelo", false)]
    [InlineData("YELLOW", false)]
    public void Works_For_String(string value, bool expected)
    {
        var obj = new TestModel3(value);
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
            Assert.Equal(nameof(TestModel2.SomeValue), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.Equal($"The field {memeberName} only permits: blue,green,yellow.", val.ErrorMessage);
        }
    }

    [Theory]
    [InlineData("green", true)]
    [InlineData("blue", true)]
    [InlineData("yellow", true)]
    [InlineData("yelo", false)]
    [InlineData("YELLOW", false)]
    public void Works_For_StringArray(string value, bool expected)
    {
        var obj = new TestModel4(new List<string> { value });
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
            Assert.Equal(nameof(TestModel4.SomeValues), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.Equal($"The field {memeberName} only permits: blue,green,yellow.", val.ErrorMessage);
        }
    }

    [Theory]
    [InlineData(true, "green", "blue", "yellow")]
    [InlineData(true, "blue", "yellow")]
    [InlineData(true, "yellow")]
    [InlineData(false, "green", "blue", "yelo")]
    [InlineData(false, "green", "BLUE", "yellow")]
    public void Works_For_CaseSensitiveStrings(bool expected, params string[] value)
    {
        var obj = new TestModel5(value.ToList());
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
            Assert.Equal(nameof(TestModel5.SomeValues), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.Equal($"The field {memeberName} only permits: blue,green,yellow.", val.ErrorMessage);
        }
    }

    [Fact]
    public void Ignores_Nulls()
    {
        var obj = new TestModel6(null, null, null, null);
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var actual = Validator.TryValidateObject(obj, context, results, true);
        Assert.True(actual);
        Assert.Empty(results);
    }

    record TestModel1([property: AllowedValues(1, 2, 3, 4, 5)] int SomeValue);
    record TestModel2([property: AllowedValues(1.1f, 2.2f, 3.3f, 4.4f, 5.5f)] float SomeValue);
    record TestModel3([property: AllowedValues("blue", "green", "yellow")] string? SomeValue);
    record TestModel4([property: AllowedValues("blue", "green", "yellow")] List<string>? SomeValues);
    record TestModel5([property: AllowedValues("blue", "green", "yellow")] List<string>? SomeValues);
    record TestModel6([property: AllowedValues(1, 2, 3, 4, 5)] int? SomeIntValue,
                      [property: AllowedValues(1.1f, 2.2f, 3.3f, 4.4f, 5.5f)] float? SomeFloatValue,
                      [property: AllowedValues("blue", "green", "yellow")] string? SomeStringValue,
                      [property: AllowedValues("blue", "green", "yellow")] List<string>? SomeStringValues);
}
