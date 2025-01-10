using BuildingBlocks.Abstractions.Commands;

namespace BuildingBlocks.Core.Commands;

/// <summary>
/// Represents a transactional internal command.
/// </summary>
public abstract record TransactionInternalCommand : InternalCommand,
    ITransactionInternalCommand;