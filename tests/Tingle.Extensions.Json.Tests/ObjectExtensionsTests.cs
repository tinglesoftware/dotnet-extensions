using System;
using Xunit;

namespace Tingle.Extensions.Json.Tests
{
    [Obsolete("Code under test is obsolete")]
    public class ObjectExtensionsTests
    {
        [Fact]
        public void JsonClone_Works()
        {
            var tt1 = new TestType1 { Value1 = 13, Value2 = "cake1" };
            var tt2 = tt1.JsonClone()!;
            Assert.NotEqual(tt1, tt2); // the instances mut be different
            Assert.Equal(tt1.Value1, tt2.Value1);
            Assert.Equal(tt1.Value2, tt2.Value2);
        }

        [Fact]
        public void JsonConvertTo_Works()
        {
            var tt2 = new TestType2 { Value1 = 13, Value2 = "cake1" };
            var tt3 = tt2.JsonConvertTo<TestType3>()!;
            Assert.NotEqual((object)tt2, tt3);
            Assert.Equal(tt2.Value1, tt3.Value1);
            Assert.Equal(tt2.Value2, tt3.Value2);
            Assert.Equal(tt2.Value3, tt3.Value3);
            Assert.Null(tt2.Value3);

            tt3.Value3 = DateTimeOffset.UtcNow;
            var tt2_n = tt3.JsonConvertTo<TestType2>()!;
            Assert.NotEqual((object)tt3, tt2_n);
            Assert.Equal(tt3.Value1, tt2_n.Value1);
            Assert.Equal(tt3.Value2, tt2_n.Value2);
            Assert.Equal(tt3.Value3, tt2_n.Value3);
        }

        class TestType1
        {
            public int Value1 { get; set; }
            public string? Value2 { get; set; }
        }

        class TestType2 : TestType1
        {
            public DateTimeOffset? Value3 { get; set; }
        }

        class TestType3
        {
            public int Value1 { get; set; }
            public string? Value2 { get; set; }
            public DateTimeOffset? Value3 { get; set; }
        }
    }
}
