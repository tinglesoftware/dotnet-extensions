﻿using System.Text.Json;
using Tingle.AspNetCore.JsonPatch.Exceptions;

namespace Tingle.AspNetCore.JsonPatch;

public class JsonPatchDocumentTest
{
    [Fact]
    public void InvalidPathAtBeginningShouldThrowException()
    {
        // Arrange
        var patchDocument = new JsonPatchDocument();

        // Act
        var exception = Assert.Throws<JsonPatchException>(() =>
        {
            patchDocument.Add("//NewInt", 1);
        });

        // Assert
        Assert.Equal(
           "The provided string '//NewInt' is an invalid path.",
            exception.Message);
    }

    [Fact]
    public void InvalidPathAtEndShouldThrowException()
    {
        // Arrange
        var patchDocument = new JsonPatchDocument();

        // Act
        var exception = Assert.Throws<JsonPatchException>(() =>
        {
            patchDocument.Add("NewInt//", 1);
        });

        // Assert
        Assert.Equal(
           "The provided string 'NewInt//' is an invalid path.",
            exception.Message);
    }

    [Fact]
    public void NonGenericPatchDocToGenericMustSerialize()
    {
        // Arrange
        var targetObject = new SimpleObject()
        {
            StringProperty = "A",
            AnotherStringProperty = "B"
        };

        var patchDocument = new JsonPatchDocument();
        patchDocument.Copy("StringProperty", "AnotherStringProperty");

        var serialized = JsonSerializer.Serialize(patchDocument);
        var deserialized = JsonSerializer.Deserialize<JsonPatchDocument<SimpleObject>>(serialized)!;

        // Act
        deserialized.ApplyTo(targetObject);

        // Assert
        Assert.Equal("A", targetObject.AnotherStringProperty);
    }

    [Fact]
    public void GenericPatchDocToNonGenericMustSerialize()
    {
        // Arrange
        var targetObject = new SimpleObject()
        {
            StringProperty = "A",
            AnotherStringProperty = "B"
        };

        var patchDocTyped = new JsonPatchDocument<SimpleObject>();
        patchDocTyped.Copy(o => o.StringProperty, o => o.AnotherStringProperty);

        var patchDocUntyped = new JsonPatchDocument();
        patchDocUntyped.Copy("StringProperty", "AnotherStringProperty");

        var serializedTyped = JsonSerializer.Serialize(patchDocTyped);
        var serializedUntyped = JsonSerializer.Serialize(patchDocUntyped);
        var deserialized = JsonSerializer.Deserialize<JsonPatchDocument>(serializedTyped)!;

        // Act
        deserialized.ApplyTo(targetObject);

        // Assert
        Assert.Equal("A", targetObject.AnotherStringProperty);
    }

    [Fact]
    public void Deserialization_Successful_ForValidJsonPatchDocument()
    {
        // Arrange
        var doc = new SimpleObject()
        {
            StringProperty = "A",
            DecimalValue = 10,
            DoubleValue = 10,
            FloatValue = 10,
            IntegerValue = 10
        };

        var patchDocument = new JsonPatchDocument<SimpleObject>();
        patchDocument.Replace(o => o.StringProperty, "B");
        patchDocument.Replace(o => o.DecimalValue, 12);
        patchDocument.Replace(o => o.DoubleValue, 12);
        patchDocument.Replace(o => o.FloatValue, 12);
        patchDocument.Replace(o => o.IntegerValue, 12);

        // default: no envelope
        var serialized = JsonSerializer.Serialize(patchDocument);

        // Act
        var deserialized = JsonSerializer.Deserialize<JsonPatchDocument<SimpleObject>>(serialized);

        // Assert
        Assert.IsType<JsonPatchDocument<SimpleObject>>(deserialized);
    }

    [Fact]
    public void Deserialization_Fails_ForInvalidJsonPatchDocument()
    {
        // Arrange
        var serialized = "{\"Operations\": [{ \"op\": \"replace\", \"path\": \"/title\", \"value\": \"New Title\"}]}";

        // Act
        var exception = Assert.Throws<JsonException>(() =>
        {
            var deserialized
                = JsonSerializer.Deserialize<JsonPatchDocument>(serialized);
        });

        // Assert
        Assert.StartsWith("The JSON value could not be converted to ", exception.Message);
    }

    [Fact]
    public void Deserialization_Fails_ForInvalidTypedJsonPatchDocument()
    {
        // Arrange
        var serialized = "{\"Operations\": [{ \"op\": \"replace\", \"path\": \"/title\", \"value\": \"New Title\"}]}";

        // Act
        var exception = Assert.Throws<JsonException>(() =>
        {
            var deserialized
                = JsonSerializer.Deserialize<JsonPatchDocument<SimpleObject>>(serialized);
        });

        // Assert
        Assert.StartsWith("The JSON value could not be converted to ", exception.Message);
    }
}
