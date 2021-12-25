using System.ComponentModel.DataAnnotations;
using Tingle.Extensions.DataAnnotations.Tests.Models;

namespace Tingle.Extensions.DataAnnotations.Tests;

public class RecursiveValidatorTests
{
    public RecursiveValidatorTests()
    {
        SaveValidationContextAttribute.SavedContexts.Clear();
    }

    [Fact]
    public void TryValidateObject_on_valid_parent_returns_no_errors()
    {
        var parent = new Parent { PropertyA = 1, PropertyB = 1 };
        var validationResults = new List<ValidationResult>();

        var result = RecursiveValidator.TryValidateObject(parent, validationResults);

        Assert.True(result);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void TryValidateObject_when_missing_required_properties_returns_errors()
    {
        var parent = new Parent { PropertyA = null, PropertyB = null };
        var validationResults = new List<ValidationResult>();

        var result = RecursiveValidator.TryValidateObject(parent, validationResults);

        Assert.False(result);
        Assert.Equal(2, validationResults.Count);
        Assert.Equal(1, validationResults.ToList().Count(x => x.ErrorMessage == "Parent PropertyA is required"));
        Assert.Equal(1, validationResults.ToList().Count(x => x.ErrorMessage == "Parent PropertyB is required"));
    }

    [Fact]
    public void TryValidateObject_calls_IValidatableObject_method()
    {
        var parent = new Parent { PropertyA = 5, PropertyB = 6 };
        var validationResults = new List<ValidationResult>();

        var result = RecursiveValidator.TryValidateObject(parent, validationResults);

        Assert.False(result);
        Assert.Single(validationResults);
        Assert.Equal("Parent PropertyA and PropertyB cannot add up to more than 10", validationResults[0].ErrorMessage);
    }

    [Fact]
    public void TryValidateObjectRecursive_returns_errors_when_child_class_has_invalid_properties()
    {
        var parent = new Parent { PropertyA = 1, PropertyB = 1 };
        parent.Child = new Child { Parent = parent, PropertyA = null, PropertyB = 5 };
        var validationResults = new List<ValidationResult>();

        var result = RecursiveValidator.TryValidateObjectRecursive(parent, validationResults);

        Assert.False(result);
        Assert.Single(validationResults);
        Assert.Equal("Child PropertyA is required", validationResults[0].ErrorMessage);
    }

    [Fact]
    public void TryValidateObjectRecursive_ignored_errors_when_child_class_has_SkipRecursiveValidationProperty()
    {
        var parent = new Parent { PropertyA = 1, PropertyB = 1 };
        parent.Child = new Child { Parent = parent, PropertyA = 1, PropertyB = 1 };
        parent.SkippedChild = new Child { PropertyA = null, PropertyB = 1 };
        var validationResults = new List<ValidationResult>();

        var result = RecursiveValidator.TryValidateObjectRecursive(parent, validationResults);

        Assert.True(result);
    }

    [Fact]
    public void TryValidateObjectRecursive_calls_IValidatableObject_method_on_child_class()
    {
        var parent = new Parent { PropertyA = 1, PropertyB = 1 };
        parent.Child = new Child { Parent = parent, PropertyA = 5, PropertyB = 6 };
        var validationResults = new List<ValidationResult>();

        var result = RecursiveValidator.TryValidateObjectRecursive(parent, validationResults);

        Assert.False(result);
        Assert.Single(validationResults);
        Assert.Equal("Child PropertyA and PropertyB cannot add up to more than 10", validationResults[0].ErrorMessage);
    }

    [Fact]
    public void TryValidateObjectRecursive_returns_errors_when_grandchild_class_has_invalid_properties()
    {
        var parent = new Parent { PropertyA = 1, PropertyB = 1 };
        parent.Child = new Child { Parent = parent, PropertyA = 1, PropertyB = 1 };
        parent.Child.GrandChildren = new[] { new GrandChild { PropertyA = 11, PropertyB = 11 } };
        var validationResults = new List<ValidationResult>();

        var result = RecursiveValidator.TryValidateObjectRecursive(parent, validationResults);

        Assert.False(result);
        Assert.Equal(2, validationResults.Count);
        Assert.Equal(1, validationResults.ToList().Count(x => x.ErrorMessage == "GrandChild PropertyA not within range"));
        Assert.Equal(1, validationResults.ToList().Count(x => x.ErrorMessage == "GrandChild PropertyB not within range"));
    }

    [Fact]
    public void TryValidateObjectRecursive_passes_validation_context_items_to_all_validation_calls()
    {
        var parent = new Parent
        {
            Child = new Child
            {
                GrandChildren = new[] { new GrandChild() }
            }
        };
        var validationResults = new List<ValidationResult>();

        var contextItems = new Dictionary<object, object?> { { "key", 12345 } };

        RecursiveValidator.TryValidateObjectRecursive(parent, validationResults, contextItems);

        Assert.Equal(3, SaveValidationContextAttribute.SavedContexts.Count);
        Assert.All(SaveValidationContextAttribute.SavedContexts.Select(c => c.Items), items =>
        {
            var v = Assert.Contains("key", items);
            Assert.Equal(contextItems["key"], v);
        });
    }

    [Fact]
    public void TryValidateObject_calls_grandchild_IValidatableObject_method()
    {
        var parent = new Parent { PropertyA = 1, PropertyB = 1 };
        parent.Child = new Child { Parent = parent, PropertyA = 1, PropertyB = 1 };
        parent.Child.GrandChildren = new[] { new GrandChild { PropertyA = 5, PropertyB = 6 } };
        var validationResults = new List<ValidationResult>();

        var result = RecursiveValidator.TryValidateObjectRecursive(parent, validationResults);

        Assert.False(result);
        Assert.Single(validationResults);
        Assert.Equal(1, validationResults.ToList().Count(x => x.ErrorMessage == "GrandChild PropertyA and PropertyB cannot add up to more than 10"));
    }

    [Fact]
    public void TryValidateObject_includes_errors_from_all_objects()
    {
        var parent = new Parent { PropertyA = 5, PropertyB = 6 };
        parent.Child = new Child { Parent = parent, PropertyA = 5, PropertyB = 6 };
        parent.Child.GrandChildren = new[] { new GrandChild { PropertyA = 5, PropertyB = 6 } };
        var validationResults = new List<ValidationResult>();

        var result = RecursiveValidator.TryValidateObjectRecursive(parent, validationResults);

        Assert.False(result);
        Assert.Equal(3, validationResults.Count);
        Assert.Equal(1, validationResults.ToList().Count(x => x.ErrorMessage == "Parent PropertyA and PropertyB cannot add up to more than 10"));
        Assert.Equal(1, validationResults.ToList().Count(x => x.ErrorMessage == "Child PropertyA and PropertyB cannot add up to more than 10"));
        Assert.Equal(1, validationResults.ToList().Count(x => x.ErrorMessage == "GrandChild PropertyA and PropertyB cannot add up to more than 10"));
    }

    [Fact]
    public void TryValidateObject_modifies_membernames_for_nested_properties()
    {
        var parent = new Parent { PropertyA = 1, PropertyB = 1 };
        parent.Child = new Child { Parent = parent, PropertyA = null, PropertyB = 5 };
        var validationResults = new List<ValidationResult>();

        var result = RecursiveValidator.TryValidateObjectRecursive(parent, validationResults);

        Assert.False(result);
        Assert.Single(validationResults);
        Assert.Equal("Child PropertyA is required", validationResults[0].ErrorMessage);
        Assert.Equal("Child.PropertyA", validationResults[0].MemberNames.First());
    }

    [Fact]
    public void TryValidateObject_object_with_dictionary_does_not_fail()
    {
        var parent = new Parent { PropertyA = 1, PropertyB = 1 };
        var classWithDictionary = new ClassWithDictionary
        {
            Objects = new List<Dictionary<string, Child>>
                {
                    new Dictionary<string, Child>
                    {
                        { "key",
                            new Child
                            {
                                Parent = parent,
                                PropertyA = 1,
                                PropertyB = 2
                            }
                        }
                    }
                }
        };
        var validationResults = new List<ValidationResult>();

        var result = RecursiveValidator.TryValidateObjectRecursive(classWithDictionary, validationResults);

        Assert.True(result);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void TryValidateObject_object_with_null_enumeration_values_does_not_fail()
    {
        var parent = new Parent { PropertyA = 1, PropertyB = 1 };
        var classWithNullableEnumeration = new ClassWithNullableEnumeration
        {
            Objects = new List<Child?>
                {
                    null,
                    new Child
                    {
                        Parent = parent,
                        PropertyA = 1,
                        PropertyB = 2
                    }
                }
        };
        var validationResults = new List<ValidationResult>();

        var result = RecursiveValidator.TryValidateObjectRecursive(classWithNullableEnumeration, validationResults);

        Assert.True(result);
        Assert.Empty(validationResults);
    }

}
