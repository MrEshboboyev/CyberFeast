using MediatR;

namespace BuildingBlocks.Abstractions.Commands;

/// <summary>
/// Defines an update command with no return value.
/// </summary>
public interface IUpdateCommand : IUpdateCommand<Unit>
{
}

/// <summary>
/// Defines an update command with a return value.
/// </summary>
/// <typeparam name="TResponse">The type of the return value.</typeparam>
public interface IUpdateCommand<out TResponse> : ICommand<TResponse>
    where TResponse : notnull
{
}