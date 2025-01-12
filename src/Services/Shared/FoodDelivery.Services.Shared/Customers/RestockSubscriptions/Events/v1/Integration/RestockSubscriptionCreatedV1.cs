using BuildingBlocks.Core.Messaging;

namespace FoodDelivery.Services.Shared.Customers.RestockSubscriptions.Events.V1.Integration;

/// <summary>
/// Represents an integration event for when a restock subscription is created.
/// </summary>
public record RestockSubscriptionCreatedV1(
    long CustomerId,
    string? Email) : IntegrationEvent;