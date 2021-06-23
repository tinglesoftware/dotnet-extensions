using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Tingle.Extensions.JsonPatch.Converters;
using Tingle.Extensions.JsonPatch.Helpers;
using Tingle.Extensions.JsonPatch.Operations;
using Tingle.Extensions.JsonPatch.Properties;

namespace Tingle.Extensions.JsonPatch
{
    [JsonConverter(typeof(TypedJsonPatchDocumentConverter))]
    public class JsonPatchDocument<TModel> : IJsonPatchDocument where TModel : class
    {
        public List<Operation<TModel>> Operations { get; private set; }

        [JsonIgnore]
        public CaseTransformType CaseTransformType { get; set; }

        public JsonPatchDocument() : this(CaseTransformType.LowerCase) { }

        public JsonPatchDocument(CaseTransformType caseTransformType)
        {
            Operations = new List<Operation<TModel>>();
            CaseTransformType = caseTransformType;
        }

        // Create from list of operations
        public JsonPatchDocument(List<Operation<TModel>> operations)
        {
            Operations = operations ?? throw new ArgumentNullException(nameof(operations));
        }

        /// <summary>
        /// Add operation.  Will result in, for example,
        /// { "op": "add", "path": "/a/b/c", "value": [ "foo", "bar" ] }
        /// </summary>
        /// <typeparam name="TProp">value type</typeparam>
        /// <param name="path">target location</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public JsonPatchDocument<TModel> Add<TProp>(Expression<Func<TModel, TProp>> path, TProp value)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

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
        /// <returns></returns>
        public JsonPatchDocument<TModel> Add<TProp>(Expression<Func<TModel, IList<TProp>>> path, TProp value, int position)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Operations.Add(new Operation<TModel>(
                "add",
                GetPath(path, position.ToString()),
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
        public JsonPatchDocument<TModel> Add<TKey, TValue>(Expression<Func<TModel, IDictionary<TKey, TValue>>> path,
                                                           TKey key,
                                                           TValue value)
            where TKey : IConvertible
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Operations.Add(new Operation<TModel>(
                op: "add",
                path: GetPath(path, Convert.ToString(key)),
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
        /// <returns></returns>
        public JsonPatchDocument<TModel> Add<TProp>(Expression<Func<TModel, IList<TProp>>> path, TProp value)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Operations.Add(new Operation<TModel>(
                "add",
                GetPath(path, "-"),
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
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Remove<TProp>(Expression<Func<TModel, IList<TProp>>> path, int position)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Remove<TProp>(Expression<Func<TModel, IList<TProp>>> path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Remove<TKey, TValue>(Expression<Func<TModel, IDictionary<TKey, TValue>>> path, TKey key)
            where TKey : IConvertible
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Replace<TProp>(Expression<Func<TModel, IList<TProp>>> path, TProp value, int position)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Replace<TProp>(Expression<Func<TModel, IList<TProp>>> path, TProp value)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Replace<TKey, TValue>(Expression<Func<TModel, IDictionary<TKey, TValue>>> path,
                                                               TKey key,
                                                               TValue value)
            where TKey : IConvertible
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Operations.Add(new Operation<TModel>(
                op: "replace",
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
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, IList<TProp>>> from,
                                                     int positionFrom,
                                                     Expression<Func<TModel, TProp>> path)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
                                                     Expression<Func<TModel, IList<TProp>>> path,
                                                     int positionTo)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, IList<TProp>>> from,
                                                     int positionFrom,
                                                     Expression<Func<TModel, IList<TProp>>> path,
                                                     int positionTo)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, IList<TProp>>> from,
                                                     int positionFrom,
                                                     Expression<Func<TModel, IList<TProp>>> path)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Move<TProp>(Expression<Func<TModel, TProp>> from, Expression<Func<TModel, IList<TProp>>> path)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, IList<TProp>>> from,
                                                     int positionFrom,
                                                     Expression<Func<TModel, TProp>> path)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
                                                     Expression<Func<TModel, IList<TProp>>> path,
                                                     int positionTo)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, IList<TProp>>> from,
                                                     int positionFrom,
                                                     Expression<Func<TModel, IList<TProp>>> path,
                                                     int positionTo)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, IList<TProp>>> from,
                                                     int positionFrom,
                                                     Expression<Func<TModel, IList<TProp>>> path)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Copy<TProp>(Expression<Func<TModel, TProp>> from, Expression<Func<TModel, IList<TProp>>> path)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Operations.Add(new Operation<TModel>(
                "copy",
                GetPath(path, "-"),
                GetPath(from, null)));

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
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Test<TProp>(Expression<Func<TModel, IList<TProp>>> path, TProp value, int position)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Test<TProp>(Expression<Func<TModel, IList<TProp>>> path, TProp value)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

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
        public JsonPatchDocument<TModel> Test<TKey, TValue>(Expression<Func<TModel, IDictionary<TKey, TValue>>> path,
                                                            TKey key,
                                                            TValue value)
            where TKey : IConvertible
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Operations.Add(new Operation<TModel>(
                op: "test",
                path: GetPath(path, Convert.ToString(key)),
                from: null,
                value: value));

            return this;
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
                path += "/" + position;
                if (segments.Count == 0)
                {
                    return "/" + path;
                }
            }

            return "/" + path;
        }

        private List<string> GetPathSegments(Expression expr)
        {
            var listOfSegments = new List<string>();
            switch (expr.NodeType)
            {
                case ExpressionType.ArrayIndex:
                    var binaryExpression = (BinaryExpression)expr;
                    listOfSegments.AddRange(GetPathSegments(binaryExpression.Left));
                    listOfSegments.Add(ExpressionHelpers.CaseTransform(binaryExpression.Right.ToString(), CaseTransformType));
                    return listOfSegments;

                case ExpressionType.Call:
                    var methodCallExpression = (MethodCallExpression)expr;
                    listOfSegments.AddRange(GetPathSegments(methodCallExpression.Object));
                    listOfSegments.Add(ExpressionHelpers.CaseTransform(EvaluateExpression(methodCallExpression.Arguments[0]), CaseTransformType));
                    return listOfSegments;

                case ExpressionType.Convert:
                    listOfSegments.AddRange(GetPathSegments(((UnaryExpression)expr).Operand));
                    return listOfSegments;

                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expr;
                    listOfSegments.AddRange(GetPathSegments(memberExpression.Expression));
                    // Get property name, respecting JsonProperty attribute
                    listOfSegments.Add(ExpressionHelpers.CaseTransform(GetPropertyNameFromMemberExpression(memberExpression), CaseTransformType));
                    return listOfSegments;

                case ExpressionType.Parameter:
                    // Fits "x => x" (the whole document which is "" as JSON pointer)
                    return listOfSegments;

                default:
                    throw new InvalidOperationException(Resources.FormatExpressionTypeNotSupported(expr));
            }
        }

        private string GetPropertyNameFromMemberExpression(MemberExpression memberExpression)
        {
            var propertyInfo = memberExpression.Expression.Type.GetProperty(memberExpression.Member.Name);
            var targetAttr = propertyInfo?.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
                                          .OfType<JsonPropertyNameAttribute>()
                                          .FirstOrDefault();

            return targetAttr?.Name ?? memberExpression.Member.Name;
        }

        private static bool ContinueWithSubPath(ExpressionType expressionType)
        {
            return (expressionType == ExpressionType.ArrayIndex
                || expressionType == ExpressionType.Call
                || expressionType == ExpressionType.Convert
                || expressionType == ExpressionType.MemberAccess);

        }

        // Evaluates the value of the key or index which may be an int or a string, 
        // or some other expression type.
        // The expression is converted to a delegate and the result of executing the delegate is returned as a string.
        private static string EvaluateExpression(Expression expression)
        {
            var converted = Expression.Convert(expression, typeof(object));
            var fakeParameter = Expression.Parameter(typeof(object), null);
            var lambda = Expression.Lambda<Func<object?, object>>(converted, fakeParameter);
            var func = lambda.Compile();

            return Convert.ToString(func(null), CultureInfo.InvariantCulture);
        }
    }
}
