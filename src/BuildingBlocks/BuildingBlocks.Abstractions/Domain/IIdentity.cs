namespace BuildingBlocks.Abstractions.Domain;

/// <summary>
/// Represents a generic identity interface for all types in the
/// Store.Services.Identity namespace with a generic identifier.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IIdentity<out TId>
{
    /// <summary>
    /// Gets the identifier value.
    /// </summary>
    TId Value { get; }
}
