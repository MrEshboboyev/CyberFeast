using BuildingBlocks.Core.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Exception.Types;

/// <summary>
/// Represents a conflict exception with a status code.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="innerException">The inner exception.</param>
public class ConflictException(string message, System.Exception? innerException = null)
    : CustomException(message, StatusCodes.Status409Conflict, innerException);

/// <summary>
/// Represents a conflict application exception with a status code.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="innerException">The inner exception.</param>
public class ConflictAppException(string message, System.Exception? innerException = null)
    : AppException(message, StatusCodes.Status409Conflict, innerException);

/// <summary>
/// Represents a conflict domain exception with a status code.
/// </summary>
public class ConflictDomainException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictDomainException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ConflictDomainException(string message)
        : base(message, StatusCodes.Status409Conflict)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictDomainException"/> class with a specified business rule type and error message.
    /// </summary>
    /// <param name="businessRuleType">The type of the broken business rule.</param>
    /// <param name="message">The error message.</param>
    public ConflictDomainException(Type businessRuleType, string message)
        : base(businessRuleType, message, StatusCodes.Status409Conflict)
    {
    }
}