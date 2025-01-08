using BuildingBlocks.Abstractions.Commands;
using MediatR;

namespace BuildingBlocks.Core.Commands;

/// <summary>
/// Provides a base implementation for command handlers without a response type.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Handles the command asynchronously.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task<Unit> HandleCommandAsync(
        TCommand command,
        CancellationToken cancellationToken);

    /// <summary>
    /// Handles the request by calling <see cref="HandleCommandAsync"/>.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task Handle(TCommand request, CancellationToken cancellationToken)
    {
        return HandleCommandAsync(request, cancellationToken);
    }
}

/// <summary>
/// Provides a base implementation for command handlers with a response type.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public abstract class CommandHandler<TCommand, TResponse>
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
    /// <summary>
    /// Handles the command asynchronously and returns a response.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a response.</returns>
    protected abstract Task<TResponse> HandleCommandAsync(
        TCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles the request by calling <see cref="HandleCommandAsync"/>.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a response.</returns>
    public Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken)
    {
        return HandleCommandAsync(request, cancellationToken);
    }
}