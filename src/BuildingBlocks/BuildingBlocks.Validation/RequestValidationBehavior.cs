using System.Text.Json;
using BuildingBlocks.Validation.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Validation;

/// <summary>
/// Provides validation for MediatR requests.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RequestValidationBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestValidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="logger">The logger.</param>
    public RequestValidationBehavior(
        IServiceProvider serviceProvider,
        ILogger<RequestValidationBehavior<TRequest, TResponse>> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Handles the request by validating it, logging the request and response, and proceeding to the next handler.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The response from the next handler.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validator = _serviceProvider.GetService<IValidator<TRequest>>()!;
        if (validator is null)
            return await next();

        _logger.LogInformation(
            "[{Prefix}] Handle request={RequestData} and response={ResponseData}",
            nameof(RequestValidationBehavior<TRequest, TResponse>),
            typeof(TRequest).Name,
            typeof(TResponse).Name);

        _logger.LogDebug(
            "Handling {FullName} with content {Request}",
            typeof(TRequest).FullName,
            JsonSerializer.Serialize(request));

        await validator.HandleValidationAsync(request, cancellationToken);

        var response = await next();

        _logger.LogInformation("Handled {FullName}", typeof(TRequest).FullName);
        return response;
    }
}

/// <summary>
/// Provides validation for streaming requests.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class StreamRequestValidationBehavior<TRequest, TResponse> : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : IStreamRequest<TResponse>
    where TResponse : class
{
    private readonly ILogger<StreamRequestValidationBehavior<TRequest, TResponse>> _logger;
    private readonly IServiceProvider _serviceProvider;
    private IValidator<TRequest> _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamRequestValidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="validator">The validator.</param>
    public StreamRequestValidationBehavior(
        IServiceProvider serviceProvider,
        ILogger<StreamRequestValidationBehavior<TRequest, TResponse>> logger,
        IValidator<TRequest> validator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    /// <summary>
    /// Handles the streaming request by validating it, logging the request and response, and proceeding to the next handler.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">The next delegate in the pipeline.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An asynchronous enumerable of responses from the next handler.</returns>
    public async IAsyncEnumerable<TResponse> Handle(
        TRequest request,
        StreamHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _validator = _serviceProvider.GetService<IValidator<TRequest>>()!;
        if (_validator is null)
        {
            await foreach (var response in next().WithCancellation(cancellationToken))
            {
                yield return response;
            }

            yield break;
        }

        _logger.LogInformation(
            "[{Prefix}] Handle request={RequestData} and response={ResponseData}",
            nameof(StreamRequestValidationBehavior<TRequest, TResponse>),
            typeof(TResponse).Name
        );

        _logger.LogDebug(
            "Handling {FullName} with content {Request}",
            typeof(TRequest).FullName,
            JsonSerializer.Serialize(request)
        );

        _validator.HandleValidation(request);

        await foreach (var response in next().WithCancellation(cancellationToken))
        {
            yield return response;
            _logger.LogInformation("Handled {FullName}", typeof(TRequest).FullName);
        }
    }
}