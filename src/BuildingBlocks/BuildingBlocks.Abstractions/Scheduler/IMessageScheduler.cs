using BuildingBlocks.Abstractions.Messages;

namespace BuildingBlocks.Abstractions.Scheduler;

/// <summary>
/// Defines methods for scheduling messages.
/// </summary>
public interface IMessageScheduler
{
    /// <summary>
    /// Schedules a message asynchronously.
    /// </summary>
    /// <param name="message">The message to schedule.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ScheduleAsync(IMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Schedules multiple messages asynchronously.
    /// </summary>
    /// <param name="messages">The messages to schedule.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ScheduleAsync(IMessage[] messages, CancellationToken cancellationToken = default);
}