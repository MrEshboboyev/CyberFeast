using Marten;
using Marten.Events;
using MediatR;

namespace BuildingBlocks.Persistence.Marten.Subscriptions;

/// <summary>
/// Publishes Marten events to the MediatR pipeline.
/// </summary>
public class MartenEventPublisher(IMediator mediator) : IMartenEventsConsumer
{
    /// <summary>
    /// Consumes Marten events and publishes them to the MediatR pipeline asynchronously.
    /// </summary>
    /// <param name="documentOperations">The document operations.</param>
    /// <param name="streamActions">The stream actions.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    public async Task ConsumeAsync(
        IDocumentOperations documentOperations,
        IReadOnlyList<StreamAction> streamActions,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var @event in streamActions.SelectMany(streamAction => streamAction.Events))
        {
            await mediator.Publish(@event, cancellationToken);
        }
    }
}