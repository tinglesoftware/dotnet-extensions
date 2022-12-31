using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Linq.Expressions;

namespace Microsoft.AspNetCore.JsonPatch;

/// <summary>
/// Extensions for <see cref="JsonPatchDocument{T}"/> to support <see cref="IDictionary{TKey, TValue}"/>
/// </summary>
public static class JsonPatchDocumentDictionaryExtensions
{
    /// <summary>
    /// Add value to the end of the dictionary
    /// </summary>
    /// <typeparam name="TModel">model type</typeparam>
    /// <typeparam name="TKey">key type</typeparam>
    /// <typeparam name="TValue">value type</typeparam>
    /// <param name="document">The document to the add the operation to</param>
    /// <param name="path">target location</param>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    /// <returns></returns>
    public static JsonPatchDocument<TModel> Add<TModel, TKey, TValue>(this JsonPatchDocument<TModel> document,
                                                                      Expression<Func<TModel, IDictionary<TKey, TValue>>> path,
                                                                      TKey key,
                                                                      TValue value)
        where TModel : class
        where TKey : IConvertible
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        document.Operations.Add(
            new Operation<TModel>(
                op: "add",
                path: GetPath(document.ContractResolver, path, Convert.ToString(key)),
                from: null,
                value: value));

        return document;
    }

    /// <summary>
    /// Replace value in a dictionary
    /// </summary>
    /// <typeparam name="TModel">model type</typeparam>
    /// <typeparam name="TKey">key type</typeparam>
    /// <typeparam name="TValue">value type</typeparam>
    /// <param name="document">The document to the add the operation to</param>
    /// <param name="path">target location</param>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    /// <returns></returns>
    public static JsonPatchDocument<TModel> Replace<TModel, TKey, TValue>(this JsonPatchDocument<TModel> document,
                                                                          Expression<Func<TModel, IDictionary<TKey, TValue>>> path,
                                                                          TKey key,
                                                                          TValue value)
        where TModel : class
        where TKey : IConvertible
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        document.Operations.Add(
            new Operation<TModel>(
                op: "replace",
                path: GetPath(document.ContractResolver, path, Convert.ToString(key)),
                from: null,
                value: value));

        return document;
    }

    /// <summary>
    /// Remove value in a dictionary
    /// </summary>
    /// <typeparam name="TModel">model type</typeparam>
    /// <typeparam name="TKey">key type</typeparam>
    /// <typeparam name="TValue">value type</typeparam>
    /// <param name="document">The document to the add the operation to</param>
    /// <param name="path">target location</param>
    /// <param name="key">key</param>
    /// <returns></returns>
    public static JsonPatchDocument<TModel> Remove<TModel, TKey, TValue>(this JsonPatchDocument<TModel> document,
                                                                         Expression<Func<TModel, IDictionary<TKey, TValue>>> path,
                                                                         TKey key)
        where TModel : class
        where TKey : IConvertible
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        document.Operations.Add(
            new Operation<TModel>(
                op: "remove",
                path: GetPath(document.ContractResolver, path, Convert.ToString(key)),
                from: null));

        return document;
    }

    private static string GetPath<TModel, TProp>(IContractResolver contractResolver,
                                                 Expression<Func<TModel, TProp>> expr,
                                                 string? position)
    {
        var segments = GetPathSegments(contractResolver, expr.Body);
        var path = string.Join("/", segments);
        if (position != null)
        {
            path += "/" + position;
            if (segments.Count == 0)
            {
                return path;
            }
        }

        return "/" + path;
    }

    private static List<string?> GetPathSegments(IContractResolver contractResolver, Expression expr)
    {
        var listOfSegments = new List<string?>();
        switch (expr.NodeType)
        {
            case ExpressionType.ArrayIndex:
                var binaryExpression = (BinaryExpression)expr;
                listOfSegments.AddRange(GetPathSegments(contractResolver, binaryExpression.Left));
                listOfSegments.Add(binaryExpression.Right.ToString());
                return listOfSegments;

            case ExpressionType.Call:
                var methodCallExpression = (MethodCallExpression)expr;
                listOfSegments.AddRange(GetPathSegments(contractResolver, methodCallExpression.Object!));
                listOfSegments.Add(EvaluateExpression(methodCallExpression.Arguments[0]));
                return listOfSegments;

            case ExpressionType.Convert:
                listOfSegments.AddRange(GetPathSegments(contractResolver, ((UnaryExpression)expr).Operand));
                return listOfSegments;

            case ExpressionType.MemberAccess:
                var memberExpression = (MemberExpression)expr;
                listOfSegments.AddRange(GetPathSegments(contractResolver, memberExpression.Expression!));
                // Get property name, respecting JsonProperty attribute
                listOfSegments.Add(GetPropertyNameFromMemberExpression(contractResolver, memberExpression));
                return listOfSegments;

            case ExpressionType.Parameter:
                // Fits "x => x" (the whole document which is "" as JSON pointer)
                return listOfSegments;

            default:
                throw new InvalidOperationException($"The expression '{expr}' is not supported");
        }
    }

    private static string? GetPropertyNameFromMemberExpression(IContractResolver contractResolver, MemberExpression memberExpression)
    {
        if (contractResolver.ResolveContract(memberExpression.Expression!.Type) is JsonObjectContract contract)
        {
            return contract.Properties.First(jp => jp.UnderlyingName == memberExpression.Member.Name).PropertyName!;
        }

        return null;
    }

    // Evaluates the value of the key or index which may be an int or a string,
    // or some other expression type.
    // The expression is converted to a delegate and the result of executing the delegate is returned as a string.
    private static string? EvaluateExpression(Expression expression)
    {
        var converted = Expression.Convert(expression, typeof(object));
        var fakeParameter = Expression.Parameter(typeof(object), null);
        var lambda = Expression.Lambda<Func<object?, object?>>(converted, fakeParameter);
        var func = lambda.Compile();

        return Convert.ToString(func(null), CultureInfo.InvariantCulture);
    }
}
