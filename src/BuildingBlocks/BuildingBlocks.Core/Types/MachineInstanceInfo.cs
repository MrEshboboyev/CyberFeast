using BuildingBlocks.Abstractions.Types;
using BuildingBlocks.Core.Extensions;

namespace BuildingBlocks.Core.Types;

/// <summary>
/// Represents information about a machine instance, including a client ID and client group.
/// </summary>
public record MachineInstanceInfo : IMachineInstanceInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MachineInstanceInfo"/> class.
    /// </summary>
    /// <param name="clientId">The unique identifier for the client.</param>
    /// <param name="clientGroup">The group to which the client belongs.</param>
    public MachineInstanceInfo(
        Guid clientId,
        string? clientGroup)
    {
        clientGroup.NotBeNullOrWhiteSpace();
        clientId.NotBeInvalid();

        ClientId = clientId;
        ClientGroup = clientGroup;
    }

    /// <summary>
    /// Gets the unique identifier for the client.
    /// </summary>
    public Guid ClientId { get; }

    /// <summary>
    /// Gets the group to which the client belongs.
    /// </summary>
    public string ClientGroup { get; }

    /// <summary>
    /// Creates a new instance of <see cref="MachineInstanceInfo"/> with a new GUID and the current domain's friendly name.
    /// </summary>
    /// <returns>A new instance of <see cref="MachineInstanceInfo"/>.</returns>
    internal static MachineInstanceInfo New() =>
        new(Guid.NewGuid(), AppDomain.CurrentDomain.FriendlyName);
}