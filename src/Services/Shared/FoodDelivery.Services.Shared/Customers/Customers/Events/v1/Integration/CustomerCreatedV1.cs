using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Messaging;

namespace FoodDelivery.Services.Shared.Customers.Customers.Events.V1.Integration;

/// <summary>
/// Represents an integration event for when a customer is created.
/// </summary>
public record CustomerCreatedV1(long CustomerId) : IntegrationEvent
{
    /// <summary>
    /// Creates a new instance of <see cref="CustomerCreatedV1"/> with in-line validation.
    /// </summary>
    /// <param name="customerId">The ID of the created customer.</param>
    /// <returns>A validated <see cref="CustomerCreatedV1"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the customer ID is negative or zero.</exception>
    public static CustomerCreatedV1 Of(long customerId) =>
        new CustomerCreatedV1(customerId.NotBeNegativeOrZero());
}