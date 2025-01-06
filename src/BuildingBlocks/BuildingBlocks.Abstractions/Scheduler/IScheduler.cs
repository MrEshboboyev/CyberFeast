namespace BuildingBlocks.Abstractions.Scheduler;

/// <summary>
/// Defines methods for scheduling commands and messages.
/// </summary>
public interface IScheduler : ICommandScheduler, IMessageScheduler
{
    /// <summary>
    /// Schedules a serialized object at a specific time with an optional description.
    /// </summary>
    /// <param name="scheduleSerializedObject">The serialized object to schedule.</param>
    /// <param name="scheduleAt">The time to schedule the object.</param>
    /// <param name="description">An optional description of the scheduled object.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ScheduleAsync(
        ScheduleSerializedObject scheduleSerializedObject,
        DateTimeOffset scheduleAt,
        string? description = null);

    /// <summary>
    /// Schedules a serialized object after a delay with an optional description.
    /// </summary>
    /// <param name="scheduleSerializedObject">The serialized object to schedule.</param>
    /// <param name="delay">The delay after which to schedule the object.</param>
    /// <param name="description">An optional description of the scheduled object.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ScheduleAsync(
        ScheduleSerializedObject scheduleSerializedObject,
        TimeSpan delay,
        string? description = null);

    /// <summary>
    /// Schedules a recurring serialized object with a specified cron expression.
    /// </summary>
    /// <param name="scheduleSerializedObject">The serialized object to schedule.</param>
    /// <param name="name">The name of the recurring schedule.</param>
    /// <param name="cronExpression">The cron expression for the recurring schedule.</param>
    /// <param name="description">An optional description of the recurring schedule.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ScheduleRecurringAsync(
        ScheduleSerializedObject scheduleSerializedObject,
        string name,
        string cronExpression,
        string? description = null);
}