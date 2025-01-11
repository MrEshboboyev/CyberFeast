using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Messaging;
using Humanizer;
using Microsoft.Extensions.Options;
using MessageHeaders = BuildingBlocks.Core.Messaging.MessageHeaders;

namespace BuildingBlocks.Integration.MassTransit;

/// <summary>
/// An implementation of the IExternalEventBus interface using MassTransit for message publishing and consuming.
/// </summary>
public class MassTransitBus(
    IBusDirectPublisher busDirectPublisher,
    IMessageMetadataAccessor messageMetadataAccessor,
    IMessagePersistenceService messagePersistenceService,
    IOptions<MessagingOptions> messagingOptions
) : IExternalEventBus
{
    private readonly MessagingOptions _messagingOptions = messagingOptions.Value;

    /// <summary>
    /// Publishes a message asynchronously.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IMessage
    {
        var correlationId = messageMetadataAccessor.GetCorrelationId();
        var cautionId = messageMetadataAccessor.GetCorrelationId();
        var messageTypeName = message.GetType().Name.Underscore();

        var eventEnvelope = EventEnvelope.From(
            message,
            correlationId,
            cautionId,
            new Dictionary<string, object?>
            {
                { MessageHeaders.ExchangeOrTopic, $"{messageTypeName}{MessagingConstants.PrimaryExchangePostfix}" },
                { MessageHeaders.Queue, messageTypeName }
            }
        );

        return PublishAsync(eventEnvelope, cancellationToken);
    }

    /// <summary>
    /// Publishes an event envelope asynchronously.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message contained in the envelope.</typeparam>
    /// <param name="eventEnvelope">The event envelope to publish.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PublishAsync<TMessage>(
        IEventEnvelope<TMessage> eventEnvelope,
        CancellationToken cancellationToken = default
    )
        where TMessage : IMessage
    {
        if (_messagingOptions.OutboxEnabled)
        {
            await messagePersistenceService.AddPublishMessageAsync(eventEnvelope, cancellationToken);
            return;
        }

        await busDirectPublisher.PublishAsync(eventEnvelope, cancellationToken);
    }

    /// <summary>
    /// Publishes a message to a specific exchange or topic and queue asynchronously.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
    /// <param name="message">The message to publish.</param>
    /// <param name="exchangeOrTopic">The exchange or topic to publish to.</param>
    /// <param name="queue">The queue to publish to.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PublishAsync<TMessage>(
        TMessage message,
        string? exchangeOrTopic = null,
        string? queue = null,
        CancellationToken cancellationToken = default
    )
        where TMessage : IMessage
    {
        var correlationId = messageMetadataAccessor.GetCorrelationId();
        var cautionId = messageMetadataAccessor.GetCorrelationId();
        var messageTypeName = message.GetType().Name.Underscore();

        var eventEnvelope = EventEnvelope.From(
            message,
            correlationId,
            cautionId,
            new Dictionary<string, object?>
            {
                {
                    MessageHeaders.ExchangeOrTopic,
                    exchangeOrTopic ?? $"{messageTypeName}{MessagingConstants.PrimaryExchangePostfix}"
                },
                { MessageHeaders.Queue, queue ?? messageTypeName }
            }
        );

        if (_messagingOptions.OutboxEnabled)
        {
            await messagePersistenceService.AddPublishMessageAsync(eventEnvelope, cancellationToken);
            return;
        }

        await busDirectPublisher.PublishAsync(eventEnvelope, exchangeOrTopic, queue, cancellationToken);
    }

    /// <summary>
    /// Publishes an event envelope to a specific exchange or topic and queue asynchronously.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message contained in the envelope.</typeparam>
    /// <param name="eventEnvelope">The event envelope to publish.</param>
    /// <param name="exchangeOrTopic">The exchange or topic to publish to.</param>
    /// <param name="queue">The queue to publish to.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PublishAsync<TMessage>(
        IEventEnvelope<TMessage> eventEnvelope,
        string? exchangeOrTopic = null,
        string? queue = null,
        CancellationToken cancellationToken = default
    )
        where TMessage : IMessage
    {
        var messageTypeName = eventEnvelope.Message.GetType().Name.Underscore();

        if (_messagingOptions.OutboxEnabled)
        {
            await messagePersistenceService.AddPublishMessageAsync(eventEnvelope, cancellationToken);
            return;
        }

        await busDirectPublisher.PublishAsync(
            eventEnvelope,
            exchangeOrTopic ?? $"{messageTypeName}{MessagingConstants.PrimaryExchangePostfix}",
            queue ?? messageTypeName,
            cancellationToken
        );
    }

    public void Consume<TMessage>(
        IMessageHandler<TMessage> handler,
        Action<IConsumeConfigurationBuilder>? consumeBuilder = null
    )
        where TMessage : IMessage
    {
    }

    public Task Consume<TMessage>(
        Abstractions.Messaging.MessageHandler<TMessage> subscribeMethod,
        Action<IConsumeConfigurationBuilder>? consumeBuilder = null,
        CancellationToken cancellationToken = default
    )
        where TMessage : IMessage
    {
        return Task.CompletedTask;
    }

    public Task Consume<TMessage>(CancellationToken cancellationToken = default)
        where TMessage : IMessage
    {
        return Task.CompletedTask;
    }

    public Task Consume(Type messageType, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task Consume<THandler, TMessage>(CancellationToken cancellationToken = default)
        where THandler : IMessageHandler<TMessage>
        where TMessage : IMessage
    {
        return Task.CompletedTask;
    }

    public Task ConsumeAll(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task ConsumeAllFromAssemblyOf<TType>(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task ConsumeAllFromAssemblyOf(
        CancellationToken cancellationToken = default,
        params Type[] assemblyMarkerTypes
    )
    {
        return Task.CompletedTask;
    }
}