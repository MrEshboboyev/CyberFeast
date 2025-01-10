using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Web.HeaderPropagation;
using BuildingBlocks.Core.Web.HeaderPropagation.Extensions;
using MassTransit;

namespace BuildingBlocks.Core.Messaging;

/// <summary>
/// Provides methods to access and manage message metadata.
/// </summary>
public class MessageMetadataAccessor(HeaderPropagationStore headerPropagationStore) : IMessageMetadataAccessor
{
    /// <summary>
    /// Retrieves the correlation ID for the current message. Generates a new ID if not set.
    /// </summary>
    /// <returns>The correlation ID.</returns>
    public Guid GetCorrelationId()
    {
        var cid = headerPropagationStore.GetCorrelationId();

        if (cid is not null)
            return (Guid)cid;

        var correlationId = NewId.NextGuid();
        headerPropagationStore.AddCorrelationId(correlationId);

        return correlationId;
    }

    /// <summary>
    /// Retrieves the causation ID for the current message.
    /// </summary>
    /// <returns>The causation ID, or null if not set.</returns>
    public Guid? GetCautionId()
    {
        return headerPropagationStore.GetCausationId();
    }
}