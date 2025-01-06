namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Defines methods for accessing message metadata.
/// </summary>
public interface IMessageMetadataAccessor
{
    /// <summary>
    /// Gets the CorrelationId from header storage and generates a new one if it does not exist.
    /// </summary>
    /// <returns>The correlation ID.</returns>
    Guid GetCorrelationId();

    /// <summary>
    /// Gets the CausationId from header storage.
    /// </summary>
    /// <returns>The causation ID.</returns>
    Guid? GetCautionId();
}