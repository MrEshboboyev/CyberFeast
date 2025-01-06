using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Exception.Types;

/// <summary>
/// Represents an exception that occurs in the API layer.
/// </summary>
public class ApiException : CustomException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code associated with the exception.</param>
    public ApiException(
        string message,
        int statusCode = StatusCodes.Status500InternalServerError)
        : base(message)
    {
        StatusCode = statusCode;
    }
}