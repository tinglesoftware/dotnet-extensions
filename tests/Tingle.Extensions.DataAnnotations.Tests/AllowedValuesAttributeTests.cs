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
        var obj = new TestModel1 { SomeValue = value };
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
        var obj = new TestModel2 { SomeValue = value };
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
        var obj = new TestModel3 { SomeValue = value };
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
        var obj = new TestModel4 { SomeValues = new List<string> { value } };
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
        var obj = new TestModel5 { SomeValues = value.ToList(), };
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
        var obj = new TestModel6 { };
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var actual = Validator.TryValidateObject(obj, context, results, true);
        Assert.True(actual);
        Assert.Empty(results);
    }

    class TestModel1
    {
        [AllowedValues(1, 2, 3, 4, 5)]
        public int SomeValue { get; set; }
    }

    class TestModel2
    {
        [AllowedValues(1.1f, 2.2f, 3.3f, 4.4f, 5.5f)]
        public float SomeValue { get; set; }
    }

    class TestModel3
    {
        [AllowedValues("blue", "green", "yellow")]
        public string? SomeValue { get; set; }
    }

    class TestModel4
    {
        [AllowedValues("blue", "green", "yellow")]
        public List<string>? SomeValues { get; set; }
    }

    class TestModel5
    {
        [AllowedValues("blue", "green", "yellow")]
        public List<string>? SomeValues { get; set; }
    }

    class TestModel6
    {
        [AllowedValues(1, 2, 3, 4, 5)]
        public int? SomeIntValue { get; set; }

        [AllowedValues(1.1f, 2.2f, 3.3f, 4.4f, 5.5f)]
        public float? SomeFloatValue { get; set; }

        [AllowedValues("blue", "green", "yellow")]
        public string? SomeStringValue { get; set; }

        [AllowedValues("blue", "green", "yellow")]
        public List<string>? SomeStringValues { get; set; }
    }
}
