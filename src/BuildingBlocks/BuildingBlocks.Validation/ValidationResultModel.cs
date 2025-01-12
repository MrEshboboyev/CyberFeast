using System.Net;
using FluentValidation.Results;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BuildingBlocks.Validation;

/// <summary>
/// Encapsulates the result of a validation process, including validation errors, status code, and message.
/// </summary>
public class ValidationResultModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResultModel"/> class.
    /// </summary>
    /// <param name="validationResult">The validation result.</param>
    public ValidationResultModel(ValidationResult? validationResult = null)
    {
        Errors = validationResult
            ?.Errors
            .Select(error => new ValidationError(error.PropertyName, error.ErrorMessage))
            .ToList() ?? new List<ValidationError>();

        Message = JsonConvert.SerializeObject(Errors);
    }

    /// <summary>
    /// Gets or sets the status code for the validation result.
    /// </summary>
    public int StatusCode { get; set; } = (int)HttpStatusCode.BadRequest;

    /// <summary>
    /// Gets or sets the validation error message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets the list of validation errors.
    /// </summary>
    public IList<ValidationError>? Errors { get; }

    /// <summary>
    /// Converts the validation result to a JSON string.
    /// </summary>
    /// <returns>The JSON representation of the validation result.</returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}