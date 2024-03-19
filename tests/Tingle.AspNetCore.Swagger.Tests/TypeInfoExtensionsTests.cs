using System.Reflection;

namespace Tingle.AspNetCore.Swagger.Tests;

public class TypeInfoExtensionsTests
{
    [Fact]
    public void GetAllFields_Works()
    {
        var fields = typeof(TestDerived).GetTypeInfo().GetAllFields();

        Assert.Contains(fields, f => f.Name == "FooField");
        Assert.Contains(fields, f => f.Name == "BarField");
    }

    [Fact]
    public void GetAllProperties_Works()
    {
        var properties = typeof(TestDerived).GetTypeInfo().GetAllProperties();

        Assert.Contains(properties, p => p.Name == "FooProp");
        Assert.Contains(properties, p => p.Name == "BarProp");
    }

    [Fact]
    public void GetAllMembers_Works()
    {
        var members = typeof(TestDerived).GetTypeInfo().GetAllMembers();

        Assert.Contains(members, p => p.Name == "FooProp");
        Assert.Contains(members, p => p.Name == "BarProp");
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
