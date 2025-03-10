using BuildingBlocks.Core.Exception.Types;

namespace FoodDelivery.Services.Catalogs.Categories.Exceptions.Domain;

public class CategoryNotFoundException(Type businessRuleType, long id)
    : NotFoundDomainException(businessRuleType, $"Category with id '{id}' not found.");