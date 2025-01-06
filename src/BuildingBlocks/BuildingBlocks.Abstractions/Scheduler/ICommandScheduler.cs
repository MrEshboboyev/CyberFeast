using BuildingBlocks.Abstractions.Commands;

namespace BuildingBlocks.Abstractions.Scheduler;

/// <summary>
/// Defines methods for scheduling internal commands.
/// </summary>
public interface ICommandScheduler
{
    /// <summary>
    /// Schedules an internal command asynchronously.
    /// </summary>
    /// <param name="command">The internal command to schedule.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ScheduleAsync(IInternalCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Schedules multiple internal commands asynchronously.
    /// </summary>
    /// <param name="commands">The internal commands to schedule.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ScheduleAsync(IInternalCommand[] commands, CancellationToken cancellationToken = default);

    /// <summary>
    /// Schedules an internal command at a specific time with an optional description.
    /// </summary>
    /// <param name="command">The internal command to schedule.</param>
    /// <param name="scheduleAt">The time to schedule the command.</param>
    /// <param name="description">An optional description of the scheduled command.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ScheduleAsync(IInternalCommand command, DateTimeOffset scheduleAt, string? description = null);

    /// <summary>
    /// Schedules multiple internal commands at a specific time with an optional description.
    /// </summary>
    /// <param name="commands">The internal commands to schedule.</param>
    /// <param name="scheduleAt">The time to schedule the commands.</param>
    /// <param name="description">An optional description of the scheduled commands.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ScheduleAsync(IInternalCommand[] commands, DateTimeOffset scheduleAt, string? description = null);

    /// <summary>
    /// Schedules a recurring internal command with a specified cron expression.
    /// </summary>
    /// <param name="command">The internal command to schedule.</param>
    /// <param name="name">The name of the recurring schedule.</param>
    /// <param name="cronExpression">The cron expression for the recurring schedule.</param>
    /// <param name="description">An optional description of the recurring schedule.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ScheduleRecurringAsync(
        IInternalCommand command,
        string name,
        string cronExpression,
        string? description = null);
}