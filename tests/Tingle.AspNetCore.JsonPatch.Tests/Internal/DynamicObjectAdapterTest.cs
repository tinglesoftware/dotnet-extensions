using System.Text.Json;

namespace Tingle.AspNetCore.JsonPatch.Internal;

public class DynamicObjectAdapterTest
{
    [Fact]
    public void TryAdd_AddsNewProperty()
    {
        // Arrange
        var adapter = new DynamicObjectAdapter();
        dynamic target = new DynamicTestObject();
        var segment = "NewProperty";
        var options = new JsonSerializerOptions();

        // Act
        var status = adapter.TryAdd(target, segment, options, "new", out string errorMessage);

        // Assert
        Assert.True(status);
        Assert.Null(errorMessage);
        Assert.Equal("new", target.NewProperty);
    }

    [Fact]
    public void TryAdd_ReplacesExistingPropertyValue()
    {
        // Arrange
        var adapter = new DynamicObjectAdapter();
        dynamic target = new DynamicTestObject();
        target.List = new List<int>() { 1, 2, 3 };
        var value = new List<string>() { "stringValue1", "stringValue2" };
        var segment = "List";
        var options = new JsonSerializerOptions();

        // Act
        var status = adapter.TryAdd(target, segment, options, value, out string errorMessage);

        // Assert
        Assert.True(status);
        Assert.Null(errorMessage);
        Assert.Equal(value, target.List);
    }

    [Fact]
    public void TryGet_GetsPropertyValue_ForExistingProperty()
    {
        // Arrange
        var adapter = new DynamicObjectAdapter();
        dynamic target = new DynamicTestObject();
        var segment = "NewProperty";
        var options = new JsonSerializerOptions();

        // Act 1
        var addStatus = adapter.TryAdd(target, segment, options, "new", out string errorMessage);

        // Assert 1
        Assert.True(addStatus);
        Assert.Null(errorMessage);
        Assert.Equal("new", target.NewProperty);

        // Act 2
        var getStatus = adapter.TryGet(target, segment, options, out object getValue, out string getErrorMessage);

        // Assert 2
        Assert.True(getStatus);
        Assert.Null(getErrorMessage);
        Assert.Equal(getValue, target.NewProperty);
    }

    [Fact]
    public void TryGet_ThrowsPathNotFoundException_ForNonExistingProperty()
    {
        // Arrange
        var adapter = new DynamicObjectAdapter();
        dynamic target = new DynamicTestObject();
        var segment = "NewProperty";
        var options = new JsonSerializerOptions();

        // Act
        var getStatus = adapter.TryGet(target, segment, options, out object getValue, out string getErrorMessage);

        // Assert
        Assert.False(getStatus);
        Assert.Null(getValue);
        Assert.Equal($"The target location specified by path segment '{segment}' was not found.", getErrorMessage);
    }

    [Fact]
    public void TryTraverse_FindsNextTarget()
    {
        // Arrange
        var adapter = new DynamicObjectAdapter();
        dynamic target = new DynamicTestObject();
        target.NestedObject = new DynamicTestObject();
        target.NestedObject.NewProperty = "A";
        var segment = "NestedObject";
        var options = new JsonSerializerOptions();

        // Act
        var status = adapter.TryTraverse(target, segment, options, out object nextTarget, out string errorMessage);

        // Assert
        Assert.True(status);
        Assert.Null(errorMessage);
        Assert.Equal(target.NestedObject, nextTarget);
    }

    [Fact]
    public void TryTraverse_ThrowsPathNotFoundException_ForNonExistingProperty()
    {
        // Arrange
        var adapter = new DynamicObjectAdapter();
        dynamic target = new DynamicTestObject();
        target.NestedObject = new DynamicTestObject();
        var segment = "NewProperty";
        var options = new JsonSerializerOptions();

        // Act
        var status = adapter.TryTraverse(target.NestedObject, segment, options, out object _, out string errorMessage);

        // Assert
        Assert.False(status);
        Assert.Equal($"The target location specified by path segment '{segment}' was not found.", errorMessage);
    }

    [Fact]
    public void TryReplace_RemovesExistingValue_BeforeAddingNewValue()
    {
        // Arrange
        var adapter = new DynamicObjectAdapter();
        dynamic target = new WriteOnceDynamicTestObject();
        target.NewProperty = new object();
        var segment = "NewProperty";
        var options = new JsonSerializerOptions();

        // Act
        var status = adapter.TryReplace(target, segment, options, "new", out string errorMessage);

        // Assert
        Assert.True(status);
        Assert.Null(errorMessage);
        Assert.Equal("new", target.NewProperty);
    }

    [Fact]
    public void TryReplace_ThrowsPathNotFoundException_ForNonExistingProperty()
    {
        // Arrange
        var adapter = new DynamicObjectAdapter();
        dynamic target = new DynamicTestObject();
        var segment = "NewProperty";
        var options = new JsonSerializerOptions();

        // Act
        var status = adapter.TryReplace(target, segment, options, "test", out string errorMessage);

        // Assert
        Assert.False(status);
        Assert.Equal($"The target location specified by path segment '{segment}' was not found.", errorMessage);
    }

    [Fact]
    public void TryReplace_ThrowsPropertyInvalidException_IfNewValueIsNotTheSameTypeAsInitialValue()
    {
        // Arrange
        var adapter = new DynamicObjectAdapter();
        dynamic target = new DynamicTestObject();
        target.NewProperty = 1;
        var segment = "NewProperty";
        var options = new JsonSerializerOptions();

        // Act
        var status = adapter.TryReplace(target, segment, options, "test", out string errorMessage);

        // Assert
        Assert.False(status);
        Assert.Equal($"The value 'test' is invalid for target location.", errorMessage);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData("new", null)]
    public void TryRemove_SetsPropertyToDefaultOrNull(object value, object? expectedValue)
    {
        // Arrange
        var adapter = new DynamicObjectAdapter();
        dynamic target = new DynamicTestObject();
        var segment = "NewProperty";
        var options = new JsonSerializerOptions();

        // Act 1
        var addStatus = adapter.TryAdd(target, segment, options, value, out string errorMessage);

        // Assert 1
        Assert.True(addStatus);
        Assert.Null(errorMessage);
        Assert.Equal(value, target.NewProperty);

        // Act 2
        var removeStatus = adapter.TryRemove(target, segment, options, out string removeErrorMessage);

        // Assert 2
        Assert.True(removeStatus);
        Assert.Null(removeErrorMessage);
        Assert.Equal(expectedValue, target.NewProperty);
    }

    [Fact]
    public void TryRemove_ThrowsPathNotFoundException_ForNonExistingProperty()
    {
        // Arrange
        var adapter = new DynamicObjectAdapter();
        dynamic target = new DynamicTestObject();
        var segment = "NewProperty";
        var options = new JsonSerializerOptions();

        // Act
        var removeStatus = adapter.TryRemove(target, segment, options, out string removeErrorMessage);

        // Assert
        Assert.False(removeStatus);
        Assert.Equal($"The target location specified by path segment '{segment}' was not found.", removeErrorMessage);
    }

    [Fact]
    public void TryTest_DoesNotThrowException_IfTestSuccessful()
    {
        var adapter = new DynamicObjectAdapter();
        dynamic target = new DynamicTestObject();
        var value = new List<object>()
        {
            "Joana",
            2,
            new Customer("Joana", 25)
        };
        target.NewProperty = value;
        var segment = "NewProperty";
        var options = new JsonSerializerOptions();

        // Act
        var testStatus = adapter.TryTest(target, segment, options, value, out string errorMessage);

        // Assert
        Assert.Equal(value, target.NewProperty);
        Assert.True(testStatus);
        Assert.True(string.IsNullOrEmpty(errorMessage), "Expected no error message");
    }

    [Fact]
    public void TryTest_ThrowsJsonPatchException_IfTestFails()
    {
        // Arrange
        var adapter = new DynamicObjectAdapter();
        dynamic target = new DynamicTestObject();
        target.NewProperty = "Joana";
        var segment = "NewProperty";
        var options = new JsonSerializerOptions();
        var expectedErrorMessage = $"The current value 'Joana' at path '{segment}' is not equal to the test value 'John'.";

        // Act
        var testStatus = adapter.TryTest(target, segment, options, "John", out string errorMessage);

        // Assert
        Assert.False(testStatus);
        Assert.Equal(expectedErrorMessage, errorMessage);
    }
}
