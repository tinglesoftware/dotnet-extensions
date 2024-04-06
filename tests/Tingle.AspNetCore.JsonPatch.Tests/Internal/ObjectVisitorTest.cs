using System.Dynamic;
using System.Text.Json;

namespace Tingle.AspNetCore.JsonPatch.Internal;

public class ObjectVisitorTest
{
    private class Class1
    {
        public string? Name { get; set; }
        public IList<string>? States { get; set; } = new List<string>();
        public IDictionary<string, string> CountriesAndRegions { get; set; } = new Dictionary<string, string>();
        public dynamic Items { get; set; } = new ExpandoObject();
    }

    private class Class1Nested
    {
        public List<Class1> Customers { get; set; } = [];
    }

    [Theory]
    [ClassData(typeof(ReturnsListAdapterData))]
    public void Visit_ValidPathToArray_ReturnsListAdapter(object targetObject, string path, object expectedTargetObject)
    {
        // Arrange
        var visitor = new ObjectVisitor(new ParsedPath(path), new JsonSerializerOptions(), create: false);

        // Act
        var visitStatus = visitor.TryVisit(ref targetObject, out var adapter, out var message);

        // Assert
        Assert.True(visitStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.Same(expectedTargetObject, targetObject);
        Assert.IsType<ListAdapter>(adapter);
    }

    class ReturnsListAdapterData : TheoryData<object?, string, object?>
    {
        public ReturnsListAdapterData()
        {
            var model = new Class1();
            Add(model, "/States/-", model.States);
            Add(model.States, "/-", model.States);

            var nestedModel = new Class1Nested();
            nestedModel.Customers.Add(new Class1());
            Add(nestedModel, "/Customers/0/States/-", nestedModel.Customers[0].States);
            Add(nestedModel, "/Customers/0/States/0", nestedModel.Customers[0].States);
            Add(nestedModel.Customers, "/0/States/-", nestedModel.Customers[0].States);
            Add(nestedModel.Customers[0], "/States/-", nestedModel.Customers[0].States);
        }
    }

    [Theory]
    [ClassData(typeof(ReturnsDictionaryAdapterData))]
    public void Visit_ValidPathToDictionary_ReturnsDictionaryAdapter(object targetObject, string path, object expectedTargetObject)
    {
        // Arrange
        var visitor = new ObjectVisitor(new ParsedPath(path), new JsonSerializerOptions(), create: false);

        // Act
        var visitStatus = visitor.TryVisit(ref targetObject, out _, out var message);

        // Assert
        Assert.True(visitStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.Same(expectedTargetObject, targetObject);
        //Assert.Equal(typeof(DictionaryAdapter<string, string>), adapter.GetType());
    }

    class ReturnsDictionaryAdapterData : TheoryData<object, string, object>
    {
        public ReturnsDictionaryAdapterData()
        {
            var model = new Class1();
            Add(model, "/CountriesAndRegions/USA", model.CountriesAndRegions);
            Add(model.CountriesAndRegions, "/USA", model.CountriesAndRegions);

            var nestedModel = new Class1Nested();
            nestedModel.Customers.Add(new Class1());
            Add(nestedModel, "/Customers/0/CountriesAndRegions/USA", nestedModel.Customers[0].CountriesAndRegions);
            Add(nestedModel.Customers, "/0/CountriesAndRegions/USA", nestedModel.Customers[0].CountriesAndRegions);
            Add(nestedModel.Customers[0], "/CountriesAndRegions/USA", nestedModel.Customers[0].CountriesAndRegions);
        }
    }

    [Theory]
    [ClassData(typeof(ReturnsExpandoAdapterData))]
    public void Visit_ValidPathToExpandoObject_ReturnsExpandoAdapter(object targetObject, string path, object expectedTargetObject)
    {
        // Arrange
        var visitor = new ObjectVisitor(new ParsedPath(path), new JsonSerializerOptions(), create: false);

        // Act
        var visitStatus = visitor.TryVisit(ref targetObject, out _, out var message);

        // Assert
        Assert.True(visitStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.Same(expectedTargetObject, targetObject);
        //Assert.Same(typeof(DictionaryAdapter<string, object>), adapter.GetType());
    }

    class ReturnsExpandoAdapterData : TheoryData<object, string, object>
    {
        public ReturnsExpandoAdapterData()
        {
            var nestedModel = new Class1Nested();
            nestedModel.Customers.Add(new Class1());
            Add(nestedModel, "/Customers/0/Items/Name", nestedModel.Customers[0].Items);
            Add(nestedModel.Customers, "/0/Items/Name", nestedModel.Customers[0].Items);
            Add(nestedModel.Customers[0], "/Items/Name", nestedModel.Customers[0].Items);
        }
    }

    [Theory]
    [ClassData(typeof(ReturnsPocoAdapterData))]
    public void Visit_ValidPath_ReturnsExpandoAdapter(object targetObject, string path, object expectedTargetObject)
    {
        // Arrange
        var visitor = new ObjectVisitor(new ParsedPath(path), new JsonSerializerOptions(), create: false);

        // Act
        var visitStatus = visitor.TryVisit(ref targetObject, out var adapter, out var message);

        // Assert
        Assert.True(visitStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.Same(expectedTargetObject, targetObject);
        Assert.IsType<PocoAdapter>(adapter);
    }

    class ReturnsPocoAdapterData : TheoryData<object, string, object>
    {
        public ReturnsPocoAdapterData()
        {
            var model = new Class1();
            Add(model, "/Name", model);

            var nestedModel = new Class1Nested();
            nestedModel.Customers.Add(new Class1());
            Add(nestedModel, "/Customers/0/Name", nestedModel.Customers[0]);
            Add(nestedModel.Customers, "/0/Name", nestedModel.Customers[0]);
            Add(nestedModel.Customers[0], "/Name", nestedModel.Customers[0]);
        }
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    public void Visit_InvalidIndexToArray_Fails(string position)
    {
        // Arrange
        var visitor = new ObjectVisitor(new ParsedPath($"/Customers/{position}/States/-"), new JsonSerializerOptions(), create: false);
        var automobileDepartment = new Class1Nested();
        object targetObject = automobileDepartment;

        // Act
        var visitStatus = visitor.TryVisit(ref targetObject, out _, out var message);

        // Assert
        Assert.False(visitStatus);
        Assert.Equal($"The index value provided by path segment '{position}' is out of bounds of the array size.", message);
    }

    [Theory]
    [InlineData("-")]
    [InlineData("foo")]
    public void Visit_InvalidIndexFormatToArray_Fails(string position)
    {
        // Arrange
        var visitor = new ObjectVisitor(new ParsedPath($"/Customers/{position}/States/-"), new JsonSerializerOptions(), create: false);
        var automobileDepartment = new Class1Nested();
        object targetObject = automobileDepartment;

        // Act
        var visitStatus = visitor.TryVisit(ref targetObject, out _, out var message);

        // Assert
        Assert.False(visitStatus);
        Assert.Equal($"The path segment '{position}' is invalid for an array index.", message);
    }

    [Fact]
    public void Visit_DoesNotValidate_FinalPathSegment()
    {
        // Arrange
        var visitor = new ObjectVisitor(new ParsedPath($"/NonExisting"), new JsonSerializerOptions(), create: false);
        var model = new Class1();
        object targetObject = model;

        // Act
        var visitStatus = visitor.TryVisit(ref targetObject, out var adapter, out var message);

        // Assert
        Assert.True(visitStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.IsType<PocoAdapter>(adapter);
    }

    [Fact]
    public void Visit_NullInteriorTarget_ReturnsFalse()
    {
        // Arrange
        var visitor = new ObjectVisitor(new ParsedPath("/States/0"), new JsonSerializerOptions(), create: false);

        // Act
        object? target = new Class1() { States = null, };
        var visitStatus = visitor.TryVisit(ref target, out var adapter, out var message);

        // Assert
        Assert.False(visitStatus);
        Assert.Null(adapter);
        Assert.Null(message);
    }

    [Fact]
    public void Visit_NullTarget_ReturnsNullAdapter()
    {
        // Arrange
        var visitor = new ObjectVisitor(new ParsedPath("test"), new JsonSerializerOptions(), create: false);

        // Act
        object? target = null;
        var visitStatus = visitor.TryVisit(ref target!, out var adapter, out var message);

        // Assert
        Assert.False(visitStatus);
        Assert.Null(adapter);
        Assert.Null(message);
    }
}
