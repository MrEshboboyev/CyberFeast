using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Core.Types;

namespace BuildingBlocks.Core.Commands;

/// <summary>
/// Represents an internal command with a unique identifier and occurrence timestamp.
/// </summary>
public abstract record InternalCommand : IInternalCommand
{
    /// <summary>
    /// Gets or sets the unique identifier for the internal command.
    /// </summary>
    public Guid InternalCommandId { get; protected set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the timestamp when the command occurred.
    /// </summary>
    public DateTime OccurredOn { get; protected set; } = DateTime.Now;

    /// <summary>
    /// Gets the full type name of the command.
    /// </summary>
    public string Type => TypeMapper.GetFullTypeName(GetType());
}