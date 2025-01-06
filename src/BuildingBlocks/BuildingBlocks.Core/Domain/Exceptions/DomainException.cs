using BuildingBlocks.Core.Exception.Types;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Domain.Exceptions;

/// <summary>
/// Represents an exception that occurs in the domain layer.
/// </summary>
public class DomainException : CustomException
{
    private readonly Type? _brokenRuleType;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code associated with the exception.</param>
    public DomainException(
        string message,
        int statusCode = StatusCodes.Status409Conflict)
        : base(message, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class with a specific business rule type.
    /// </summary>
    /// <param name="businessRuleType">The type of the broken business rule.</param>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code associated with the exception.</param>
    public DomainException(
        Type businessRuleType,
        string message,
        int statusCode = StatusCodes.Status409Conflict)
        : base(message, statusCode)
    {
        _brokenRuleType = businessRuleType;
    }

    /// <summary>
    /// Returns the fully qualified name of this exception and the broken rule type.
    /// </summary>
    /// <returns>The fully qualified name of this exception and the broken rule type.</returns>
    public override string ToString()
    {
        return _brokenRuleType is not null
            ? $"{GetType().FullName}:{_brokenRuleType.FullName}"
            : $"{GetType().FullName}";
    }
}