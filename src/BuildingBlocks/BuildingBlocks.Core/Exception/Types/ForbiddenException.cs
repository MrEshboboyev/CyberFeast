using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Exception.Types;

/// <summary>
/// Represents a forbidden exception.
/// </summary>
public class ForbiddenException : IdentityException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception, if any.</param>
    public ForbiddenException(
        string message,
        System.Exception? innerException = null)
        : base(
            message,
            statusCode: StatusCodes.Status403Forbidden,
            innerException)
    {
    }
}