using BuildingBlocks.Core.Messaging;

namespace FoodDelivery.Services.Shared.Catalogs.Suppliers.Events.V1.Integration;

/// <summary>
/// Represents the event when a supplier is deleted.
/// </summary>
public record SupplierDeletedV1(long Id) : IntegrationEvent;