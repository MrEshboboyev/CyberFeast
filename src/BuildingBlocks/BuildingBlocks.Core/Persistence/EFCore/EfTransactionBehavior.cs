using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BuildingBlocks.Core.Persistence.EFCore;

/// <summary>
/// Provides transaction behavior for MediatR requests, ensuring transactions are handled and domain events are published.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class EfTransactionBehavior<TRequest, TResponse>(
    IDbFacadeResolver dbFacadeResolver,
    ILogger<EfTransactionBehavior<TRequest, TResponse>> logger,
    IDomainEventPublisher domainEventPublisher,
    ISerializer serializer,
    IDomainEventContext domainEventContext
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    /// <summary>
    /// Handles the request, managing transactions and publishing domain events.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="next">The next request handler in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using (Serilog.Context.LogContext.PushProperty(
                   "RequestObject",
                   serializer.Serialize(request)))
        {
            if (request is not ITransactionRequest)
                return await next();

            logger.LogInformation(
                "{Prefix} Handled command {MediatrRequest}",
                nameof(EfTransactionBehavior<TRequest, TResponse>),
                typeof(TRequest).FullName);

            logger.LogDebug(
                "{Prefix} Handled command {MediatrRequest} with content {RequestContent}",
                nameof(EfTransactionBehavior<TRequest, TResponse>),
                typeof(TRequest).FullName,
                JsonSerializer.Serialize(request));

            logger.LogInformation(
                "{Prefix} Open the transaction for {MediatrRequest}",
                nameof(EfTransactionBehavior<TRequest, TResponse>),
                typeof(TRequest).FullName);

            var strategy = dbFacadeResolver.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                var isInnerTransaction = dbFacadeResolver.Database.CurrentTransaction is not null;
                var transaction =
                    dbFacadeResolver.Database.CurrentTransaction
                    ?? await dbFacadeResolver.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var response = await next();

                    logger.LogInformation(
                        "{Prefix} Executed the {MediatrRequest} request",
                        nameof(EfTransactionBehavior<TRequest, TResponse>),
                        typeof(TRequest).FullName);

                    var domainEvents = domainEventContext.GetAllUncommittedEvents();
                    await domainEventPublisher.PublishAsync(
                        domainEvents.ToArray(),
                        cancellationToken);

                    if (isInnerTransaction == false)
                        await transaction.CommitAsync(cancellationToken);

                    domainEventContext.MarkUncommittedDomainEventAsCommitted();

                    return response;
                }
                catch
                {
                    if (isInnerTransaction == false)
                        await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
    }
}