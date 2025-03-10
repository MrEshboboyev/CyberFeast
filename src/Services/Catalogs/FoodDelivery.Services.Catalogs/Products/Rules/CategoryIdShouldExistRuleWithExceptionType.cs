using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Abstractions.Domain;
using BuildingBlocks.Core.Extensions;
using FoodDelivery.Services.Catalogs.Categories;
using FoodDelivery.Services.Catalogs.Categories.Exceptions.Domain;
using FoodDelivery.Services.Catalogs.Shared.Contracts;

namespace FoodDelivery.Services.Catalogs.Products.Rules;

public class CategoryIdShouldExistRuleWithExceptionType(
    [NotNull] AggregateFuncOperation<CategoryId?, bool>? categoryChecker,
    [NotNull] CategoryId? id) : IBusinessRuleWithExceptionType<CategoryNotFoundException>
{
    public bool IsBroken()
    {
        categoryChecker.NotBeNull();
        id.NotBeNull();
        var exists = categoryChecker(id).GetAwaiter().GetResult();

        return !exists;
    }

    public CategoryNotFoundException Exception => new(GetType(), id!);
}
