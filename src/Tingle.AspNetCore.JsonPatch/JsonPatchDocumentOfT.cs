using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tingle.AspNetCore.JsonPatch.Adapters;
using Tingle.AspNetCore.JsonPatch.Converters;
using Tingle.AspNetCore.JsonPatch.Exceptions;
using Tingle.AspNetCore.JsonPatch.Internal;
using Tingle.AspNetCore.JsonPatch.Operations;
using Tingle.AspNetCore.JsonPatch.Properties;

namespace Tingle.AspNetCore.JsonPatch;

[JsonConverter(typeof(TypedJsonPatchDocumentConverter))]
public class JsonPatchDocument<TModel>(List<Operation<TModel>> operations, JsonSerializerOptions serializerOptions) : IJsonPatchDocument where TModel : class
{
    public List<Operation<TModel>> Operations { get; private set; } = operations ?? throw new ArgumentNullException(nameof(operations));

    [JsonIgnore]
    public JsonSerializerOptions SerializerOptions { get; set; } = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));

    public JsonPatchDocument() : this([]) { }

    public JsonPatchDocument(List<Operation<TModel>> operations) : this(operations, new()) { }

    public JsonPatchDocument(JsonSerializerOptions serializerOptions) : this([], serializerOptions) { }

    /// <summary>
    /// Add operation.  Will result in, for example,
    /// { "op": "add", "path": "/a/b/c", "value": [ "foo", "bar" ] }
    /// </summary>
    /// <typeparam name="TProp">value type</typeparam>
    /// <param name="path">target location</param>
    /// <param name="value">value</param>
    /// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
    public JsonPatchDocument<TModel> Add<TProp>(Expression<Func<TModel, TProp>> path, TProp value)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "add",
            GetPath(path, null),
            from: null,
            value: value));

        return this;
    }

    /// <summary>
    /// Add value to list at given position
    /// </summary>
    /// <typeparam name="TProp">value type</typeparam>
    /// <param name="path">target location</param>
    /// <param name="value">value</param>
    /// <param name="position">position</param>
    /// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
    public JsonPatchDocument<TModel> Add<TProp>(
        Expression<Func<TModel, IList<TProp>?>> path,
        TProp value,
        int position)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "add",
            GetPath(path, position.ToString(CultureInfo.InvariantCulture)),
            from: null,
            value: value));

        return this;
    }

    /// <summary>
    /// Add value to the end of the list
    /// </summary>
    /// <typeparam name="TProp">value type</typeparam>
    /// <param name="path">target location</param>
    /// <param name="value">value</param>
    /// <returns>The <see cref="JsonPatchDocument{TModel}"/> for chaining.</returns>
    public JsonPatchDocument<TModel> Add<TProp>(Expression<Func<TModel, IList<TProp>?>> path, TProp value)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "add",
            GetPath(path, "-"),
            from: null,
            value: value));

        return this;
    }
    /// <summary>
    /// Add operation. Will result in, for example,
    /// { "op": "add", "path": "/a/b/c", "value": [ "foo", "bar" ] }
    /// </summary>
    /// <typeparam name="TKey">key type</typeparam>
    /// <typeparam name="TValue">value type</typeparam>
    /// <param name="path">target location</param>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Add<TKey, TValue>(Expression<Func<TModel, IDictionary<TKey, TValue>?>> path,
                                                       TKey key,
                                                       TValue value)
        where TKey : IConvertible
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            op: "add",
            path: GetPath(path, Convert.ToString(key)),
            from: null,
            value: value));

        return this;
    }

    /// <summary>
    /// Remove value at target location.  Will result in, for example,
    /// { "op": "remove", "path": "/a/b/c" }
    /// </summary>
    /// <param name="path">target location</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Remove<TProp>(Expression<Func<TModel, TProp>> path)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>("remove", GetPath(path, null), from: null));

        return this;
    }

    /// <summary>
    /// Remove value from list at given position
    /// </summary>
    /// <typeparam name="TProp">value type</typeparam>
    /// <param name="path">target location</param>
    /// <param name="position">position</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Remove<TProp>(Expression<Func<TModel, IList<TProp>?>> path, int position)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "remove",
            GetPath(path, position.ToString()),
            from: null));

        return this;
    }

    /// <summary>
    /// Remove value from end of list
    /// </summary>
    /// <typeparam name="TProp">value type</typeparam>
    /// <param name="path">target location</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Remove<TProp>(Expression<Func<TModel, IList<TProp>?>> path)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "remove",
            GetPath(path, "-"),
            from: null));

        return this;
    }

    /// <summary>
    /// Remove value at target location. Will result in, for example,
    /// { "op": "remove", "path": "/a/b/c" }
    /// </summary>
    /// <typeparam name="TKey">key type</typeparam>
    /// <typeparam name="TValue">value type</typeparam>
    /// <param name="path">target location</param>
    /// <param name="key">key</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Remove<TKey, TValue>(Expression<Func<TModel, IDictionary<TKey, TValue>?>> path, TKey key)
        where TKey : IConvertible
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            op: "remove",
            path: GetPath(path, Convert.ToString(key)),
            from: null));

        return this;
    }

    /// <summary>
    /// Replace value.  Will result in, for example,
    /// { "op": "replace", "path": "/a/b/c", "value": 42 }
    /// </summary>
    /// <param name="path">target location</param>
    /// <param name="value">value</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Replace<TProp>(Expression<Func<TModel, TProp>> path, TProp value)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "replace",
            GetPath(path, null),
            from: null,
            value: value));

        return this;
    }

    /// <summary>
    /// Replace value in a list at given position
    /// </summary>
    /// <typeparam name="TProp">value type</typeparam>
    /// <param name="path">target location</param>
    /// <param name="value">value</param>
    /// <param name="position">position</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Replace<TProp>(Expression<Func<TModel, IList<TProp>?>> path, TProp value, int position)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "replace",
            GetPath(path, position.ToString()),
            from: null,
            value: value));

        return this;
    }

    /// <summary>
    /// Replace value at end of a list
    /// </summary>
    /// <typeparam name="TProp">value type</typeparam>
    /// <param name="path">target location</param>
    /// <param name="value">value</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Replace<TProp>(Expression<Func<TModel, IList<TProp>?>> path, TProp value)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "replace",
            GetPath(path, "-"),
            from: null,
            value: value));

        return this;
    }

    /// <summary>
    /// Replace value. Will result in, for example,
    /// { "op": "replace", "path": "/a/b/c", "value": 42 }
    /// </summary>
    /// <typeparam name="TKey">key type</typeparam>
    /// <typeparam name="TValue">value type</typeparam>
    /// <param name="path">target location</param>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Replace<TKey, TValue>(Expression<Func<TModel, IDictionary<TKey, TValue>?>> path,
                                                           TKey key,
                                                           TValue value)
        where TKey : IConvertible
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            op: "replace",
            path: GetPath(path, Convert.ToString(key)),
            from: null,
            value: value));

        return this;
    }

    /// <summary>
    /// Test value.  Will result in, for example,
    /// { "op": "test", "path": "/a/b/c", "value": 42 }
    /// </summary>
    /// <param name="path">target location</param>
    /// <param name="value">value</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Test<TProp>(Expression<Func<TModel, TProp>> path, TProp value)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "test",
            GetPath(path, null),
            from: null,
            value: value));

        return this;
    }

    /// <summary>
    /// Test value in a list at given position
    /// </summary>
    /// <typeparam name="TProp">value type</typeparam>
    /// <param name="path">target location</param>
    /// <param name="value">value</param>
    /// <param name="position">position</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Test<TProp>(Expression<Func<TModel, IList<TProp>?>> path, TProp value, int position)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "test",
            GetPath(path, position.ToString()),
            from: null,
            value: value));

        return this;
    }

    /// <summary>
    /// Test value at end of a list
    /// </summary>
    /// <typeparam name="TProp">value type</typeparam>
    /// <param name="path">target location</param>
    /// <param name="value">value</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Test<TProp>(Expression<Func<TModel, IList<TProp>?>> path, TProp value)
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "test",
            GetPath(path, "-"),
            from: null,
            value: value));

        return this;
    }

    /// <summary>
    /// Test value. Will result in, for example,
    /// { "op": "test", "path": "/a/b/c", "value": 42 }
    /// </summary>
    /// <typeparam name="TKey">key type</typeparam>
    /// <typeparam name="TValue">value type</typeparam>
    /// <param name="path">target location</param>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Test<TKey, TValue>(Expression<Func<TModel, IDictionary<TKey, TValue>?>> path,
                                                        TKey key,
                                                        TValue value)
        where TKey : IConvertible
    {
        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            op: "test",
            path: GetPath(path, Convert.ToString(key)),
            from: null,
            value: value));

        return this;
    }

    /// <summary>
    /// Removes value at specified location and add it to the target location.  Will result in, for example:
    /// { "op": "move", "from": "/a/b/c", "path": "/a/b/d" }
    /// </summary>
    /// <param name="from">source location</param>
    /// <param name="path">target location</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, TProp>> from, Expression<Func<TModel, TProp>> path)
    {
        ArgumentNullException.ThrowIfNull(from);

        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "move",
            GetPath(path, null),
            GetPath(from, null)));

        return this;
    }

    /// <summary>
    /// Move from a position in a list to a new location
    /// </summary>
    /// <typeparam name="TProp"></typeparam>
    /// <param name="from">source location</param>
    /// <param name="positionFrom">position</param>
    /// <param name="path">target location</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, IList<TProp>?>> from,
                                                 int positionFrom,
                                                 Expression<Func<TModel, TProp>> path)
    {
        ArgumentNullException.ThrowIfNull(from);

        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "move",
            GetPath(path, null),
            GetPath(from, positionFrom.ToString())));

        return this;
    }

    /// <summary>
    /// Move from a property to a location in a list
    /// </summary>
    /// <typeparam name="TProp"></typeparam>
    /// <param name="from">source location</param>
    /// <param name="path">target location</param>
    /// <param name="positionTo">position</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, TProp>> from,
                                                 Expression<Func<TModel, IList<TProp>?>> path,
                                                 int positionTo)
    {
        ArgumentNullException.ThrowIfNull(from);

        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "move",
            GetPath(path, positionTo.ToString()),
            GetPath(from, null)));

        return this;
    }

    /// <summary>
    /// Move from a position in a list to another location in a list
    /// </summary>
    /// <typeparam name="TProp"></typeparam>
    /// <param name="from">source location</param>
    /// <param name="positionFrom">position (source)</param>
    /// <param name="path">target location</param>
    /// <param name="positionTo">position (target)</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, IList<TProp>?>> from,
                                                 int positionFrom,
                                                 Expression<Func<TModel, IList<TProp>?>> path,
                                                 int positionTo)
    {
        ArgumentNullException.ThrowIfNull(from);

        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "move",
            GetPath(path, positionTo.ToString()),
            GetPath(from, positionFrom.ToString())));

        return this;
    }

    /// <summary>
    /// Move from a position in a list to the end of another list
    /// </summary>
    /// <typeparam name="TProp"></typeparam>
    /// <param name="from">source location</param>
    /// <param name="positionFrom">position</param>
    /// <param name="path">target location</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, IList<TProp>?>> from,
                                                 int positionFrom,
                                                 Expression<Func<TModel, IList<TProp>?>> path)
    {
        ArgumentNullException.ThrowIfNull(from);

        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "move",
            GetPath(path, "-"),
            GetPath(from, positionFrom.ToString())));

        return this;
    }

    /// <summary>
    /// Move to the end of a list
    /// </summary>
    /// <typeparam name="TProp"></typeparam>
    /// <param name="from">source location</param>
    /// <param name="path">target location</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, TProp>> from, Expression<Func<TModel, IList<TProp>?>> path)
    {
        ArgumentNullException.ThrowIfNull(from);

        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "move",
            GetPath(path, "-"),
            GetPath(from, null)));

        return this;
    }

    /// <summary>
    /// Copy the value at specified location to the target location. Will esult in, for example:
    /// { "op": "copy", "from": "/a/b/c", "path": "/a/b/e" }
    /// </summary>
    /// <param name="from">source location</param>
    /// <param name="path">target location</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, TProp>> from, Expression<Func<TModel, TProp>> path)
    {
        ArgumentNullException.ThrowIfNull(from);

        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "copy",
            GetPath(path, null),
            GetPath(from, null)));

        return this;
    }

    /// <summary>
    /// Copy from a position in a list to a new location
    /// </summary>
    /// <typeparam name="TProp"></typeparam>
    /// <param name="from">source location</param>
    /// <param name="positionFrom">position</param>
    /// <param name="path">target location</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, IList<TProp>?>> from,
                                                 int positionFrom,
                                                 Expression<Func<TModel, TProp>> path)
    {
        ArgumentNullException.ThrowIfNull(from);

        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "copy",
            GetPath(path, null),
            GetPath(from, positionFrom.ToString())));

        return this;
    }

    /// <summary>
    /// Copy from a property to a location in a list
    /// </summary>
    /// <typeparam name="TProp"></typeparam>
    /// <param name="from">source location</param>
    /// <param name="path">target location</param>
    /// <param name="positionTo">position</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, TProp>> from,
                                                 Expression<Func<TModel, IList<TProp>?>> path,
                                                 int positionTo)
    {
        ArgumentNullException.ThrowIfNull(from);

        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "copy",
            GetPath(path, positionTo.ToString()),
            GetPath(from, null)));

        return this;
    }

    /// <summary>
    /// Copy from a position in a list to a new location in a list
    /// </summary>
    /// <typeparam name="TProp"></typeparam>
    /// <param name="from">source location</param>
    /// <param name="positionFrom">position (source)</param>
    /// <param name="path">target location</param>
    /// <param name="positionTo">position (target)</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, IList<TProp>?>> from,
                                                 int positionFrom,
                                                 Expression<Func<TModel, IList<TProp>?>> path,
                                                 int positionTo)
    {
        ArgumentNullException.ThrowIfNull(from);

        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "copy",
            GetPath(path, positionTo.ToString()),
            GetPath(from, positionFrom.ToString())));

        return this;
    }

    /// <summary>
    /// Copy from a position in a list to the end of another list
    /// </summary>
    /// <typeparam name="TProp"></typeparam>
    /// <param name="from">source location</param>
    /// <param name="positionFrom">position</param>
    /// <param name="path">target location</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, IList<TProp>?>> from,
                                                 int positionFrom,
                                                 Expression<Func<TModel, IList<TProp>?>> path)
    {
        ArgumentNullException.ThrowIfNull(from);

        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "copy",
            GetPath(path, "-"),
            GetPath(from, positionFrom.ToString())));

        return this;
    }

    /// <summary>
    /// Copy to the end of a list
    /// </summary>
    /// <typeparam name="TProp"></typeparam>
    /// <param name="from">source location</param>
    /// <param name="path">target location</param>
    /// <returns></returns>
    public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, TProp>> from, Expression<Func<TModel, IList<TProp>?>> path)
    {
        ArgumentNullException.ThrowIfNull(from);

        ArgumentNullException.ThrowIfNull(path);

        Operations.Add(new Operation<TModel>(
            "copy",
            GetPath(path, "-"),
            GetPath(from, null)));

        return this;
    }

    /// <summary>
    /// Apply this JsonPatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
    public void ApplyTo(TModel objectToApplyTo)
    {
        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        ApplyTo(objectToApplyTo, new ObjectAdapter(SerializerOptions, null, AdapterFactory.Default, create: false));
    }

    /// <summary>
    /// Apply this JsonPatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
    /// <param name="logErrorAction">Action to log errors</param>
    public void ApplyTo(TModel objectToApplyTo, Action<JsonPatchError> logErrorAction)
    {
        ApplyTo(objectToApplyTo, new ObjectAdapter(SerializerOptions, logErrorAction, AdapterFactory.Default, create: false), logErrorAction);
    }

    /// <summary>
    /// Apply this JsonPatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
    /// <param name="adapter">IObjectAdapter instance to use when applying</param>
    /// <param name="logErrorAction">Action to log errors</param>
    public void ApplyTo(TModel objectToApplyTo, IObjectAdapter adapter, Action<JsonPatchError> logErrorAction)
    {
        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        ArgumentNullException.ThrowIfNull(adapter);

        foreach (var op in Operations)
        {
            try
            {
                op.Apply(objectToApplyTo, adapter);
            }
            catch (JsonPatchException jsonPatchException)
            {
                var errorReporter = logErrorAction ?? ErrorReporter.Default;
                errorReporter(new JsonPatchError(objectToApplyTo, op, jsonPatchException.Message));

                // As per JSON Patch spec if an operation results in error, further operations should not be executed.
                break;
            }
        }
    }

    /// <summary>
    /// Apply this JsonPatchDocument
    /// </summary>
    /// <param name="objectToApplyTo">Object to apply the JsonPatchDocument to</param>
    /// <param name="adapter">IObjectAdapter instance to use when applying</param>
    public void ApplyTo(TModel objectToApplyTo, IObjectAdapter adapter)
    {
        ArgumentNullException.ThrowIfNull(objectToApplyTo);

        ArgumentNullException.ThrowIfNull(adapter);

        // apply each operation in order
        foreach (var op in Operations)
        {
            op.Apply(objectToApplyTo, adapter);
        }
    }

    IList<Operation> IJsonPatchDocument.GetOperations()
    {
        var allOps = new List<Operation>();

        if (Operations != null)
        {
            foreach (var op in Operations)
            {
                var untypedOp = new Operation
                {
                    op = op.op,
                    value = op.value,
                    path = op.path,
                    from = op.from
                };

                allOps.Add(untypedOp);
            }
        }

        return allOps;
    }

    // Internal for testing
    internal string GetPath<TProp>(Expression<Func<TModel, TProp>> expr, string? position)
    {
        var segments = GetPathSegments(expr.Body);
        var path = string.Join("/", segments);
        if (position != null)
        {
            path += "/" + (SerializerOptions.DictionaryKeyPolicy?.ConvertName(position) ?? position);
            if (segments.Count == 0)
            {
                return path;
            }
        }

        return "/" + path;
    }

    private List<string> GetPathSegments(Expression? expr)
    {
        if (expr is null) return [];
        var listOfSegments = new List<string>();
        switch (expr.NodeType)
        {
            case ExpressionType.ArrayIndex:
                var binaryExpression = (BinaryExpression)expr;
                listOfSegments.AddRange(GetPathSegments(binaryExpression.Left));
                listOfSegments.Add(binaryExpression.Right.ToString());
                return listOfSegments;

            case ExpressionType.Call:
                var methodCallExpression = (MethodCallExpression)expr;
                listOfSegments.AddRange(GetPathSegments(methodCallExpression.Object));
                listOfSegments.Add(EvaluateExpression(methodCallExpression.Arguments[0]));
                return listOfSegments;

            case ExpressionType.Convert:
                listOfSegments.AddRange(GetPathSegments(((UnaryExpression)expr).Operand));
                return listOfSegments;

            case ExpressionType.MemberAccess:
                var memberExpression = (MemberExpression)expr;
                listOfSegments.AddRange(GetPathSegments(memberExpression.Expression));
                // Get property name, respecting JsonPropertyName attribute
                listOfSegments.Add(GetPropertyNameFromMemberExpression(SerializerOptions, memberExpression));
                return listOfSegments;

            case ExpressionType.Parameter:
                // Fits "x => x" (the whole document which is "" as JSON pointer)
                return listOfSegments;

            default:
                throw new InvalidOperationException(Resources.FormatExpressionTypeNotSupported(expr));
        }
    }

    private static string GetPropertyNameFromMemberExpression(JsonSerializerOptions serializerOptions, MemberExpression memberExpression)
    {
        var propertyInfo = memberExpression.Expression!.Type.GetProperty(memberExpression.Member.Name);
        var targetAttr = propertyInfo?.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
                                      .OfType<JsonPropertyNameAttribute>()
                                      .FirstOrDefault();

        return targetAttr?.Name
               ?? serializerOptions.PropertyNamingPolicy?.ConvertName(memberExpression.Member.Name)
               ?? memberExpression.Member.Name;
    }

    // Evaluates the value of the key or index which may be an int or a string, 
    // or some other expression type.
    // The expression is converted to a delegate and the result of executing the delegate is returned as a string.
    private static string EvaluateExpression(Expression expression)
    {
        var converted = Expression.Convert(expression, typeof(object));
        var fakeParameter = Expression.Parameter(typeof(object), null);
        var lambda = Expression.Lambda<Func<object, object>>(converted, fakeParameter);
        var func = lambda.Compile();

        return Convert.ToString(func(null!), CultureInfo.InvariantCulture)!;
    }
}
