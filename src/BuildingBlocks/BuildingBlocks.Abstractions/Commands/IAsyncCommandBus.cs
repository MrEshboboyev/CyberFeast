namespace BuildingBlocks.Abstractions.Commands;

/// <summary>
/// Defines methods for sending asynchronous commands.
/// </summary>
public interface IAsyncCommandBus
{
    /// <summary>
    /// Asynchronously sends an external command.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendExternalAsync<TCommand>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : IAsyncCommand;
}