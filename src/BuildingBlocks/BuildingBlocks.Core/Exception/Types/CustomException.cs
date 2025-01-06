using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Exception.Types;

/// <summary>
/// Represents a custom exception that includes additional error messages and a status code.
/// </summary>
public class CustomException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code associated with the exception.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="errors">Additional error messages.</param>
    protected CustomException(
        string message,
        int statusCode = StatusCodes.Status500InternalServerError,
        System.Exception? innerException = null,
        params string[] errors
    )
        : base(message, innerException)
    {
        ErrorMessages = errors;
        StatusCode = statusCode;
    }

    /// <summary>
    /// Gets the additional error messages.
    /// </summary>
    public IEnumerable<string> ErrorMessages { get; protected set; }

    /// <summary>
    /// Gets the HTTP status code associated with the exception.
    /// </summary>
    public int StatusCode { get; protected set; }

    /// <summary>
    /// Returns the fully qualified name of this exception.
    /// </summary>
    /// <returns>The fully qualified name of this exception.</returns>
    public override string ToString()
    {
        return GetType().FullName ?? GetType().Name;
    }
}