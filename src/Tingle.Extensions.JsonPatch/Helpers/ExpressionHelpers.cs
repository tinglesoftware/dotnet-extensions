﻿using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Tingle.Extensions.JsonPatch.Helpers
{
    /// <summary>
    /// Expression helpers. These can be used when implementing custom ObjectAdapters.
    /// </summary>
    public static class ExpressionHelpers
    {
        /// <summary>
        /// Returns the path from a given expression.  Takes JsonProperty into account.
        /// </summary>
        /// <typeparam name="T">Class type</typeparam>
        /// <typeparam name="TProp">Property type on class</typeparam>
        /// <param name="expr">Expression the path must be returned from</param>
        /// <param name="caseTransformType"></param>
        /// <returns>Property path</returns>
        public static string GetPath<T, TProp>(Expression<Func<T, TProp>> expr, CaseTransformType caseTransformType) where T : class
        {
            return "/" + GetPath(expr.Body, caseTransformType, true);
        }

        private static string? GetPath(Expression expr, CaseTransformType caseTransformType, bool firstTime)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.ArrayIndex:
                    var binaryExpression = (BinaryExpression)expr;

                    if (ContinueWithSubPath(binaryExpression.Left.NodeType, false))
                    {
                        var leftFromBinaryExpression = GetPath(binaryExpression.Left, caseTransformType, false);
                        return leftFromBinaryExpression + "/" + CaseTransform(binaryExpression.Right.ToString(), caseTransformType);
                    }
                    else
                    {
                        return CaseTransform(binaryExpression.Right.ToString(), caseTransformType);
                    }

                case ExpressionType.Call:
                    var methodCallExpression = (MethodCallExpression)expr;

                    if (ContinueWithSubPath(methodCallExpression.Object.NodeType, false))
                    {
                        var leftFromMemberCallExpression = GetPath(methodCallExpression.Object, caseTransformType, false);
                        return leftFromMemberCallExpression + "/" +
                            CaseTransform(GetIndexerInvocation(methodCallExpression.Arguments[0]), caseTransformType);
                    }
                    else
                    {
                        return CaseTransform(GetIndexerInvocation(methodCallExpression.Arguments[0]), caseTransformType);
                    }

                case ExpressionType.Convert:
                    return GetPath(((UnaryExpression)expr).Operand, caseTransformType, false);

                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expr;

                    if (ContinueWithSubPath(memberExpression.Expression.NodeType, false))
                    {
                        var left = GetPath(memberExpression.Expression, caseTransformType, false);

                        // if there's a JsonProperty attribute, we must return the PropertyName
                        // from the attribute rather than the member name 

                        var jsonPropertyAttribute =
                            memberExpression.Member.GetCustomAttributes(
                            typeof(JsonPropertyNameAttribute), false);

                        if (jsonPropertyAttribute.Length > 0)
                        {
                            // get value
                            var castedAttribrute = (JsonPropertyNameAttribute)jsonPropertyAttribute[0];
                            return left + "/" + CaseTransform(castedAttribrute.Name, caseTransformType);
                        }

                        return left + "/" + CaseTransform(memberExpression.Member.Name, caseTransformType);
                    }
                    else
                    {
                        // Same here: if there's a JsonProperty attribute, we must return the PropertyName
                        // from the attribute rather than the member name 

                        var jsonPropertyAttribute =
                            memberExpression.Member.GetCustomAttributes(
                            typeof(JsonPropertyNameAttribute), false);

                        if (jsonPropertyAttribute.Length > 0)
                        {
                            // get value
                            var castedAttribrute = (JsonPropertyNameAttribute)jsonPropertyAttribute[0];
                            return CaseTransform(castedAttribrute.Name, caseTransformType);
                        }

                        return CaseTransform(memberExpression.Member.Name, caseTransformType);
                    }

                case ExpressionType.Parameter:
                    // Fits "x => x" (the whole document which is "" as JSON pointer)
                    return firstTime ? "" : null;

                default:
                    return "";
            }
        }

        private static bool ContinueWithSubPath(ExpressionType expressionType, bool firstTime)
        {
            if (firstTime)
            {
                return (expressionType == ExpressionType.ArrayIndex
                       || expressionType == ExpressionType.Call
                       || expressionType == ExpressionType.Convert
                       || expressionType == ExpressionType.MemberAccess
                       || expressionType == ExpressionType.Parameter);
            }
            else
            {
                return (expressionType == ExpressionType.ArrayIndex
                    || expressionType == ExpressionType.Call
                    || expressionType == ExpressionType.Convert
                    || expressionType == ExpressionType.MemberAccess);
            }
        }

        private static string GetIndexerInvocation(Expression expression)
        {
            var converted = Expression.Convert(expression, typeof(object));
            var fakeParameter = Expression.Parameter(typeof(object), null);
            var lambda = Expression.Lambda<Func<object?, object>>(converted, fakeParameter);

            var func = lambda.Compile();
            return Convert.ToString(func(null), CultureInfo.InvariantCulture);
        }

        public static string CaseTransform(string propertyName, CaseTransformType type)
        {

            if (string.IsNullOrWhiteSpace(propertyName)) return propertyName;
            string result = type switch
            {
                CaseTransformType.CamelCase => char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1),
                CaseTransformType.LowerCase => propertyName.ToLowerInvariant(),
                CaseTransformType.UpperCase => propertyName.ToUpperInvariant(),
                CaseTransformType.OriginalCase => propertyName,
                _ => throw new NotImplementedException(),
            };
            return result;
        }
    }
}
