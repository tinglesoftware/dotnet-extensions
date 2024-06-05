using System.Reflection;
using Tingle.AspNetCore.Swagger.Filters.Schemas;

namespace Tingle.AspNetCore.Swagger.Tests;

public class InheritDocSchemaFilterTests
{
    [Fact]
    public void GetAllMembers_Works()
    {
        var members = InheritDocSchemaFilter.GetAllMembers(typeof(TestDerived).GetTypeInfo());

        var names = members.Select(m => m.Name).ToList();
        Assert.Contains("FooProp", names);
        Assert.Contains("BarProp", names);
    }

    public class TestBase
    {
        public string? FooField;
        public int FooProp { get; set; }
    }

    public class TestDerived : TestBase
    {
        public string? BarField;
        public int BarProp { get; set; }
    }
}
