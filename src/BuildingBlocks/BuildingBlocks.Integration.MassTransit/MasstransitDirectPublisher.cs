using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.Messaging;
using Humanizer;
using MassTransit;
using RabbitMQ.Client;

namespace BuildingBlocks.Integration.MassTransit;

/// <summary>
/// An implementation of the IBusDirectPublisher interface using MassTransit for direct message publishing.
/// </summary>
public class MasstransitDirectPublisher(IBus bus) : IBusDirectPublisher
{
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
        await bus.Publish(
            eventEnvelope,
            envelopeWrapperContext =>
                FillMasstransitContextInformation(eventEnvelope, envelopeWrapperContext),
            cancellationToken
        );
    }

    /// <summary>
    /// Publishes a non-generic event envelope asynchronously.
    /// </summary>
    /// <param name="eventEnvelope">The event envelope to publish.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task PublishAsync(IEventEnvelope eventEnvelope, CancellationToken cancellationToken = default)
    {
        var messageType = eventEnvelope.Message.GetType();
        var publishMethod = typeof(IBusDirectPublisher)
            .GetMethods()
            .FirstOrDefault(x => 
                x.GetGenericArguments().Length != 0 && x.GetParameters().Length == 2)!;
        var genericPublishMethod = publishMethod.MakeGenericMethod(messageType);
        var publishTask = (Task)genericPublishMethod
            .Invoke(this, [eventEnvelope, cancellationToken])!;

        return publishTask!;
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
        var bindExchangeName = eventEnvelope.Message.GetType().Name.Underscore();

        if (string.IsNullOrEmpty(exchangeOrTopic))
        {
            exchangeOrTopic = $"{bindExchangeName}{MessagingConstants.PrimaryExchangePostfix}";
        }

        var endpointAddress = GetEndpointAddress(
            exchangeOrTopic: exchangeOrTopic,
            queue: queue,
            bindExchange: bindExchangeName,
            exchangeType: ExchangeType.Direct
        );

        var sendEndpoint = await bus.GetSendEndpoint(new Uri(endpointAddress));
        await sendEndpoint.Send(
            eventEnvelope,
            envelopeWrapperContext =>
                FillMasstransitContextInformation(eventEnvelope, envelopeWrapperContext),
            cancellationToken
        );
    }

    /// <summary>
    /// Publishes a non-generic event envelope to a specific exchange or topic and queue asynchronously.
    /// </summary>
    /// <param name="eventEnvelope">The event envelope to publish.</param>
    /// <param name="exchangeOrTopic">The exchange or topic to publish to.</param>
    /// <param name="queue">The queue to publish to.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task PublishAsync(
        IEventEnvelope eventEnvelope,
        string? exchangeOrTopic = null,
        string? queue = null,
        CancellationToken cancellationToken = default
    )
    {
        var messageType = eventEnvelope.Message.GetType();
        var publishMethod = typeof(IBusDirectPublisher)
            .GetMethods()
            .FirstOrDefault(x => 
                x.GetGenericArguments().Length != 0 && x.GetParameters().Length == 4)!;
        var genericPublishMethod = publishMethod.MakeGenericMethod(messageType);
        var publishTask = (Task)
            genericPublishMethod.Invoke(
                this,
                [eventEnvelope, exchangeOrTopic, queue, cancellationToken]
            )!;

        return publishTask!;
    }

    #region Private Methods
    
    /// <summary>
    /// Fills the MassTransit context information for publishing the event envelope.
    /// </summary>
    /// <param name="eventEnvelope">The event envelope.</param>
    /// <param name="envelopeWrapperContext">The MassTransit publish context.</param>
    private static void FillMasstransitContextInformation(
        IEventEnvelope eventEnvelope,
        PublishContext<IEventEnvelope> envelopeWrapperContext
    )
    {
        envelopeWrapperContext.MessageId = eventEnvelope.Metadata.MessageId;
        envelopeWrapperContext.CorrelationId = eventEnvelope.Metadata.CorrelationId;
        envelopeWrapperContext.SetRoutingKey(eventEnvelope.Message.GetType().Name.Underscore());

        foreach (var header in eventEnvelope.Metadata.Headers)
        {
            envelopeWrapperContext.Headers.Set(header.Key, header.Value);
        }
    }

    /// <summary>
    /// Fills the MassTransit context information for sending the event envelope.
    /// </summary>
    /// <param name="eventEnvelope">The event envelope.</param>
    /// <param name="envelopeWrapperContext">The MassTransit send context.</param>
    private static void FillMasstransitContextInformation(
        IEventEnvelope eventEnvelope,
        SendContext<IEventEnvelope> envelopeWrapperContext
    )
    {
        envelopeWrapperContext.MessageId = eventEnvelope.Metadata.MessageId;
        envelopeWrapperContext.CorrelationId = eventEnvelope.Metadata.CorrelationId;

        foreach (var header in eventEnvelope.Metadata.Headers)
        {
            envelopeWrapperContext.Headers.Set(header.Key, header.Value);
        }
    }

    /// <summary>
    /// Constructs the endpoint address for publishing or sending the event envelope.
    /// </summary>
    /// <param name="exchangeOrTopic">The exchange or topic.</param>
    /// <param name="queue">The queue.</param>
    /// <param name="bindExchange">The bind exchange.</param>
    /// <param name="exchangeType">The exchange type. Defaults to "direct".</param>
    /// <param name="bindQueue">Indicates whether to bind the queue. Defaults to false.</param>
    /// <returns>The constructed endpoint address.</returns>
    private static string GetEndpointAddress(
        string exchangeOrTopic,
        string? queue,
        string? bindExchange,
        string exchangeType = ExchangeType.Direct,
        bool bindQueue = false
    )
    {
        var endpoint = $"exchange:{exchangeOrTopic}?type={exchangeType}&durable=true";

        if (!string.IsNullOrEmpty(bindExchange))
        {
            endpoint += $"&bindexchange={bindExchange}";
        }

        if (!string.IsNullOrEmpty(queue))
        {
            endpoint += $"&queue={queue}";
        }

        if (bindQueue)
        {
            endpoint += "&bind=true";
        }

        return endpoint;
    }
    
    #endregion
}