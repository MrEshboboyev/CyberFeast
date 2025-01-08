using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Abstractions.Messaging.PersistMessage;
using MediatR;

namespace BuildingBlocks.Core.Commands;

/// <summary>
/// Implements the <see cref="ICommandBus"/> interface to send commands and schedule internal commands.
/// </summary>
public class CommandBus(
    IMediator mediator,
    IMessagePersistenceService messagePersistenceService) : ICommandBus
{
    /// <summary>
    /// Sends a command and returns a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the command.</returns>
    public async Task<TResult> SendAsync<TResult>(
        ICommand<TResult> command,
        CancellationToken cancellationToken = default) where TResult : class
    {
        return await mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Sends a command without returning a result.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SendAsync<TRequest>(
        TRequest command,
        CancellationToken cancellationToken = default) where TRequest : ICommand
    {
        await mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Schedules an internal command for execution.
    /// </summary>
    /// <param name="internalCommandCommand">The internal command to schedule.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ScheduleAsync(
        IInternalCommand internalCommandCommand,
        CancellationToken cancellationToken = default)
    {
        await messagePersistenceService.AddInternalMessageAsync(
            internalCommandCommand,
            cancellationToken);
    }

    /// <summary>
    /// Schedules multiple internal commands for execution.
    /// </summary>
    /// <param name="internalCommandCommands">The internal commands to schedule.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ScheduleAsync(
        IInternalCommand[] internalCommandCommands,
        CancellationToken cancellationToken = default)
    {
        foreach (var internalCommandCommand in internalCommandCommands)
        {
            await ScheduleAsync(internalCommandCommand, cancellationToken);
        }
    }
}