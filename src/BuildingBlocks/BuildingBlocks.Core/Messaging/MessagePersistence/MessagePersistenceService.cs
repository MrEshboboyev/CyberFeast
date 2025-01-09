using System.Linq.Expressions;
using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Events.Internal;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Abstractions.Serialization;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Types;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.Messaging.MessagePersistence;

/// <summary>
/// Provides services for persisting and processing messages.
/// </summary>
public class MessagePersistenceService(
    ILogger<MessagePersistenceService> logger,
    IMessagePersistenceRepository messagePersistenceRepository,
    IMessageSerializer messageSerializer,
    IMediator mediator,
    IBusDirectPublisher busDirectPublisher,
    ISerializer serializer
) : IMessagePersistenceService
{
    /// <summary>
    /// Gets messages by filter asynchronously.
    /// </summary>
    /// <param name="predicate">The filter predicate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with a list of filtered messages as the result.</returns>
    public Task<IReadOnlyList<StoreMessage>> GetByFilterAsync(
        Expression<Func<StoreMessage, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        return messagePersistenceRepository
            .GetByFilterAsync(predicate ?? (_ => true), cancellationToken);
    }

    /// <summary>
    /// Adds a publishing message to the persistence store asynchronously.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="eventEnvelope">The event envelope containing the message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddPublishMessageAsync<TMessage>(
        IEventEnvelope<TMessage> eventEnvelope,
        CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        await AddMessageCore(eventEnvelope, MessageDeliveryType.Outbox, cancellationToken);
    }

    /// <summary>
    /// Adds a received message to the persistence store asynchronously.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="eventEnvelope">The event envelope containing the message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddReceivedMessageAsync<TMessage>(
        IEventEnvelope<TMessage> eventEnvelope,
        CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        await AddMessageCore(eventEnvelope, MessageDeliveryType.Inbox, cancellationToken);
    }

    /// <summary>
    /// Adds an internal message to the persistence store asynchronously.
    /// </summary>
    /// <typeparam name="TInternalCommand">The type of the internal command.</typeparam>
    /// <param name="internalCommand">The internal command to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddInternalMessageAsync<TInternalCommand>(
        TInternalCommand internalCommand,
        CancellationToken cancellationToken = default) where TInternalCommand : IInternalCommand
    {
        await messagePersistenceRepository.AddAsync(
            new StoreMessage(
                internalCommand.InternalCommandId,
                TypeMapper.GetFullTypeName(internalCommand.GetType()), // same process so we use full type name
                serializer.Serialize(internalCommand),
                MessageDeliveryType.Internal),
            cancellationToken
        );
    }

    /// <summary>
    /// Adds a notification event to the persistence store asynchronously.
    /// </summary>
    /// <typeparam name="TDomainNotification">The type of the domain notification event.</typeparam>
    /// <param name="notification">The notification event to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddNotificationAsync<TDomainNotification>(
        TDomainNotification notification,
        CancellationToken cancellationToken = default)
        where TDomainNotification : IDomainNotificationEvent
    {
        await messagePersistenceRepository.AddAsync(
            new StoreMessage(
                notification.EventId,
                TypeMapper.GetFullTypeName(notification.GetType()), // same process so we use full type name
                serializer.Serialize(notification),
                MessageDeliveryType.Internal),
            cancellationToken
        );
    }

    /// <summary>
    /// Core method for adding messages to the persistence store.
    /// </summary>
    /// <param name="eventEnvelope">The event envelope containing the message.</param>
    /// <param name="deliveryType">The delivery type of the message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AddMessageCore(
        IEventEnvelope eventEnvelope,
        MessageDeliveryType deliveryType,
        CancellationToken cancellationToken = default)
    {
        eventEnvelope.Message.NotBeNull();

        var id = eventEnvelope.Message is IMessage im ? im.MessageId : Guid.NewGuid();

        await messagePersistenceRepository.AddAsync(
            new StoreMessage(
                id,
                TypeMapper.GetFullTypeName(eventEnvelope.Message
                    .GetType()), // because each service has its own persistence and inbox and outbox processor will run in the same process we can use full type name
                messageSerializer.Serialize(eventEnvelope),
                deliveryType
            ),
            cancellationToken
        );

        logger.LogInformation(
            "Message with id: {MessageID} and delivery type: {DeliveryType} saved in persistence message store",
            id,
            deliveryType.ToString()
        );
    }

    /// <summary>
    /// Processes a message asynchronously by its ID.
    /// </summary>
    /// <param name="messageId">The ID of the message to process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ProcessAsync(
        Guid messageId,
        CancellationToken cancellationToken = default)
    {
        var message = await messagePersistenceRepository
            .GetByIdAsync(messageId, cancellationToken);

        if (message is null)
            return;

        switch (message.DeliveryType)
        {
            case MessageDeliveryType.Inbox:
                await ProcessInbox(message, cancellationToken);
                break;
            case MessageDeliveryType.Internal:
                await ProcessInternal(message, cancellationToken);
                break;
            case MessageDeliveryType.Outbox:
                await ProcessOutbox(message, cancellationToken);
                break;
        }

        await messagePersistenceRepository
            .ChangeStateAsync(message.Id, MessageStatus.Processed, cancellationToken);
    }

    /// <summary>
    /// Processes all unprocessed messages asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ProcessAllAsync(CancellationToken cancellationToken = default)
    {
        var messages = await messagePersistenceRepository
            .GetByFilterAsync(x => x.MessageStatus != MessageStatus.Processed,
                cancellationToken);

        foreach (var message in messages)
        {
            await ProcessAsync(message.Id, cancellationToken);
        }
    }

    /// <summary>
    /// Processes outbox messages asynchronously.
    /// </summary>
    /// <param name="storeMessage">The stored message to process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task ProcessOutbox(
        StoreMessage storeMessage,
        CancellationToken cancellationToken)
    {
        var messageType = TypeMapper.GetType(storeMessage.DataType);
        var eventEnvelope = messageSerializer.Deserialize(storeMessage.Data, messageType);

        if (eventEnvelope is null)
            return;

        await busDirectPublisher.PublishAsync(eventEnvelope, cancellationToken);

        logger.LogInformation(
            "Message with id: {MessageId} and delivery type: {DeliveryType} processed from the persistence message store",
            storeMessage.Id,
            storeMessage.DeliveryType
        );
    }

    /// <summary>
    /// Processes internal messages asynchronously.
    /// </summary>
    /// <param name="storeMessage">The stored message to process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task ProcessInternal(
        StoreMessage storeMessage,
        CancellationToken cancellationToken)
    {
        var messageType = TypeMapper.GetType(storeMessage.DataType);
        var internalMessage = serializer.Deserialize(storeMessage.Data, messageType);

        switch (internalMessage)
        {
            case null:
                return;
            case IDomainNotificationEvent domainNotificationEvent:
                await mediator.Publish(domainNotificationEvent, cancellationToken);

                logger.LogInformation(
                    "Domain-Notification with id: {EventID} and delivery type: {DeliveryType} processed from the persistence message store",
                    storeMessage.Id,
                    storeMessage.DeliveryType
                );
                break;
            case IInternalCommand internalCommand:
                await mediator.Send(internalCommand, cancellationToken);

                logger.LogInformation(
                    "InternalCommand with id: {EventID} and delivery type: {DeliveryType} processed from the persistence message store",
                    storeMessage.Id,
                    storeMessage.DeliveryType
                );
                break;
        }
    }

    /// <summary>
    /// Processes inbox messages asynchronously.
    /// </summary>
    /// <param name="storeMessage">The stored message to process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private Task ProcessInbox(
        StoreMessage storeMessage,
        CancellationToken cancellationToken)
    {
        var messageType = TypeMapper.GetType(storeMessage.DataType);
        var messageEnvelope = messageSerializer.Deserialize(storeMessage.Data, messageType);

        // Process the message envelope (this is a placeholder and needs actual implementation)

        return Task.CompletedTask;
    }
}