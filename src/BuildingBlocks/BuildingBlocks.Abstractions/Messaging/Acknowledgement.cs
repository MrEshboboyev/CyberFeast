namespace BuildingBlocks.Abstractions.Messaging;

/// <summary>
/// Represents an acknowledgment for a message.
/// </summary>
public abstract class Acknowledgement
{
}

/// <summary>
/// Represents a successful acknowledgment for a message.
/// </summary>
public class Ack : Acknowledgement
{
}

/// <summary>
/// Represents a negative acknowledgment for a message.
/// </summary>
/// <param name="exception">The exception that caused the negative acknowledgment.</param>
/// <param name="requeue">Indicates whether the message should be requeued.</param>
public class Nack(Exception exception, bool requeue = true) : Acknowledgement
{
    public bool Requeue { get; } = requeue;
    public Exception Exception { get; } = exception;
}

/// <summary>
/// Represents a rejection acknowledgment for a message.
/// </summary>
/// <param name="requeue">Indicates whether the message should be requeued.</param>
public class Reject(bool requeue = true) : Acknowledgement
{
    public bool Requeue { get; } = requeue;
}
