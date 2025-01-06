using System.Diagnostics;

namespace BuildingBlocks.Abstractions.Messaging.Context;

/// <summary>
/// Defines the structure for a consume context, including a strongly-typed message and metadata.
/// </summary>
/// <typeparam name="TMessage">The type of the message.</typeparam>
public interface IConsumeContext<out TMessage> : IConsumeContext
    where TMessage : class, IMessage
{
    /// <summary>
    /// Gets the strongly-typed message being consumed.
    /// </summary>
    new TMessage Message { get; }
}

/// <summary>
/// Defines the structure for a consume context, including metadata and other contextual information.
/// </summary>
public interface IConsumeContext
{
    /// <summary>
    /// Gets the message being consumed.
    /// </summary>
    object Message { get; }

    /// <summary>
    /// Gets the headers associated with the message.
    /// </summary>
    IDictionary<string, object?> Headers { get; }

    /// <summary>
    /// Gets or sets the parent context for tracing.
    /// </summary>
    ActivityContext? ParentContext { get; set; }

    /// <summary>
    /// Gets the unique identifier of the message.
    /// </summary>
    Guid MessageId { get; }

    /// <summary>
    /// Gets the type of the message.
    /// </summary>
    string MessageType { get; }

    /// <summary>
    /// Gets the context items.
    /// </summary>
    ContextItems Items { get; }

    /// <summary>
    /// Gets the size of the message payload.
    /// </summary>
    int PayloadSize { get; }

    /// <summary>
    /// Gets the version of the message.
    /// </summary>
    ulong Version { get; }

    /// <summary>
    /// Gets the date and time when the message was created.
    /// </summary>
    DateTime Created { get; }
}