using MediatR;

namespace BuildingBlocks.Abstractions.Commands;

/// <summary>
/// Defines a command with no return value in the CQRS pattern.
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Defines a command with a return value in the CQRS pattern.
/// </summary>
/// <typeparam name="T">The type of the return value.</typeparam>
public interface ICommand<out T> : IRequest<T>
    where T : notnull
{
}