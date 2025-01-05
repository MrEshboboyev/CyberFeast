namespace BuildingBlocks.Abstractions.Commands;

/// <summary>
/// Defines a delete command with an identifier and a return value.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
/// <typeparam name="TResponse">The type of the return value.</typeparam>
public interface IDeleteCommand<TId, out TResponse> : ICommand<TResponse>
    where TId : struct
    where TResponse : notnull
{
    /// <summary>
    /// Gets the identifier of the entity to delete.
    /// </summary>
    TId Id { get; init; }
}

/// <summary>
/// Defines a delete command with an identifier and no return value.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IDeleteCommand<TId> : ICommand
    where TId : struct
{
    /// <summary>
    /// Gets the identifier of the entity to delete.
    /// </summary>
    TId Id { get; init; }
}