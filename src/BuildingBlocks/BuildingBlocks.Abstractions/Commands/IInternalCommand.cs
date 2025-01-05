using MediatR;

namespace BuildingBlocks.Abstractions.Commands;

/// <summary>
/// Defines the structure for an internal command.
/// </summary>
public interface IInternalCommand : ICommand
{
    /// <summary>
    /// Gets the unique identifier of the internal command.
    /// </summary>
    Guid InternalCommandId { get; }

    /// <summary>
    /// Gets the date and time when the internal command occurred.
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Gets the type of the internal command.
    /// </summary>
    string Type { get; }
}