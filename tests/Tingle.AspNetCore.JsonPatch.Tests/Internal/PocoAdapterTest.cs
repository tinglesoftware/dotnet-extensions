using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tingle.AspNetCore.JsonPatch.Internal;

public class PocoAdapterTest
{
    [Fact]
    public void TryAdd_ReplacesExistingProperty()
    {
        // Arrange
        var adapter = new PocoAdapter();
        var options = new JsonSerializerOptions();
        var model = new Customer
        {
            Name = "Joana"
        };

        // Act
        var addStatus = adapter.TryAdd(model, "Name", options, "John", out var errorMessage);

        // Assert
        Assert.Equal("John", model.Name);
        Assert.True(addStatus);
        Assert.True(string.IsNullOrEmpty(errorMessage), "Expected no error message");
    }

    [Fact]
    public void TryAdd_ThrowsJsonPatchException_IfPropertyDoesNotExist()
    {
        // Arrange
        var adapter = new PocoAdapter();
        var options = new JsonSerializerOptions();
        var model = new Customer
        {
            Name = "Joana"
        };
        var expectedErrorMessage = "The target location specified by path segment 'LastName' was not found.";

        // Act
        var addStatus = adapter.TryAdd(model, "LastName", options, "Smith", out var errorMessage);

        // Assert
        Assert.False(addStatus);
        Assert.Equal(expectedErrorMessage, errorMessage);
    }

    [Fact]
    public void TryGet_ExistingProperty()
    {
        // Arrange
        var adapter = new PocoAdapter();
        var options = new JsonSerializerOptions();
        var model = new Customer
        {
            Name = "Joana"
        };

        // Act
        var getStatus = adapter.TryGet(model, "Name", options, out var value, out var errorMessage);

        // Assert
        Assert.Equal("Joana", value);
        Assert.True(getStatus);
        Assert.True(string.IsNullOrEmpty(errorMessage), "Expected no error message");
    }

    [Fact]
    public void TryGet_ThrowsJsonPatchException_IfPropertyDoesNotExist()
    {
        // Arrange
        var adapter = new PocoAdapter();
        var options = new JsonSerializerOptions();
        var model = new Customer
        {
            Name = "Joana"
        };
        var expectedErrorMessage = "The target location specified by path segment 'LastName' was not found.";

        // Act
        var getStatus = adapter.TryGet(model, "LastName", options, out var value, out var errorMessage);

        // Assert
        Assert.Null(value);
        Assert.False(getStatus);
        Assert.Equal(expectedErrorMessage, errorMessage);
    }

    [Fact]
    public void TryRemove_SetsPropertyToNull()
    {
        // Arrange
        var adapter = new PocoAdapter();
        var options = new JsonSerializerOptions();
        var model = new Customer
        {
            Name = "Joana"
        };

        // Act
        var removeStatus = adapter.TryRemove(model, "Name", options, out var errorMessage);

        // Assert
        Assert.Null(model.Name);
        Assert.True(removeStatus);
        Assert.True(string.IsNullOrEmpty(errorMessage), "Expected no error message");
    }

    [Fact]
    public void TryRemove_ThrowsJsonPatchException_IfPropertyDoesNotExist()
    {
        // Arrange
        var adapter = new PocoAdapter();
        var options = new JsonSerializerOptions();
        var model = new Customer
        {
            Name = "Joana"
        };
        var expectedErrorMessage = "The target location specified by path segment 'LastName' was not found.";

        // Act
        var removeStatus = adapter.TryRemove(model, "LastName", options, out var errorMessage);

        // Assert
        Assert.False(removeStatus);
        Assert.Equal(expectedErrorMessage, errorMessage);
    }

    [Fact]
    public void TryReplace_OverwritesExistingValue()
    {
        // Arrange
        var adapter = new PocoAdapter();
        var options = new JsonSerializerOptions();
        var model = new Customer
        {
            Name = "Joana"
        };

        // Act
        var replaceStatus = adapter.TryReplace(model, "Name", options, "John", out var errorMessage);

        // Assert
        Assert.Equal("John", model.Name);
        Assert.True(replaceStatus);
        Assert.True(string.IsNullOrEmpty(errorMessage), "Expected no error message");
    }

    [Fact]
    public void TryReplace_ThrowsJsonPatchException_IfNewValueIsInvalidType()
    {
        // Arrange
        var adapter = new PocoAdapter();
        var options = new JsonSerializerOptions();
        var model = new Customer
        {
            Age = 25
        };

        var expectedErrorMessage = "The value 'TwentySix' is invalid for target location.";

        // Act
        var replaceStatus = adapter.TryReplace(model, "Age", options, "TwentySix", out var errorMessage);

        // Assert
        Assert.Equal(25, model.Age);
        Assert.False(replaceStatus);
        Assert.Equal(expectedErrorMessage, errorMessage);
    }

    [Fact]
    public void TryReplace_ThrowsJsonPatchException_IfPropertyDoesNotExist()
    {
        // Arrange
        var adapter = new PocoAdapter();
        var options = new JsonSerializerOptions();
        var model = new Customer
        {
            Name = "Joana"
        };
        var expectedErrorMessage = "The target location specified by path segment 'LastName' was not found.";

        // Act
        var replaceStatus = adapter.TryReplace(model, "LastName", options, "Smith", out var errorMessage);

        // Assert
        Assert.Equal("Joana", model.Name);
        Assert.False(replaceStatus);
        Assert.Equal(expectedErrorMessage, errorMessage);
    }

    [Fact]
    public void TryReplace_UsesCustomConverter()
    {
        // Arrange
        var adapter = new PocoAdapter();
        var options = new JsonSerializerOptions { Converters = { new RectangleJsonConverter(), }, };
        var model = new Square()
        {
            Rectangle = new Rectangle()
            {
                RectangleProperty = "Square"
            }
        };

        // Act
        var replaceStatus = adapter.TryReplace(model, "Rectangle", options, "Oval", out _);

        // Assert
        Assert.Equal("Oval", model.Rectangle.RectangleProperty);
        Assert.True(replaceStatus);
    }

    [Fact]
    public void TryTest_DoesNotThrowException_IfTestSuccessful()
    {
        var adapter = new PocoAdapter();
        var options = new JsonSerializerOptions();
        var model = new Customer
        {
            Name = "Joana"
        };

        // Act
        var testStatus = adapter.TryTest(model, "Name", options, "Joana", out var errorMessage);

        // Assert
        Assert.Equal("Joana", model.Name);
        Assert.True(testStatus);
        Assert.True(string.IsNullOrEmpty(errorMessage), "Expected no error message");
    }

    [Fact]
    public void TryTest_ThrowsJsonPatchException_IfTestFails()
    {
        // Arrange
        var adapter = new PocoAdapter();
        var options = new JsonSerializerOptions();
        var model = new Customer
        {
            Name = "Joana"
        };
        var expectedErrorMessage = "The current value 'Joana' at path 'Name' is not equal to the test value 'John'.";

        // Act
        var testStatus = adapter.TryTest(model, "Name", options, "John", out var errorMessage);

        // Assert
        Assert.False(testStatus);
        Assert.Equal(expectedErrorMessage, errorMessage);
    }

    private class Customer
    {
        public string? Name { get; set; }

        public int Age { get; set; }
    }

    public class RectangleJsonConverter : JsonConverter<Rectangle>
    {
        public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new Rectangle
            {
                RectangleProperty = reader.GetString(),
            };
        }

        public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
