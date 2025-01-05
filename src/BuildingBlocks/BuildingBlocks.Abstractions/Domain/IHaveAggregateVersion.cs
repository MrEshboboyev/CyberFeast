namespace BuildingBlocks.Abstractions.Domain;

/// <summary>
/// Defines a property for the original version of the aggregate.
/// </summary>
public interface IHaveAggregateVersion
{
    /// <summary>
    /// Gets the original version of the aggregate. This is used to ensure optimistic concurrency,
    /// to check if there were no changes made to the aggregate state between load and save for the current operation.
    /// </summary>
    long OriginalVersion { get; }
}