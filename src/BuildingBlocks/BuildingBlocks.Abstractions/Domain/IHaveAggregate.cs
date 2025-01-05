using BuildingBlocks.Abstractions.Events;

namespace BuildingBlocks.Abstractions.Domain;

/// <summary>
/// Defines methods for managing aggregates, including checking business rules.
/// </summary>
public interface IHaveAggregate : IHaveDomainEvents, IHaveAggregateVersion
{
    /// <summary>
    /// Checks a specific business rule for the aggregate and throws an exception if the rule is not satisfied.
    /// </summary>
    /// <param name="rule">The business rule to check.</param>
    void CheckRule(IBusinessRule rule);
}