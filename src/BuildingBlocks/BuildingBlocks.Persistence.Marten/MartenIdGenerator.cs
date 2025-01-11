using Marten.Schema.Identity;

namespace BuildingBlocks.Persistence.Marten;

/// <summary>
/// Provides methods for generating new IDs using CombGuidIdGeneration.
/// </summary>
public static class MartenIdGenerator
{
    /// <summary>
    /// Generates a new GUID using CombGuidIdGeneration.
    /// </summary>
    /// <returns>A new GUID.</returns>
    public static Guid New() => CombGuidIdGeneration.NewGuid();
}