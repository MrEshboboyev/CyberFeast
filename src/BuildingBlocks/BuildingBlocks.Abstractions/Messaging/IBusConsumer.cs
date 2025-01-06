namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Defines methods for consuming messages.
/// </summary>
public interface IBusConsumer
{
    /// <summary>
    /// Consumes a message using the specified handler.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="handler">The handler to execute the message.</param>
    /// <param name="consumeBuilder">The configuration builder for message consumption.</param>
    void Consume<TMessage>(
        IMessageHandler<TMessage> handler,
        Action<IConsumeConfigurationBuilder>? consumeBuilder = null)
        where TMessage : IMessage;

    /// <summary>
    /// Consumes a message using the specified subscribe method.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="subscribeMethod">The delegate handler to execute the message.</param>
    /// <param name="consumeBuilder">The configuration builder for message consumption.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Consume<TMessage>(
        MessageHandler<TMessage> subscribeMethod,
        Action<IConsumeConfigurationBuilder>? consumeBuilder = null,
        CancellationToken cancellationToken = default)
        where TMessage : IMessage;

    /// <summary>
    /// Consumes a message with the specified message type, discovering a message handler for this type.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Consume<TMessage>(CancellationToken cancellationToken = default)
        where TMessage : IMessage;

    /// <summary>
    /// Consumes a message with the specified message type, discovering a message handler for this type.
    /// </summary>
    /// <param name="messageType">The type of the message.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Consume(Type messageType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consumes a message with the specified message type and handler.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Consume<THandler, TMessage>(CancellationToken cancellationToken = default)
        where THandler : IMessageHandler<TMessage>
        where TMessage : IMessage;

    /// <summary>
    /// Consumes all messages that implement the <see cref="IMessageHandler{TMessage}"/> interface.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ConsumeAll(CancellationToken cancellationToken = default);

    /// <summary>
    /// Consumes all messages that implement the <see cref="IMessageHandler{TMessage}"/> interface from the assembly of the provided type.
    /// </summary>
    /// <typeparam name="TType">The type for discovering the associated assembly.</typeparam>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ConsumeAllFromAssemblyOf<TType>(CancellationToken cancellationToken = default);

    /// <summary>
    /// Consumes all messages that implement the <see cref="IMessageHandler{TMessage}"/> interface from the assemblies of the provided types.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <param name="assemblyMarkerTypes">The types for discovering the associated assemblies.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ConsumeAllFromAssemblyOf(CancellationToken cancellationToken = default, params Type[] assemblyMarkerTypes);
}