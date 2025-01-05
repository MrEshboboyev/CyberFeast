namespace BuildingBlocks.Abstractions.Commands;

/// <summary>
/// Defines methods for sending and scheduling commands.
/// </summary>
public interface ICommandBus
{
    /// <summary>
    /// Asynchronously sends a command and returns a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation and contains the result of the command execution.</returns>
    Task<TResult> SendAsync<TResult>(
        ICommand<TResult> command,
        CancellationToken cancellationToken = default)
        where TResult : class;

    /// <summary>
    /// Asynchronously sends a command without returning a result.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendAsync<TCommand>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    /// <summary>
    /// Asynchronously schedules a single internal command.
    /// </summary>
    /// <param name="internalCommand">The internal command to schedule.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ScheduleAsync(
        IInternalCommand internalCommand,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously schedules multiple internal commands.
    /// </summary>
    /// <param name="internalCommandCommands">The internal commands to schedule.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ScheduleAsync(
        IInternalCommand[] internalCommandCommands,
        CancellationToken cancellationToken = default);
}