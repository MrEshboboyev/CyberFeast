namespace BuildingBlocks.Abstractions.Types;

/// <summary>
/// Defines properties for machine instance information.
/// </summary>
public interface IMachineInstanceInfo
{
    /// <summary>
    /// Gets the client group.
    /// </summary>
    string ClientGroup { get; }

    /// <summary>
    /// Gets the client ID.
    /// </summary>
    Guid ClientId { get; }
}