using BuildingBlocks.Abstractions.Domain;

namespace BuildingBlocks.Core.Domain.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a business rule is violated.
/// </summary>
/// <param name="brokenRule">The business rule that was violated.</param>
public class BusinessRuleValidationException(
    IBusinessRule brokenRule) : DomainException(brokenRule.Message)
{
    /// <summary>
    /// Gets the business rule that was violated.
    /// </summary>
    public IBusinessRule BrokenRule { get; } = brokenRule;

    /// <summary>
    /// Gets the details of the broken rule.
    /// </summary>
    public string Details { get; } = brokenRule.Message;

    /// <summary>
    /// Returns a string representation of the exception.
    /// </summary>
    /// <returns>The string representation of the exception.</returns>
    public override string ToString()
    {
        return $"{BrokenRule.GetType().FullName}: {BrokenRule.Message}";
    }
}