using BuildingBlocks.Abstractions.Persistence;
using MediatR;

namespace BuildingBlocks.Abstractions.Commands;

/// <summary>
/// Defines a create command that is part of a transaction and returns a result.
/// </summary>
/// <typeparam name="TResponse">The type of the result.</typeparam>
public interface ITransactionCreateCommand<out TResponse> : 
    ICommand<TResponse>,
    ITransactionRequest
    where TResponse : notnull
{
}

/// <summary>
/// Defines a create command that is part of a transaction and returns a <see cref="Unit"/> result.
/// </summary>
public interface ITransactionCreateCommand : ITransactionCreateCommand<Unit>
{
}