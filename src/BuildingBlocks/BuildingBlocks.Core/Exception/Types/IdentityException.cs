using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Exception.Types;

/// <summary>
/// Represents an identity-related exception.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="statusCode">The HTTP status code, defaulting to 400 (Bad Request).</param>
/// <param name="innerException">The inner exception, if any.</param>
/// <param name="errors">Additional error messages.</param>
public class IdentityException(
    string message,
    int statusCode = StatusCodes.Status400BadRequest,
    System.Exception? innerException = null,
    params string[] errors)
    : CustomException(message, statusCode, innerException, errors);