namespace BuildingBlocks.Abstractions.Commands;

/// <summary>
/// Defines a create command with a return value.
/// </summary>
/// <typeparam name="TResponse">The type of the return value.</typeparam>
public interface ICreateCommand<out TResponse> : ICommand<TResponse>
    where TResponse : notnull
{
}

/// <summary>
/// Defines a create command with no return value.
/// </summary>
public interface ICreateCommand : ICommand
{
}