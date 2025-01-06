namespace BuildingBlocks.Abstractions.Queries;

/// <summary>
/// Defines a query to retrieve a specific item by its identifier.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IItemQuery<out TId, out TResponse> : IQuery<TResponse>
    where TId : struct
    where TResponse : notnull
{
    /// <summary>
    /// Gets a list of related entities to include in the query.
    /// </summary>
    IList<string> Includes { get; }

    /// <summary>
    /// Gets the identifier of the item.
    /// </summary>
    TId Id { get; }
}