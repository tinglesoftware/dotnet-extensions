using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Tingle.Extensions.DataAnnotations.Tests
{
    public class DateMustBeInTheFutureAttributeTests
    {
        [Fact]
        public void DateMustBeInTheFuture_Validation_Works_For_DateTime()
        {
            var obj = new TestModel1 { SomeDate = DateTime.UtcNow.AddMinutes(1) };
            var context = new ValidationContext(obj);
            var results = new List<ValidationResult>();
            var actual = Validator.TryValidateObject(obj, context, results, true);
            Assert.True(actual);
            Assert.Empty(results);

            // now test for a date in the past
            obj.SomeDate = DateTime.UtcNow.AddMinutes(-1);
            //context = new ValidationContext(obj);
            results = new List<ValidationResult>();
            actual = Validator.TryValidateObject(obj, context, results, true);
            Assert.False(actual);
            var val = Assert.Single(results);
            var memeberName = Assert.Single(val.MemberNames);
            Assert.Equal(nameof(TestModel1.SomeDate), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.EndsWith("must be a date in the future.", val.ErrorMessage);
        }

        [Fact]
        public void DateMustBeInTheFuture_Validation_Works_For_DateTimeOffset()
        {
            var obj = new TestModel2 { SomeDate = DateTimeOffset.UtcNow.AddMinutes(1) };
            var context = new ValidationContext(obj);
            var results = new List<ValidationResult>();
            var actual = Validator.TryValidateObject(obj, context, results, true);
            Assert.True(actual);
            Assert.Empty(results);

            // now test for a date in the past
            obj.SomeDate = DateTimeOffset.UtcNow.AddMinutes(-1);
            //context = new ValidationContext(obj);
            results = new List<ValidationResult>();
            actual = Validator.TryValidateObject(obj, context, results, true);
            Assert.False(actual);
            var val = Assert.Single(results);
            var memeberName = Assert.Single(val.MemberNames);
            Assert.Equal(nameof(TestModel2.SomeDate), memeberName);
            Assert.NotNull(val.ErrorMessage);
            Assert.NotEmpty(val.ErrorMessage);
            Assert.EndsWith("must be a date in the future.", val.ErrorMessage);
        }

        [Fact]
        public void DateMustBeInTheFuture_Validation_IgnoresNonDateField()
        {
            var obj = new TestModel3 { SomeField1 = Guid.NewGuid().ToString(), SomeField2 = new Random().Next(0, int.MaxValue) };
            var context = new ValidationContext(obj);
            var results = new List<ValidationResult>();
            var actual = Validator.TryValidateObject(obj, context, results, true);
            Assert.True(actual);
            Assert.Empty(results);
        }

        class TestModel1
        {
            [DateMustBeInTheFuture]
            public DateTime SomeDate { get; set; }
        }

        class TestModel2
        {
            [DateMustBeInTheFuture]
            public DateTimeOffset SomeDate { get; set; }
        }

        class TestModel3
        {
            [DateMustBeInTheFuture]
            public string SomeField1 { get; set; }

            [DateMustBeInTheFuture]
            public int SomeField2 { get; set; }
        }
    }
}
