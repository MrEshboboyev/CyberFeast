using FluentValidation;
using FluentValidation.Results;
using ValidationException = BuildingBlocks.Core.Exception.Types.ValidationException;

namespace BuildingBlocks.Validation.Extensions;

/// <summary>
/// Contains extension methods for handling validation using FluentValidation.
/// </summary>
public static class ValidatorExtensions
{
    /// <summary>
    /// Converts a <see cref="ValidationResult"/> to a <see cref="ValidationResultModel"/>.
    /// </summary>
    /// <param name="validationResult">The validation result.</param>
    /// <returns>The validation result model.</returns>
    private static ValidationResultModel ToValidationResultModel(this ValidationResult? validationResult)
    {
        return new ValidationResultModel(validationResult);
    }

    /// <summary>
    /// Asynchronously validates a request and throws a <see cref="ValidationException"/> if validation fails.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <param name="validator">The validator.</param>
    /// <param name="request">The request to validate.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The validated request.</returns>
    /// <exception cref="ValidationException">Thrown if validation fails.</exception>
    public static async Task<TRequest> HandleValidationAsync<TRequest>(
        this IValidator<TRequest> validator,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator
            .ValidateAsync(request, cancellationToken).ConfigureAwait(false);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                validationResult.ToValidationResultModel().Errors?.FirstOrDefault()?.Message
                ?? validationResult.ToValidationResultModel().Message);
        }

        return request;
    }

    /// <summary>
    /// Synchronously validates a request and throws a <see cref="ValidationException"/> if validation fails.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <param name="validator">The validator.</param>
    /// <param name="request">The request to validate.</param>
    /// <returns>The validated request.</returns>
    /// <exception cref="ValidationException">Thrown if validation fails.</exception>
    public static TRequest HandleValidation<TRequest>(
        this IValidator<TRequest> validator,
        TRequest request)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                validationResult.ToValidationResultModel().Errors?.FirstOrDefault()?.Message
                ?? validationResult.ToValidationResultModel().Message);
        }

        return request;
    }
}