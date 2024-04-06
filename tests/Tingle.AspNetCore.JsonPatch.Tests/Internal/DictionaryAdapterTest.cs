using System.Globalization;
using System.Text.Json;

namespace Tingle.AspNetCore.JsonPatch.Internal;

public class DictionaryAdapterTest
{
    [Fact]
    public void Add_KeyWhichAlreadyExists_ReplacesExistingValue()
    {
        // Arrange
        var key = "Status";
        var dictionary = new Dictionary<string, int>(StringComparer.Ordinal) { [key] = 404, };
        var dictionaryAdapter = new DictionaryAdapter<string, int>();
        var options = new JsonSerializerOptions();

        // Act
        var addStatus = dictionaryAdapter.TryAdd(dictionary, key, options, 200, out var message);

        // Assert
        Assert.True(addStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.Single(dictionary);
        Assert.Equal(200, dictionary[key]);
    }

    [Fact]
    public void Add_IntKeyWhichAlreadyExists_ReplacesExistingValue()
    {
        // Arrange
        var intKey = 1;
        var dictionary = new Dictionary<int, object> { [intKey] = "Mike", };
        var dictionaryAdapter = new DictionaryAdapter<int, object>();
        var options = new JsonSerializerOptions();

        // Act
        var addStatus = dictionaryAdapter.TryAdd(dictionary, intKey.ToString(CultureInfo.InvariantCulture), options, "James", out var message);

        // Assert
        Assert.True(addStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.Single(dictionary);
        Assert.Equal("James", dictionary[intKey]);
    }

    [Fact]
    public void GetInvalidKey_ThrowsInvalidPathSegmentException()
    {
        // Arrange
        var dictionaryAdapter = new DictionaryAdapter<int, object>();
        var options = new JsonSerializerOptions();
        var key = 1;
        var dictionary = new Dictionary<int, object>();

        // Act
        var addStatus = dictionaryAdapter.TryAdd(dictionary, key.ToString(CultureInfo.InvariantCulture), options, "James", out var message);

        // Assert
        Assert.True(addStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.Single(dictionary);
        Assert.Equal("James", dictionary[key]);

        // Act
        var guidKey = new Guid();
        var getStatus = dictionaryAdapter.TryGet(dictionary, guidKey.ToString(), options, out var outValue, out message);

        // Assert
        Assert.False(getStatus);
        Assert.Equal($"The provided path segment '{guidKey}' cannot be converted to the target type.", message);
        Assert.Null(outValue);
    }

    [Fact]
    public void Get_UsingCaseSensitiveKey_FailureScenario()
    {
        // Arrange
        var dictionaryAdapter = new DictionaryAdapter<string, object>();
        var options = new JsonSerializerOptions();
        var nameKey = "Name";
        var dictionary = new Dictionary<string, object>(StringComparer.Ordinal);

        // Act
        var addStatus = dictionaryAdapter.TryAdd(dictionary, nameKey, options, "James", out var message);

        // Assert
        Assert.True(addStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.Single(dictionary);
        Assert.Equal("James", dictionary[nameKey]);

        // Act
        var getStatus = dictionaryAdapter.TryGet(dictionary, nameKey.ToUpperInvariant(), options, out var outValue, out message);

        // Assert
        Assert.False(getStatus);
        Assert.Equal("The target location specified by path segment 'NAME' was not found.", message);
        Assert.Null(outValue);
    }

    [Fact]
    public void Get_UsingCaseSensitiveKey_SuccessScenario()
    {
        // Arrange
        var dictionaryAdapter = new DictionaryAdapter<string, object>();
        var options = new JsonSerializerOptions();
        var nameKey = "Name";
        var dictionary = new Dictionary<string, object>(StringComparer.Ordinal);

        // Act
        var addStatus = dictionaryAdapter.TryAdd(dictionary, nameKey, options, "James", out var message);

        // Assert
        Assert.True(addStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.Single(dictionary);
        Assert.Equal("James", dictionary[nameKey]);

        // Act
        addStatus = dictionaryAdapter.TryGet(dictionary, nameKey, options, out var outValue, out message);

        // Assert
        Assert.True(addStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.Equal("James", outValue?.ToString());
    }

    [Fact]
    public void ReplacingExistingItem()
    {
        // Arrange
        var nameKey = "Name";
        var dictionary = new Dictionary<string, object>(StringComparer.Ordinal) { { nameKey, "Mike" }, };
        var dictionaryAdapter = new DictionaryAdapter<string, object>();
        var options = new JsonSerializerOptions();

        // Act
        var replaceStatus = dictionaryAdapter.TryReplace(dictionary, nameKey, options, "James", out var message);

        // Assert
        Assert.True(replaceStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.Single(dictionary);
        Assert.Equal("James", dictionary[nameKey]);
    }

    [Fact]
    public void ReplacingExistingItem_WithGuidKey()
    {
        // Arrange
        var guidKey = new Guid();
        var dictionary = new Dictionary<Guid, object> { { guidKey, "Mike" }, };
        var dictionaryAdapter = new DictionaryAdapter<Guid, object>();
        var options = new JsonSerializerOptions();

        // Act
        var replaceStatus = dictionaryAdapter.TryReplace(dictionary, guidKey.ToString(), options, "James", out var message);

        // Assert
        Assert.True(replaceStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.Single(dictionary);
        Assert.Equal("James", dictionary[guidKey]);
    }

    [Fact]
    public void ReplacingWithInvalidValue_ThrowsInvalidValueForPropertyException()
    {
        // Arrange
        var guidKey = new Guid();
        var dictionary = new Dictionary<Guid, int> { { guidKey, 5 }, };
        var dictionaryAdapter = new DictionaryAdapter<Guid, int>();
        var options = new JsonSerializerOptions();

        // Act
        var replaceStatus = dictionaryAdapter.TryReplace(dictionary, guidKey.ToString(), options, "test", out var message);

        // Assert
        Assert.False(replaceStatus);
        Assert.Equal("The value 'test' is invalid for target location.", message);
        Assert.Equal(5, dictionary[guidKey]);
    }

    [Fact]
    public void Replace_NonExistingKey_Fails()
    {
        // Arrange
        var nameKey = "Name";
        var dictionary = new Dictionary<string, object>(StringComparer.Ordinal);
        var dictionaryAdapter = new DictionaryAdapter<string, object>();
        var options = new JsonSerializerOptions();

        // Act
        var replaceStatus = dictionaryAdapter.TryReplace(dictionary, nameKey, options, "Mike", out var message);

        // Assert
        Assert.False(replaceStatus);
        Assert.Equal("The target location specified by path segment 'Name' was not found.", message);
        Assert.Empty(dictionary);
    }

    [Fact]
    public void Remove_NonExistingKey_Fails()
    {
        // Arrange
        var nameKey = "Name";
        var dictionary = new Dictionary<string, object>(StringComparer.Ordinal);
        var dictionaryAdapter = new DictionaryAdapter<string, object>();
        var options = new JsonSerializerOptions();

        // Act
        var removeStatus = dictionaryAdapter.TryRemove(dictionary, nameKey, options, out var message);

        // Assert
        Assert.False(removeStatus);
        Assert.Equal("The target location specified by path segment 'Name' was not found.", message);
        Assert.Empty(dictionary);
    }

    [Fact]
    public void Remove_RemovesFromDictionary()
    {
        // Arrange
        var nameKey = "Name";
        var dictionary = new Dictionary<string, object>(StringComparer.Ordinal) { [nameKey] = "James", };
        var dictionaryAdapter = new DictionaryAdapter<string, object>();
        var options = new JsonSerializerOptions();

        // Act
        var removeStatus = dictionaryAdapter.TryRemove(dictionary, nameKey, options, out var message);

        //Assert
        Assert.True(removeStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.Empty(dictionary);
    }

    [Fact]
    public void Remove_RemovesFromDictionary_WithUriKey()
    {
        // Arrange
        var uriKey = new Uri("http://www.test.com/name");
        var dictionary = new Dictionary<Uri, object> { [uriKey] = "James", };
        var dictionaryAdapter = new DictionaryAdapter<Uri, object>();
        var options = new JsonSerializerOptions();

        // Act
        var removeStatus = dictionaryAdapter.TryRemove(dictionary, uriKey.ToString(), options, out var message);

        //Assert
        Assert.True(removeStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
        Assert.Empty(dictionary);
    }

    [Fact]
    public void Test_DoesNotThrowException_IfTestIsSuccessful()
    {
        // Arrange
        var key = "Name";
        var dictionary = new Dictionary<string, List<object>>();
        var value = new List<object>()
        {
            "James",
            2,
            new Customer("James", 25)
        };
        dictionary[key] = value;
        var dictionaryAdapter = new DictionaryAdapter<string, List<object>>();
        var options = new JsonSerializerOptions();

        // Act
        var testStatus = dictionaryAdapter.TryTest(dictionary, key, options, value, out var message);

        //Assert
        Assert.True(testStatus);
        Assert.True(string.IsNullOrEmpty(message), "Expected no error message");
    }

    [Fact]
    public void Test_ThrowsJsonPatchException_IfTestFails()
    {
        // Arrange
        var key = "Name";
        var dictionary = new Dictionary<string, object> { [key] = "James", };
        var dictionaryAdapter = new DictionaryAdapter<string, object>();
        var options = new JsonSerializerOptions();
        var expectedErrorMessage = "The current value 'James' at path 'Name' is not equal to the test value 'John'.";

        // Act
        var testStatus = dictionaryAdapter.TryTest(dictionary, key, options, "John", out var errorMessage);

        //Assert
        Assert.False(testStatus);
        Assert.Equal(expectedErrorMessage, errorMessage);
    }
}
