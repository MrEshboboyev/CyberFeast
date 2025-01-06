using BuildingBlocks.Core.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Exception.Types;

/// <summary>
/// Represents a not found exception with a status code.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="innerException">The inner exception.</param>
public class NotFoundException(string message, System.Exception? innerException = null)
    : CustomException(message, StatusCodes.Status404NotFound, innerException);

/// <summary>
/// Represents a not found application exception with a status code.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="innerException">The inner exception.</param>
public class NotFoundAppException(string message, System.Exception? innerException = null)
    : AppException(message, StatusCodes.Status404NotFound, innerException);

/// <summary>
/// Represents a not found domain exception with a status code.
/// </summary>
public class NotFoundDomainException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundDomainException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public NotFoundDomainException(string message)
        : base(message, StatusCodes.Status404NotFound)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundDomainException"/> class with a specified business rule type and error message.
    /// </summary>
    /// <param name="businessRuleType">The type of the broken business rule.</param>
    /// <param name="message">The error message.</param>
    public NotFoundDomainException(Type businessRuleType, string message)
        : base(businessRuleType, message, StatusCodes.Status404NotFound)
    {
    }
}