namespace BuildingBlocks.Validation;

/// <summary>
/// Represents a validation error with a field and an error message.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class.
    /// </summary>
    /// <param name="field">The name of the field with the validation error.</param>
    /// <param name="message">The validation error message.</param>
    public ValidationError(string field, string message)
    {
        Field = string.IsNullOrEmpty(field) ? null : field;
        Message = message;
    }

    /// <summary>
    /// Gets the name of the field with the validation error, or null if the field name is not provided.
    /// </summary>
    public string? Field { get; }

    /// <summary>
    /// Gets the validation error message.
    /// </summary>
    public string Message { get; }
}