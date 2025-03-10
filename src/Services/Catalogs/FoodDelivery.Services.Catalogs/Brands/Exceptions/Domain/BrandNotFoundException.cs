using BuildingBlocks.Core.Exception.Types;

namespace FoodDelivery.Services.Catalogs.Brands.Exceptions.Domain;

public class BrandNotFoundException(Type businessRuleType, long id)
    : NotFoundDomainException(businessRuleType, $"Brand with id '{id}' not found");