namespace BuildingBlocks.Abstractions.Core;

/// <summary>
/// Defines a contract for generating unique identifiers.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IIdGenerator<out TId>
{
    /// <summary>
    /// Generates a new unique identifier.
    /// </summary>
    /// <returns>A new unique identifier of type TId.</returns>
    TId New();
}