using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Validation;

/// <summary>
/// Validates a request object of type <typeparamref name="T"/> using FluentValidation.
/// </summary>
/// <typeparam name="T">The type of the request object.</typeparam>
public class ValidateRequest<T>
{
    private ValidationResult Validation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateRequest{T}"/> class.
    /// </summary>
    /// <param name="value">The request object to validate.</param>
    /// <param name="validation">The validation result.</param>
    private ValidateRequest(T value, ValidationResult validation)
    {
        Value = value;
        Validation = validation;
    }

    /// <summary>
    /// Gets the request object.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Gets a value indicating whether the request object is valid.
    /// </summary>
    public bool IsValid => Validation.IsValid;

    /// <summary>
    /// Gets the validation errors as a dictionary with the field names as keys.
    /// </summary>
    public IDictionary<string, string[]> Errors =>
        Validation
            .Errors.GroupBy(x => x.PropertyName)
            .ToDictionary(
                x => x.Key,
                x => x.Select(e => e.ErrorMessage).ToArray());

    /// <summary>
    /// Deconstructs the validation result into a validity flag and the request object.
    /// </summary>
    /// <param name="isValid">Indicates whether the request object is valid.</param>
    /// <param name="value">The request object.</param>
    public void Deconstruct(out bool isValid, out T value)
    {
        isValid = IsValid;
        value = Value;
    }

    /// <summary>
    /// Binds and validates JSON data from the HTTP context as a request object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="context">The HTTP context containing the request data.</param>
    /// <param name="parameter">The parameter information.</param>
    /// <returns>A <see cref="ValidateRequest{T}"/> instance containing the request object and validation result.</returns>
    /// <exception cref="ArgumentException">Thrown if the request data is null or invalid.</exception>
    public static async ValueTask<ValidateRequest<T>> BindAsync(
        HttpContext context,
        ParameterInfo parameter)
    {
        // Only JSON is supported right now, no complex model binding
        var value = await context.Request.ReadFromJsonAsync<T>();
        var validator = context.RequestServices.GetRequiredService<IValidator<T>>();

        if (value is null)
        {
            throw new ArgumentException(parameter.Name);
        }

        var results = await validator.ValidateAsync(value);

        return new ValidateRequest<T>(value, results);
    }
}