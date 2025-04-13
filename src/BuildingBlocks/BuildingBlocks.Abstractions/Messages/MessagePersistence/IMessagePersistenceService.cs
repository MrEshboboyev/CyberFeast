using System.Linq.Expressions;
using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Abstractions.Messages.MessagePersistence;

public interface IMessagePersistenceService
{
    Task<IReadOnlyList<PersistMessage>> GetByFilterAsync(
        Expression<Func<PersistMessage, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<PersistMessage>> GetByFilterAsync(
        MessageStatus? status = null,
        MessageDeliveryType? deliveryType = null,
        string? dataType = null,
        CancellationToken cancellationToken = default
    );

    Task AddPublishMessageAsync(IMessageEnvelopeBase messageEnvelope, CancellationToken cancellationToken = default);

    Task AddReceivedMessageAsync<TMessage>(
        IMessageEnvelopeBase messageEnvelope,
        Func<IMessageEnvelopeBase, Task> dispatchAction,
        CancellationToken cancellationToken = default
    );

    Task AddInternalMessageAsync<TInternalCommand>(
        TInternalCommand internalCommand,
        CancellationToken cancellationToken = default
    )
        where TInternalCommand : IInternalCommand;

    Task AddNotificationAsync<TDomainNotification>(
        TDomainNotification notification,
        CancellationToken cancellationToken = default
    )
        where TDomainNotification : IDomainNotificationEvent<IDomainEvent>;

    Task MarkAsDeliveredAsync(Guid messageId, CancellationToken cancellationToken);
    Task ProcessAsync(Guid messageId, CancellationToken cancellationToken = default);

    Task ProcessAllAsync(CancellationToken cancellationToken = default);
}
