namespace BuildingBlocks.Core.Domain.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a concurrency conflict occurs.
/// </summary>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
/// <param name="id">The identifier of the aggregate where the conflict occurred.</param>
public class ConcurrencyException<TId>(TId id)
    : DomainException($"A different version than expected was found in aggregate {id}");