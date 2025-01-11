using System.Text.Json;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Persistence.Mongo;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Persistence.Mongo;

/// <summary>
/// A behavior for handling transactions in MongoDB for requests implementing <see cref="ITransactionRequest"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class MongoTransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    private readonly IMongoDbContext _dbContext;
    private readonly ILogger<MongoTransactionBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoTransactionBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="dbContext">The MongoDB context.</param>
    /// <param name="logger">The logger instance.</param>
    public MongoTransactionBehavior(IMongoDbContext dbContext, ILogger<MongoTransactionBehavior<TRequest, TResponse>> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the request and manages the transaction.
    /// </summary>
    /// <param name="request">The request instance.</param>
    /// <param name="next">The next handler delegate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The response instance.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ITransactionRequest)
        {
            return await next();
        }

        _logger.LogInformation(
            "{Prefix} Handled command {MediatRRequest}",
            nameof(MongoTransactionBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName
        );
        _logger.LogDebug(
            "{Prefix} Handled command {MediatRRequest} with content {RequestContent}",
            nameof(MongoTransactionBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName,
            JsonSerializer.Serialize(request)
        );
        _logger.LogInformation(
            "{Prefix} Open the transaction for {MediatRRequest}",
            nameof(MongoTransactionBehavior<TRequest, TResponse>),
            typeof(TRequest).FullName
        );

        try
        {
            // Begin transaction
            await _dbContext.BeginTransactionAsync(cancellationToken);

            var response = await next();
            _logger.LogInformation(
                "{Prefix} Executed the {MediatRRequest} request",
                nameof(MongoTransactionBehavior<TRequest, TResponse>),
                typeof(TRequest).FullName
            );

            // Commit transaction
            await _dbContext.CommitTransactionAsync(cancellationToken);

            return response;
        }
        catch (Exception)
        {
            // Rollback transaction
            await _dbContext.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}