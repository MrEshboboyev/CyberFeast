using BuildingBlocks.Abstractions.Commands;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Validation.Extensions;
using FluentValidation;
using FoodDelivery.Services.Catalogs.Products.Exceptions.Application;
using FoodDelivery.Services.Catalogs.Shared.Contracts;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services.Catalogs.Products.Features.DebitingProductStock.v1;

// we don't pass value-objects and domains to our commands and events, just primitive types
internal record DebitProductStock(long ProductId, int Quantity) : ITransactionCommand
{
    public static DebitProductStock Of(long productId, int quantity)
    {
        return new DebitProductStockValidator().HandleValidation(new DebitProductStock(productId, quantity));
    }
}

internal class DebitProductStockValidator : AbstractValidator<DebitProductStock>
{
    public DebitProductStockValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId must be greater than 0");
    }
}

internal class DebitProductStockHandler(ICatalogDbContext catalogDbContext) : ICommandHandler<DebitProductStock>
{
    public async Task Handle(DebitProductStock command, CancellationToken cancellationToken)
    {
        command.NotBeNull();

        var (productId, quantity) = command;

        var product = await catalogDbContext.Products.FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);

        if (product is null)
            throw new ProductNotFoundException(productId);

        product.DebitStock(quantity);

        await catalogDbContext.SaveChangesAsync(cancellationToken);
    }
}
