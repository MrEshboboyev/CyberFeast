using BuildingBlocks.Abstractions.Persistence;

namespace BuildingBlocks.Abstractions.Commands;

/// <summary>
/// Defines an internal command that is part of a transaction.
/// </summary>
public interface ITransactionInternalCommand : IInternalCommand, ITransactionRequest
{
}