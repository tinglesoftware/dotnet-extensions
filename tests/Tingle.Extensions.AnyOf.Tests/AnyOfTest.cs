using System;
using System.Collections.Generic;
using Xunit;

namespace Tingle.Extensions.AnyOf.Tests
{
    public class AnyOfTest
    {
        [Theory]
        [MemberData(nameof(variant2TypesTestData))]
        public void Ctor_Variant2Types(object arg)
        {
            var @case = Assert.IsAssignableFrom<TestCase>(arg);
            Assert.Equal(@case.WantValue, @case.AnyOf.Value);
            Assert.Equal(@case.WantType, @case.AnyOf.Type);
        }

        public static readonly IEnumerable<object[]> variant2TypesTestData = new List<object[]>
        {
            new object []{
                new TestCase(new AnyOf<int, string>(default(int)),
                             default(int),
                             typeof(int)),
            },

            new object []{
                new TestCase(new AnyOf<int, string>(234),
                             234,
                             typeof(int)),
            },

            new object []{
                new TestCase(new AnyOf<int, string>(default(string?)),
                             default(string?),
                             typeof(string)),
            },

            new object []{
                new TestCase(new AnyOf<int, string>("Hello!"),
                             "Hello!",
                             typeof(string)),
            },

            new object []{
                new TestCase(new AnyOf<int?, string>(default(int?)),
                             default(int?),
                             typeof(int?)),
            },
        };

        [Theory]
        [MemberData(nameof(GetVariant3TypesTestData))]
        public void Ctor_Variant3Types(object arg)
        {
            var @case = Assert.IsAssignableFrom<TestCase>(arg);
            Assert.Equal(@case.WantValue, @case.AnyOf.Value);
            Assert.Equal(@case.WantType, @case.AnyOf.Type);
        }

        public static IEnumerable<object[]> GetVariant3TypesTestData()
        {

            yield return new object[]{
                new TestCase(new AnyOf<int, string, SomeClass>(default(int)),
                             default(int),
                             typeof(int)),
            };

            yield return new object[]{
                new TestCase(new AnyOf<int, string, SomeClass>(234),
                             234,
                             typeof(int)),
            };

            yield return new object[]{
                new TestCase(new AnyOf<int, string, SomeClass>(default(string)),
                             default(string),
                             typeof(string)),
            };

            yield return new object[]{
                new TestCase(new AnyOf<int, string, SomeClass>("Hello!"),
                             "Hello!",
                             typeof(string)),
            };

            yield return new object[]{
                new TestCase(new AnyOf<int?, string, SomeClass>(default(int?)),
                             default(int?),
                             typeof(int?)),
            };

            yield return new object[]{
                new TestCase(new AnyOf<int?, string, SomeClass>(default(SomeClass)),
                             default(SomeClass),
                             typeof(SomeClass)),
            };

            var someClass = new SomeClass { SomeString = "foo" };
            yield return new object[]{
                new TestCase(new AnyOf<int?, string, SomeClass>(someClass),
                             someClass,
                             typeof(SomeClass)),
            };
        }

        [Theory]
        [MemberData(nameof(implicitOperatorTestData))]
        public void ImplicitOperator_ReturnsNullForNullValuesRegardlessOfType(object arg)
        {
            var container = Assert.IsAssignableFrom<Container>(arg);
            Assert.Null(container.AnyOf2);
            Assert.Null(container.AnyOf3);
        }

        public static readonly IEnumerable<object[]> implicitOperatorTestData = new List<object[]>
        {
            new object []{new Container(),},
            new object []{new Container { AnyOf2 = null },},
            new object []{new Container { AnyOf2 = null },},
            new object []{new Container { AnyOf2 = null },},
            new object []{new Container { AnyOf3 = null },},
            new object []{new Container { AnyOf3 = null },},
            new object []{new Container { AnyOf3 = null },},
            new object []{new Container { AnyOf3 = null },},
        };

        private class TestCase
        {
            public TestCase(IAnyOf anyOf, object? wantValue, Type wantType)
            {
                AnyOf = anyOf ?? throw new ArgumentNullException(nameof(anyOf));
                WantValue = wantValue;
                WantType = wantType ?? throw new ArgumentNullException(nameof(wantType));
            }

            public IAnyOf AnyOf { get; set; }

            public object? WantValue { get; set; }

            public Type WantType { get; set; }
        }

        private class SomeClass
        {
            public string? SomeString { get; set; }
        }

        private class Container
        {
            public AnyOf<int, string>? AnyOf2 { get; set; }

            public AnyOf<int, string, SomeClass>? AnyOf3 { get; set; }
        }
    }
}
