using Marten;
using Marten.Events;

namespace BuildingBlocks.Persistence.Marten.Subscriptions;

/// <summary>
/// Defines an interface for consuming Marten events asynchronously.
/// </summary>
public interface IMartenEventsConsumer
{
    /// <summary>
    /// Consumes Marten events asynchronously.
    /// </summary>
    /// <param name="documentOperations">The document operations.</param>
    /// <param name="streamActions">The stream actions.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task ConsumeAsync(
        IDocumentOperations documentOperations,
        IReadOnlyList<StreamAction> streamActions,
        CancellationToken cancellationToken = default
    );
}