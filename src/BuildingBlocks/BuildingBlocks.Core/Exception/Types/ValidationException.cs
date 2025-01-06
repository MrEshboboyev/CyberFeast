namespace BuildingBlocks.Core.Exception.Types;

/// <summary>
/// Represents a validation exception.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="innerException">The inner exception, if any.</param>
/// <param name="errors">Additional validation error messages.</param>
public class ValidationException(
    string message,
    System.Exception? innerException = null,
    params string[] errors)
    : BadRequestException(
        message,
        innerException,
        errors);