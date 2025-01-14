using BuildingBlocks.Core.Messaging;

namespace FoodDelivery.Services.Shared.Catalogs.Suppliers.Events.V1.Integration;

/// <summary>
/// Represents the event when a supplier is created.
/// </summary>
public record SupplierCreatedV1(
    long Id,
    string Name) : IntegrationEvent;