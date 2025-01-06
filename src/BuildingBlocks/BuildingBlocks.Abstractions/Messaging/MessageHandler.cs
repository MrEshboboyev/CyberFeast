using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Defines a handler for a message of type <typeparamref name="TMessage"/>.
/// </summary>
/// <typeparam name="TMessage">The type of the message.</typeparam>
/// <param name="context">The event envelope containing the message.</param>
/// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
/// <returns>A task that represents the asynchronous operation.</returns>
public delegate Task MessageHandler<in TMessage>(
    IEventEnvelope<TMessage> context,
    CancellationToken cancellationToken = default)
    where TMessage : IMessage;

/// <summary>
/// Defines a handler for a message of type <typeparamref name="TMessage"/> that returns an acknowledgment.
/// </summary>
/// <typeparam name="TMessage">The type of the message.</typeparam>
/// <param name="context">The event envelope containing the message.</param>
/// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
/// <returns>A task that represents the asynchronous operation and contains the acknowledgment.</returns>
public delegate Task<Acknowledgement> MessageHandlerAck<in TMessage>(
    IEventEnvelope<TMessage> context,
    CancellationToken cancellationToken = default)
    where TMessage : IMessage;