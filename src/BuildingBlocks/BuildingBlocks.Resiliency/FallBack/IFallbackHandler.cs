using MediatR;

namespace BuildingBlocks.Resiliency.Fallback;

/// <summary>
/// Defines a method for handling fallback behavior in case of failures.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IFallbackHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the fallback behavior for the given request.
    /// </summary>
    /// <param name="request">The request to handle fallback for.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the fallback response.</returns>
    Task<TResponse> HandleFallbackAsync(TRequest request, CancellationToken cancellationToken);
}