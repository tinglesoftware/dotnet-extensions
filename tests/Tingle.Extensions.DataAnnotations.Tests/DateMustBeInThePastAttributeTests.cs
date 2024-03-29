﻿using System.ComponentModel.DataAnnotations;

namespace Tingle.Extensions.DataAnnotations.Tests;

public class DateMustBeInThePastAttributeTests
{
    [Fact]
    public void DateMustBeInThePast_Validation_Works_For_DateTime()
    {
        var obj = new TestModel1(DateTime.UtcNow.AddMinutes(-1));
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var actual = Validator.TryValidateObject(obj, context, results, true);
        Assert.True(actual);
        Assert.Empty(results);

        // now test for a date in the past
        obj = new TestModel1(DateTime.UtcNow.AddMinutes(1));
        context = new ValidationContext(obj);
        results = [];
        actual = Validator.TryValidateObject(obj, context, results, true);
        Assert.False(actual);
        var val = Assert.Single(results);
        var memeberName = Assert.Single(val.MemberNames);
        Assert.Equal(nameof(TestModel1.SomeDate), memeberName);
        Assert.NotNull(val.ErrorMessage);
        Assert.NotEmpty(val.ErrorMessage);
        Assert.EndsWith("must be a date in the past.", val.ErrorMessage);
    }

    [Fact]
    public void DateMustBeInThePast_Validation_Works_For_DateTimeOffset()
    {
        var obj = new TestModel2(DateTimeOffset.UtcNow.AddMinutes(-1));
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var actual = Validator.TryValidateObject(obj, context, results, true);
        Assert.True(actual);
        Assert.Empty(results);

        // now test for a date in the past
        obj = new TestModel2(DateTimeOffset.UtcNow.AddMinutes(1));
        context = new ValidationContext(obj);
        results = [];
        actual = Validator.TryValidateObject(obj, context, results, true);
        Assert.False(actual);
        var val = Assert.Single(results);
        var memeberName = Assert.Single(val.MemberNames);
        Assert.Equal(nameof(TestModel2.SomeDate), memeberName);
        Assert.NotNull(val.ErrorMessage);
        Assert.NotEmpty(val.ErrorMessage);
        Assert.EndsWith("must be a date in the past.", val.ErrorMessage);
    }

    [Fact]
    public void DateMustBeInThePast_Validation_IgnoresNonDateField()
    {
        var obj = new TestModel3(Guid.NewGuid().ToString(), new Random().Next(0, int.MaxValue));
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        var actual = Validator.TryValidateObject(obj, context, results, true);
        Assert.True(actual);
        Assert.Empty(results);
    }

    record TestModel1([property: DateMustBeInThePast] DateTime SomeDate);
    record TestModel2([property: DateMustBeInThePast] DateTimeOffset SomeDate);
    record TestModel3([property: DateMustBeInThePast] string? SomeField1,
                      [property: DateMustBeInThePast] int SomeField2);
}
