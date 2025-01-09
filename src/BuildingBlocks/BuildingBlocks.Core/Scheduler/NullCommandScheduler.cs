using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Scheduler;

namespace BuildingBlocks.Core.Scheduler;

/// <summary>
/// A command scheduler that performs no actual scheduling.
/// All methods return a completed task immediately.
/// </summary>
public class NullCommandScheduler : ICommandScheduler
{
    /// <summary>
    /// Schedules a single command with no delay.
    /// </summary>
    /// <param name="command">The command to schedule.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A completed task.</returns>
    public Task ScheduleAsync(IInternalCommand command, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Schedules multiple commands with no delay.
    /// </summary>
    /// <param name="commands">The commands to schedule.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A completed task.</returns>
    public Task ScheduleAsync(IInternalCommand[] commands, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Schedules a single command to run at a specific time with an optional description.
    /// </summary>
    /// <param name="command">The command to schedule.</param>
    /// <param name="scheduleAt">The time to schedule the command.</param>
    /// <param name="description">An optional description for the scheduled command.</param>
    /// <returns>A completed task.</returns>
    public Task ScheduleAsync(IInternalCommand command, DateTimeOffset scheduleAt, string? description = null)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Schedules multiple commands to run at a specific time with an optional description.
    /// </summary>
    /// <param name="commands">The commands to schedule.</param>
    /// <param name="scheduleAt">The time to schedule the commands.</param>
    /// <param name="description">An optional description for the scheduled commands.</param>
    /// <returns>A completed task.</returns>
    public Task ScheduleAsync(IInternalCommand[] commands, DateTimeOffset scheduleAt, string? description = null)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Schedules a recurring command with a specified name, cron expression, and optional description.
    /// </summary>
    /// <param name="command">The command to schedule.</param>
    /// <param name="name">The name of the recurring task.</param>
    /// <param name="cronExpression">The cron expression for the recurring schedule.</param>
    /// <param name="description">An optional description for the scheduled command.</param>
    /// <returns>A completed task.</returns>
    public Task ScheduleRecurringAsync(
        IInternalCommand command,
        string name,
        string cronExpression,
        string? description = null
    )
    {
        return Task.CompletedTask;
    }
}