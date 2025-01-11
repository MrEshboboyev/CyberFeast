using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Persistence.Marten.Subscriptions;

/// <summary>
/// Handles Marten event subscriptions and consumes events using configured consumers.
/// </summary>
public class MartenSubscription(
    IEnumerable<IMartenEventsConsumer> consumers,
    ILogger<MartenSubscription> logger
) : IProjection
{
    /// <summary>
    /// Intended to be used only in the async scope, not implemented for synchronous use.
    /// </summary>
    /// <param name="operations">The document operations.</param>
    /// <param name="streams">The stream actions.</param>
    public void Apply(IDocumentOperations operations, IReadOnlyList<StreamAction> streams) =>
        throw new NotImplementedException("Subscriptions should work only in the async scope");

    /// <summary>
    /// Consumes Marten events asynchronously using configured consumers.
    /// </summary>
    /// <param name="operations">The document operations.</param>
    /// <param name="streams">The stream actions.</param>
    /// <param name="cancellation">A token to cancel the operation.</param>
    public async Task ApplyAsync(
        IDocumentOperations operations,
        IReadOnlyList<StreamAction> streams,
        CancellationToken cancellation
    )
    {
        try
        {
            foreach (var consumer in consumers)
            {
                await consumer.ConsumeAsync(operations, streams, cancellation);
            }
        }
        catch (Exception exc)
        {
            logger.LogError("Error while processing Marten Subscription: {ExceptionMessage}", exc.Message);
            throw;
        }
    }
}