using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Exception.Types;

/// <summary>
/// Represents an exception that occurs in the application layer.
/// </summary>
public class AppException : CustomException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code associated with the exception.</param>
    /// <param name="innerException">The inner exception.</param>
    public AppException(
        string message,
        int statusCode = StatusCodes.Status400BadRequest,
        System.Exception? innerException = null)
        : base(message, statusCode, innerException)
    {
    }
}