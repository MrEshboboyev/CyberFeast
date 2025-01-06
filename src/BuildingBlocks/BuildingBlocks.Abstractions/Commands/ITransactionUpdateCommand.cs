using BuildingBlocks.Abstractions.Persistence;
using MediatR;

namespace BuildingBlocks.Abstractions.Commands;

/// <summary>
/// Defines an update command that is part of a transaction and returns a result.
/// </summary>
/// <typeparam name="TResponse">The type of the result.</typeparam>
public interface ITransactionUpdateCommand<out TResponse> : 
    IUpdateCommand<TResponse>,
    ITransactionRequest
    where TResponse : notnull
{
}

/// <summary>
/// Defines an update command that is part of a transaction and returns a <see cref="Unit"/> result.
/// </summary>
public interface ITransactionUpdateCommand : ITransactionUpdateCommand<Unit>
{
}