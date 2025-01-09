using BuildingBlocks.Abstractions.Core;

namespace BuildingBlocks.Core.IdsGenerator;

/// <summary>
/// Generates unique GUIDs.
/// </summary>
public class GuidIdGenerator : IIdGenerator<Guid>
{
    /// <summary>
    /// Generates a new unique GUID.
    /// </summary>
    public Guid New()
    {
        return Guid.NewGuid();
    }
}