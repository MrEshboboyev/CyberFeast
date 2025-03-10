using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Validation.Extensions;
using FluentValidation;
using FoodDelivery.Services.Catalogs.Products.Exceptions.Application;
using FoodDelivery.Services.Catalogs.Products.ValueObjects;
using FoodDelivery.Services.Catalogs.Shared.Contracts;
using FoodDelivery.Services.Catalogs.Shared.Extensions;

namespace FoodDelivery.Services.Catalogs.Products.Features.ReplenishingProductStock.v1;

// we don't pass value-objects and domains to our commands and events, just primitive types
public record ReplenishProductStock(long ProductId, int Quantity) : ITransactionCommand
{
    public static ReplenishProductStock Of(long productId, int quantity)
    {
        return new ReplenishingProductStockValidator().HandleValidation(new ReplenishProductStock(productId, quantity));
    }
}

internal class ReplenishingProductStockValidator : AbstractValidator<ReplenishProductStock>
{
    public ReplenishingProductStockValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId must be greater than 0");
    }
}

internal class ReplenishingProductStockHandler(ICatalogDbContext catalogDbContext)
    : ICommandHandler<ReplenishProductStock>
{
    public async Task Handle(ReplenishProductStock command, CancellationToken cancellationToken)
    {
        command.NotBeNull();

        var (productId, quantity) = command;

        var product = await catalogDbContext.FindProductByIdAsync(ProductId.Of(productId));
        if (product is null)
            throw new ProductNotFoundException(productId);

        product.ReplenishStock(quantity);
        await catalogDbContext.SaveChangesAsync(cancellationToken);
    }
}
