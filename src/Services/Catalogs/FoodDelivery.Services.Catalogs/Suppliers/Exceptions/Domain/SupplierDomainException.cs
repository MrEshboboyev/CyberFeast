using BuildingBlocks.Core.Domain.Exceptions;

namespace FoodDelivery.Services.Catalogs.Suppliers.Exceptions.Domain;

public class SupplierDomainException(string message) : DomainException(message);