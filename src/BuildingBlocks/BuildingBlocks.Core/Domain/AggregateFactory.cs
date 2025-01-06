using System.Linq.Expressions;

namespace BuildingBlocks.Core.Domain;

/// <summary>
/// Provides a factory for creating instances of aggregates.
/// </summary>
/// <typeparam name="T">The type of the aggregate.</typeparam>
public static class AggregateFactory<T>
{
    private static readonly Func<T> _constructor = CreateTypeConstructor();

    private static Func<T> CreateTypeConstructor()
    {
        try
        {
            var newExpr = Expression.New(typeof(T));
            var func = Expression.Lambda<Func<T>>(newExpr);
            return func.Compile();
        }
        catch (ArgumentException)
        {
            return null!;
        }
    }

    /// <summary>
    /// Creates an instance of the aggregate type using the constructor delegate.
    /// </summary>
    /// <returns>An instance of the aggregate type.</returns>
    /// <exception cref="Exception">Thrown if the aggregate does not have a parameterless constructor.</exception>
    public static T CreateAggregate()
    {
        if (_constructor == null)
            throw new System.Exception($"Aggregate {typeof(T).Name} does not have a parameterless constructor");
        return _constructor();
    }
}