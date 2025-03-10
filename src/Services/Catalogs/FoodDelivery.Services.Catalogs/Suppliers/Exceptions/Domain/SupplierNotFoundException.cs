using BuildingBlocks.Core.Exception.Types;

namespace FoodDelivery.Services.Catalogs.Suppliers.Exceptions.Domain;

public class SupplierNotFoundException(Type businessRuleType, long id)
    : NotFoundDomainException(businessRuleType, $"Supplier with id '{id}' not found.");