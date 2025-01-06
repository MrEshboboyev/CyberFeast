using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Exception.Types;

/// <summary>
/// Represents a bad request exception.
/// </summary>
public class BadRequestException : CustomException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="errors">Additional error messages.</param>
    public BadRequestException(
        string message,
        System.Exception? innerException = null,
        params string[] errors)
        : base(message, StatusCodes.Status400BadRequest, innerException, errors)
    {
    }
}