using BuildingBlocks.Core.Extensions;
using FoodDelivery.Services.Catalogs.Categories.Contracts;
using FoodDelivery.Services.Catalogs.Shared.Contracts;
using FoodDelivery.Services.Catalogs.Shared.Extensions;

namespace FoodDelivery.Services.Catalogs.Categories.Services;

public class CategoryChecker(ICatalogDbContext catalogDbContext) : ICategoryChecker
{
    public bool CategoryExists(CategoryId categoryId)
    {
        categoryId.NotBeNull();
        var category = catalogDbContext.FindCategory(categoryId);

        return category is not null;
    }
}