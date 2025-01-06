using BuildingBlocks.Abstractions.Persistence;

namespace BuildingBlocks.Abstractions.Commands;

/// <summary>
/// Defines a command that is part of a transaction.
/// </summary>
public interface ITransactionCommand : ICommand, ITransactionRequest
{
}

/// <summary>
/// Defines a command that is part of a transaction and returns a result.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
public interface ITransactionCommand<out T> : ICommand<T>, ITransactionRequest
    where T : notnull
{
}