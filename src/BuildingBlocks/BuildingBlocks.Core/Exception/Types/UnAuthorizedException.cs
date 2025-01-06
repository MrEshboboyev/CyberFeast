using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Exception.Types;


/// <summary>
/// Represents an unauthorized exception.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="innerException">The inner exception, if any.</param>
public class UnAuthorizedException(
    string message,
    System.Exception? innerException = null)
    : IdentityException(
        message, 
        StatusCodes.Status401Unauthorized,
        innerException);