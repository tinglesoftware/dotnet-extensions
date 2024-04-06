using System.Linq.Expressions;

#nullable disable

namespace Tingle.AspNetCore.JsonPatch.Properties;

internal partial class Resources
{
    public static string FormatTargetLocationAtPathSegmentNotFound(string segment)
        => string.Format(TargetLocationAtPathSegmentNotFound, segment);

    public static string FormatValueForTargetSegmentCannotBeNullOrEmpty(string segment)
        => string.Format(ValueForTargetSegmentCannotBeNullOrEmpty, segment);

    public static string FormatValueNotEqualToTestValue(object currentValue, object value, string segment)
        => string.Format(ValueNotEqualToTestValue, currentValue, value, segment);

    public static string FormatCannotCopyProperty(string propertyName)
        => string.Format(CannotCopyProperty, propertyName);

    public static string FormatCannotPerformOperation(string operation, string path)
        => string.Format(CannotPerformOperation, operation, path);

    public static string FormatCannotReadProperty(object propertyName)
        => string.Format(CannotReadProperty, propertyName);

    public static string FormatCannotUpdateProperty(object propertyName)
        => string.Format(CannotUpdateProperty, propertyName);

    public static string FormatTargetLocationNotFound(string operation, string path)
        => string.Format(TargetLocationNotFound, operation, path);

    public static string FormatInvalidPathSegment(string path)
        => string.Format(InvalidPathSegment, path);

    public static string FormatInvalidValueForPath(string path)
        => string.Format(InvalidValueForPath, path);

    public static string FormatInvalidIndexValue(string segment)
        => string.Format(InvalidIndexValue, segment);

    public static string FormatIndexOutOfBounds(string segment)
        => string.Format(IndexOutOfBounds, segment);

    public static string FormatValueAtListPositionNotEqualToTestValue(object currentValue, object value, int position)
        => string.Format(ValueAtListPositionNotEqualToTestValue, currentValue, value, position);

    public static string FormatInvalidValueForProperty(object value)
        => string.Format(InvalidValueForProperty, value);

    public static string FormatPatchNotSupportedForArrays(string name)
        => string.Format(PatchNotSupportedForArrays, name);

    public static string FormatPatchNotSupportedForNonGenericLists(string name)
        => string.Format(PatchNotSupportedForNonGenericLists, name);

    public static string FormatExpressionTypeNotSupported(Expression expr)
        => string.Format(ExpressionTypeNotSupported, expr);

    public static string FormatInvalidJsonPatchOperation(string path)
        => string.Format(InvalidJsonPatchOperation, path);
}
