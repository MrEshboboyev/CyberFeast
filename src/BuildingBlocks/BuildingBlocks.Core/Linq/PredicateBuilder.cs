using System.Linq.Expressions;

namespace BuildingBlocks.Core.Linq;

/// <summary>
/// Provides utility methods for building and combining LINQ expressions.
/// </summary>
public static class PredicateBuilder
{
    /// <summary>
    /// Constructs a LINQ expression based on property name, comparison operator, and value.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="propertyName">The property name to compare.</param>
    /// <param name="comparison">The comparison operator (e.g., "==", "!=", ">", etc.).</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>A LINQ expression representing the comparison.</returns>
    public static Expression<Func<T, bool>> Build<T>(
        string propertyName,
        string comparison,
        string value)
    {
        const string parameterName = "x";
        var parameter = Expression.Parameter(typeof(T), parameterName);

        var left = propertyName.Split('.')
            .Aggregate((Expression)parameter, Expression.Property);

        var body = MakeComparison(left, comparison, value);

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    /// <summary>
    /// Combines two LINQ expressions using the logical "AND" operator.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="a">The first expression.</param>
    /// <param name="b">The second expression.</param>
    /// <returns>A combined LINQ expression using the logical "AND" operator.</returns>
    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> a,
        Expression<Func<T, bool>> b)
    {
        var p = a.Parameters[0];

        var visitor = new SubstExpressionVisitor
        {
            Subst =
            {
                [b.Parameters[0]] = p
            }
        };

        Expression body = Expression.And(a.Body, visitor.Visit(b.Body));
        return Expression.Lambda<Func<T, bool>>(body, p);
    }

    /// <summary>
    /// Combines two LINQ expressions using the logical "OR" operator.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="a">The first expression.</param>
    /// <param name="b">The second expression.</param>
    /// <returns>A combined LINQ expression using the logical "OR" operator.</returns>
    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> a,
        Expression<Func<T, bool>> b)
    {
        var p = a.Parameters[0];

        var visitor = new SubstExpressionVisitor
        {
            Subst =
            {
                [b.Parameters[0]] = p
            }
        };

        Expression body = Expression.Or(a.Body, visitor.Visit(b.Body));

        return Expression.Lambda<Func<T, bool>>(body, p);
    }

    /// <summary>
    /// Creates a comparison expression based on the comparison operator and value.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="comparison">The comparison operator (e.g., "==", "!=", ">", etc.).</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>An expression representing the comparison.</returns>
    private static Expression MakeComparison(
        Expression left,
        string comparison,
        string value)
    {
        return comparison switch
        {
            "==" => MakeBinary(ExpressionType.Equal, left, value),
            "!=" => MakeBinary(ExpressionType.NotEqual, left, value),
            ">" => MakeBinary(ExpressionType.GreaterThan, left, value),
            ">=" => MakeBinary(ExpressionType.GreaterThanOrEqual, left, value),
            "<" => MakeBinary(ExpressionType.LessThan, left, value),
            "<=" => MakeBinary(ExpressionType.LessThanOrEqual, left, value),
            "Contains"
                or "StartsWith"
                or "EndsWith"
                => Expression.Call(
                    MakeString(left),
                    comparison,
                    Type.EmptyTypes,
                    Expression.Constant(value, typeof(string))
                ),
            "In" => MakeList(left, value.Split(',')),
            _ => throw new NotSupportedException($"Invalid comparison operator '{comparison}'.")
        };
    }

    /// <summary>
    /// Creates a "contains" expression for a list of values.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="codes">The list of values to compare against.</param>
    /// <returns>An expression representing the "contains" comparison.</returns>
    private static Expression MakeList(Expression left, IEnumerable<string> codes)
    {
        var objValues = codes.Cast<object>().ToList();
        var type = typeof(List<object>);
        var methodInfo = type.GetMethod("Contains", new[] { typeof(object) });
        var list = Expression.Constant(objValues);
        var body = Expression.Call(list, methodInfo!, left);
        return body;
    }

    /// <summary>
    /// Converts a source expression to a string.
    /// </summary>
    /// <param name="source">The source expression.</param>
    /// <returns>A string expression representing the source.</returns>
    private static Expression MakeString(Expression source)
    {
        return source.Type == typeof(string)
            ? source
            : Expression.Call(source, "ToString", Type.EmptyTypes);
    }

    /// <summary>
    /// Creates a binary expression based on the comparison operator and value.
    /// </summary>
    /// <param name="type">The type of the binary expression.</param>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>A binary expression representing the comparison.</returns>
    private static Expression MakeBinary(
        ExpressionType type,
        Expression left,
        string value)
    {
        object typedValue = value;
        if (left.Type != typeof(string))
        {
            if (string.IsNullOrEmpty(value))
            {
                typedValue = null!;
                if (Nullable.GetUnderlyingType(left.Type) == null)
                    left = Expression
                        .Convert(left, typeof(Nullable<>)
                            .MakeGenericType(left.Type));
            }
            else
            {
                var valueType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
                typedValue = valueType.IsEnum
                    ? Enum.Parse(valueType, value)
                    : valueType == typeof(Guid)
                        ? Guid.Parse(value)
                        : Convert.ChangeType(value, valueType);
            }
        }

        var right = Expression.Constant(typedValue, left.Type);
        return Expression.MakeBinary(type, left, right);
    }

    /// <summary>
    /// A custom expression visitor for substituting parameter expressions.
    /// </summary>
    private class SubstExpressionVisitor : ExpressionVisitor
    {
        public readonly Dictionary<Expression, Expression> Subst = new();

        /// <summary>
        /// Visits the parameter expression and substitutes it if a new value is provided.
        /// </summary>
        /// <param name="node">The parameter expression node.</param>
        /// <returns>The substituted expression if available; otherwise, the original node.</returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return Subst.TryGetValue(node, out var newValue)
                ? newValue
                : node;
        }
    }
}