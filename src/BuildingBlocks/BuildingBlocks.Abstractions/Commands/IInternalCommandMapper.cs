using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Abstractions.Commands;

/// <summary>
/// Defines methods for mapping domain events to internal commands.
/// </summary>
public interface IInternalCommandMapper
{
    /// <summary>
    /// Maps a list of domain events to a list of internal commands.
    /// </summary>
    /// <param name="domainEvents">The list of domain events to map.</param>
    /// <returns>A read-only list of mapped internal commands.</returns>
    IReadOnlyList<IInternalCommand?>? MapToInternalCommands(IReadOnlyList<IDomainEvent> domainEvents);

    /// <summary>
    /// Maps a single domain event to an internal command.
    /// </summary>
    /// <param name="domainEvent">The domain event to map.</param>
    /// <returns>The mapped internal command.</returns>
    IInternalCommand? MapToInternalCommand(IDomainEvent domainEvent);
}